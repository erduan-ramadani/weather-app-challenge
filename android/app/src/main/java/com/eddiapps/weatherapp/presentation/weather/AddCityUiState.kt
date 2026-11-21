package com.eddiapps.weatherapp.presentation.weather

import com.eddiapps.weatherapp.domain.model.CitySuggestion

data class AddCityUiState(
    val isOpen: Boolean = false,
    val searchResults: List<CitySuggestion> = emptyList(),
    val hasSearchedCities: Boolean = false,
    val isSearching: Boolean = false,
    val isAddingCity: Boolean = false,
    val searchErrorMessage: String? = null,
    val addCityErrorMessage: String? = null
)
