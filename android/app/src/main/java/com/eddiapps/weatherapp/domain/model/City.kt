package com.eddiapps.weatherapp.domain.model

data class City(
    val id: Int,
    val name: String,
    val country: String,
    val region: String?,
    val isSelected: Boolean
)