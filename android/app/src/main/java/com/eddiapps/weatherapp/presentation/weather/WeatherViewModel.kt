package com.eddiapps.weatherapp.presentation.weather

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.eddiapps.weatherapp.domain.exception.BackendUnavailableException
import com.eddiapps.weatherapp.domain.exception.CityAlreadyExistsException
import com.eddiapps.weatherapp.domain.usecases.WeatherUseCases
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import java.io.IOException
import kotlin.coroutines.cancellation.CancellationException

class WeatherViewModel(
    private val useCases: WeatherUseCases
) : ViewModel() {

    private val _uiState = MutableStateFlow(WeatherUiState())
    val uiState = _uiState.asStateFlow()

    private val _addCityState = MutableStateFlow(AddCityUiState())
    val addCityState = _addCityState.asStateFlow()

    init {
        loadCities()
    }

    private suspend fun safely(
        fallbackMessage: String,
        onError: (String) -> Unit,
        onFinished: () -> Unit = {},
        action: suspend () -> Unit
    ) {
        try {
            action()
        } catch (e: CancellationException) {
            throw e
        } catch (e: Exception) {
            val message = when (e) {
                is BackendUnavailableException, is IOException ->
                    "Keine Verbindung zum Server. Bitte versuche es später erneut."

                is CityAlreadyExistsException ->
                    "Diese Stadt wurde bereits hinzugefügt."

                else -> fallbackMessage
            }

            onError(message)
        } finally {
            onFinished()
        }
    }

    fun loadCities() {
        viewModelScope.launch {
            reloadCities()
        }
    }

    private suspend fun reloadCities() {
        _uiState.update {
            it.copy(
                isLoading = true,
                errorMessage = null,
                weatherErrorMessage = null,
                weather = null
            )
        }

        safely(
            fallbackMessage = "Städte konnten nicht geladen werden.",
            onError = { message ->
                _uiState.update {
                    it.copy(errorMessage = message)
                }
            },
            onFinished = {
                _uiState.update {
                    it.copy(isLoading = false)
                }
            }
        ) {
            val cities = useCases.getCities()

            _uiState.update {
                it.copy(
                    cities = cities,
                    isLoading = false
                )
            }

            if (cities.any { it.isSelected }) {
                loadWeather()
            }
        }
    }

    private suspend fun loadWeather() {
        _uiState.update {
            it.copy(
                isWeatherLoading = true,
                weatherErrorMessage = null
            )
        }

        safely(
            fallbackMessage = "Wetterdaten konnten nicht geladen werden.",
            onError = { message ->
                _uiState.update {
                    it.copy(weatherErrorMessage = message)
                }
            },
            onFinished = {
                _uiState.update {
                    it.copy(isWeatherLoading = false)
                }
            }
        ) {
            val weather = useCases.getCurrentWeather()

            _uiState.update {
                it.copy(weather = weather)
            }
        }
    }

    fun selectCity(cityId: Int) {
        val state = _uiState.value

        if (
            state.isLoading ||
            state.isDeletingCity ||
            state.cities.any { it.id == cityId && it.isSelected }
        ) return

        _uiState.update {
            it.copy(
                isLoading = true,
                errorMessage = null,
                weather = null
            )
        }

        viewModelScope.launch {
            safely(
                fallbackMessage = "Stadt konnte nicht ausgewählt werden.",
                onError = { message ->
                    _uiState.update {
                        it.copy(
                            isLoading = false,
                            errorMessage = message
                        )
                    }
                }
            ) {
                useCases.selectCity(cityId)
                reloadCities()
            }
        }
    }

    fun openAddCityDialog() {
        _addCityState.value = AddCityUiState(isOpen = true)
    }

    fun closeAddCityDialog() {
        val state = _addCityState.value

        if (state.isSearching || state.isAddingCity) return

        _addCityState.update {
            it.copy(isOpen = false)
        }
    }

    fun searchCities(name: String) {
        val query = name.trim()
        val state = _addCityState.value

        if (
            query.isBlank() ||
            state.isSearching ||
            state.isAddingCity
        ) return

        _addCityState.update {
            it.copy(
                isSearching = true,
                searchResults = emptyList(),
                hasSearchedCities = false,
                searchErrorMessage = null,
                addCityErrorMessage = null
            )
        }

        viewModelScope.launch {
            safely(
                fallbackMessage = "Bei der Stadtsuche ist ein Fehler aufgetreten.",
                onError = { message ->
                    _addCityState.update {
                        it.copy(searchErrorMessage = message)
                    }
                },
                onFinished = {
                    _addCityState.update {
                        it.copy(isSearching = false)
                    }
                }
            ) {
                val results = useCases.searchCities(query)

                _addCityState.update {
                    it.copy(
                        searchResults = results,
                        hasSearchedCities = true
                    )
                }
            }
        }
    }

    fun addCity(locationId: Int) {
        val state = _addCityState.value

        if (state.isAddingCity || state.isSearching) return

        _addCityState.update {
            it.copy(
                isAddingCity = true,
                addCityErrorMessage = null
            )
        }

        viewModelScope.launch {
            safely(
                fallbackMessage = "Stadt konnte nicht hinzugefügt werden.",
                onError = { message ->
                    _addCityState.update {
                        it.copy(addCityErrorMessage = message)
                    }
                },
                onFinished = {
                    _addCityState.update {
                        it.copy(isAddingCity = false)
                    }
                }
            ) {
                useCases.addCity(locationId)

                _addCityState.update {
                    it.copy(
                        isOpen = false,
                        searchResults = emptyList()
                    )
                }

                reloadCities()
            }
        }
    }

    fun deleteCity(cityId: Int) {
        val state = _uiState.value

        if (state.isDeletingCity || state.isLoading) return

        _uiState.update {
            it.copy(
                isDeletingCity = true,
                deleteCityErrorMessage = null
            )
        }

        viewModelScope.launch {
            safely(
                fallbackMessage = "Stadt konnte nicht gelöscht werden.",
                onError = { message ->
                    _uiState.update {
                        it.copy(deleteCityErrorMessage = message)
                    }
                },
                onFinished = {
                    _uiState.update {
                        it.copy(isDeletingCity = false)
                    }
                }
            ) {
                useCases.deleteCity(cityId)
                reloadCities()
            }
        }
    }

    fun refreshWeather() {
        val state = _uiState.value

        if (
            state.isLoading ||
            state.isWeatherLoading ||
            state.cities.none { it.isSelected }
        ) return

        viewModelScope.launch {
            loadWeather()
        }
    }
}
