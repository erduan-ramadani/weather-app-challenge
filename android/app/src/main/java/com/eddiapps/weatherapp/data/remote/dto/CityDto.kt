package com.eddiapps.weatherapp.data.remote.dto

data class CityDto(
    val id: Int,
    val locationId: Int,
    val name: String,
    val country: String,
    val region: String?,
    val latitude: Double,
    val longitude: Double,
    val isSelected: Boolean
)