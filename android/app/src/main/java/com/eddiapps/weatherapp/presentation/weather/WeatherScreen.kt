package com.eddiapps.weatherapp.presentation.weather

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.material3.Button
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.eddiapps.weatherapp.domain.model.City
import com.eddiapps.weatherapp.presentation.weather.components.AddCityDialog
import com.eddiapps.weatherapp.presentation.weather.components.CityDropdown
import com.eddiapps.weatherapp.presentation.weather.components.DeleteCityDialog
import com.eddiapps.weatherapp.presentation.weather.components.WeatherCard
import org.koin.compose.viewmodel.koinViewModel

@Composable
fun WeatherScreen(
    viewModel: WeatherViewModel = koinViewModel()
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    val addCityState by viewModel.addCityState.collectAsStateWithLifecycle()

    var cityToDelete by remember { mutableStateOf<City?>(null) }
    val isBusy = uiState.isLoading || uiState.isDeletingCity
    val selectedCity = uiState.cities.firstOrNull { it.isSelected }

    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .padding(24.dp),
        verticalArrangement = Arrangement.spacedBy(16.dp)
    ) {
        item {
            Text(
                text = "WeatherApp",
                style = MaterialTheme.typography.headlineMedium
            )
        }

        item {
            Button(
                onClick = viewModel::openAddCityDialog,
                enabled = !isBusy
            ) {
                Text("Stadt hinzufügen")
            }
        }

        when {
            uiState.isLoading -> item {
                CircularProgressIndicator()
            }

            uiState.errorMessage != null -> item {
                Column(
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    Text(
                        text = uiState.errorMessage.orEmpty(),
                        color = MaterialTheme.colorScheme.error
                    )

                    Button(onClick = viewModel::loadCities) {
                        Text("Erneut versuchen")
                    }
                }
            }

            uiState.cities.isEmpty() -> item {
                Text("Noch keine Städte vorhanden.")
            }

            else -> {
                item {
                    CityDropdown(
                        cities = uiState.cities,
                        onCitySelected = viewModel::selectCity,
                        enabled = !isBusy
                    )
                }

                selectedCity?.let { city ->
                    item {
                        OutlinedButton(
                            onClick = { cityToDelete = city },
                            enabled = !isBusy
                        ) {
                            Text("Stadt löschen")
                        }
                    }
                }

                if (uiState.isDeletingCity) {
                    item {
                        LinearProgressIndicator(
                            modifier = Modifier.fillMaxWidth()
                        )
                    }
                }

                uiState.deleteCityErrorMessage?.let { message ->
                    item {
                        Text(
                            text = message,
                            color = MaterialTheme.colorScheme.error
                        )
                    }
                }

                item {
                    when {
                        uiState.isWeatherLoading -> {
                            CircularProgressIndicator()
                        }

                        uiState.weatherErrorMessage != null -> {
                            Column(
                                verticalArrangement = Arrangement.spacedBy(8.dp)
                            ) {
                                Text(
                                    text = uiState.weatherErrorMessage.orEmpty(),
                                    color = MaterialTheme.colorScheme.error
                                )

                                OutlinedButton(
                                    onClick = viewModel::refreshWeather
                                ) {
                                    Text("Erneut versuchen")
                                }
                            }
                        }

                        else -> {
                            uiState.weather?.let { weather ->
                                WeatherCard(weather = weather)
                            }
                        }
                    }
                }
            }
        }
    }

    if (addCityState.isOpen) {
        AddCityDialog(
            state = addCityState,
            onSearch = viewModel::searchCities,
            onAddCity = viewModel::addCity,
            onDismiss = viewModel::closeAddCityDialog
        )
    }

    cityToDelete?.let { city ->
        DeleteCityDialog(
            cityName = city.name,
            onConfirm = {
                cityToDelete = null
                viewModel.deleteCity(city.id)
            },
            onDismiss = { cityToDelete = null }
        )
    }
}
