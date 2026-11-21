package com.eddiapps.weatherapp.data.remote.dto

data class WeatherDto(
    val temperature: Double,
    val rainProbability: Int?,
    val humidity: Int,
    val windSpeed: Double,
    val lastUpdatedUtc: String
)
