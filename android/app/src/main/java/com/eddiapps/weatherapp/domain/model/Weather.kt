package com.eddiapps.weatherapp.domain.model

data class Weather(
    val city: String,
    val temperature: Double,
    val rainProbability: Int?,
    val humidity: Int,
    val windSpeed: Double,
    val lastUpdatedUtc: String
)
