package com.eddiapps.weatherapp.domain.model

data class CitySuggestion(
    val locationId: Int,
    val name: String,
    val country: String,
    val region: String?
)
