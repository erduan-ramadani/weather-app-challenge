package com.eddiapps.weatherapp.domain.usecases

import com.eddiapps.weatherapp.domain.model.City
import com.eddiapps.weatherapp.domain.repository.WeatherRepository

class GetCitiesUseCase(
    private val repository: WeatherRepository
) {
    suspend operator fun invoke(): List<City> =
        repository.getCities()
}