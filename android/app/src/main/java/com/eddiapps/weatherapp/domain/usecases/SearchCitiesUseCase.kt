package com.eddiapps.weatherapp.domain.usecases

import com.eddiapps.weatherapp.domain.model.CitySuggestion
import com.eddiapps.weatherapp.domain.repository.WeatherRepository

class SearchCitiesUseCase(
    private val repository: WeatherRepository
) {
    suspend operator fun invoke(name: String): List<CitySuggestion> =
        repository.searchCities(name)
}