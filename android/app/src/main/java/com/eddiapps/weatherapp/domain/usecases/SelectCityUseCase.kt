package com.eddiapps.weatherapp.domain.usecases

import com.eddiapps.weatherapp.domain.repository.WeatherRepository

class SelectCityUseCase(
    private val repository: WeatherRepository
) {
    suspend operator fun invoke(cityId: Int) =
        repository.selectCity(cityId)
}