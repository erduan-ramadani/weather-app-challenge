package com.eddiapps.weatherapp.domain.usecases

import com.eddiapps.weatherapp.domain.model.Weather
import com.eddiapps.weatherapp.domain.repository.WeatherRepository

class GetCurrentWeatherUseCase(
    private val repository: WeatherRepository
) {
    suspend operator fun invoke(): Weather =
        repository.getCurrentWeather()
}