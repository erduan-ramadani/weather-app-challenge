package com.eddiapps.weatherapp.presentation.weather

import com.eddiapps.weatherapp.domain.model.City
import com.eddiapps.weatherapp.domain.model.Weather

data class WeatherUiState(
    val cities: List<City> = emptyList(),
    val weather: Weather? = null,
    val isLoading: Boolean = false,
    val isWeatherLoading: Boolean = false,
    val errorMessage: String? = null,
    val weatherErrorMessage: String? = null,
    val isDeletingCity: Boolean = false,
    val deleteCityErrorMessage: String? = null
)