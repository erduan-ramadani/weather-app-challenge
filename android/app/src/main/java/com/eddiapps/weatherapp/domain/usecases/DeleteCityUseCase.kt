package com.eddiapps.weatherapp.domain.usecases

import com.eddiapps.weatherapp.domain.repository.WeatherRepository

class DeleteCityUseCase(
    private val repository: WeatherRepository
) {
    suspend operator fun invoke(cityId: Int) =
        repository.deleteCity(cityId)
}