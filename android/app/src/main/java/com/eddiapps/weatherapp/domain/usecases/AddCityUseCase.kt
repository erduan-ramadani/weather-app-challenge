package com.eddiapps.weatherapp.domain.usecases

import com.eddiapps.weatherapp.domain.repository.WeatherRepository

class AddCityUseCase(
    private val repository: WeatherRepository
) {
    suspend operator fun invoke(locationId: Int) =
        repository.addCity(locationId)
}