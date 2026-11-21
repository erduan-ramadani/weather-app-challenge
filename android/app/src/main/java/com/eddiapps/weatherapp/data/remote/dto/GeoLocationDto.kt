package com.eddiapps.weatherapp.data.remote.dto

data class GeoLocationDto(
    val id: Int,
    val name: String,
    val country: String,
    val region: String?,
    val latitude: Double,
    val longitude: Double
)
