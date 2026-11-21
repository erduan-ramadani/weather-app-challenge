package com.eddiapps.weatherapp.domain.repository

import com.eddiapps.weatherapp.domain.model.City
import com.eddiapps.weatherapp.domain.model.CitySuggestion
import com.eddiapps.weatherapp.domain.model.Weather

interface WeatherRepository {

    suspend fun getCities(): List<City>

    suspend fun getCurrentWeather(): Weather

    suspend fun selectCity(cityId: Int)

    suspend fun searchCities(name: String): List<CitySuggestion>

    suspend fun addCity(locationId: Int)

    suspend fun deleteCity(cityId: Int)
}