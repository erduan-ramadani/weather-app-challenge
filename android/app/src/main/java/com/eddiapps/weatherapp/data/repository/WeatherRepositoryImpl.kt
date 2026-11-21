package com.eddiapps.weatherapp.data.repository

import com.eddiapps.weatherapp.data.remote.WeatherApiService
import com.eddiapps.weatherapp.data.remote.dto.CreateCityRequestDto
import com.eddiapps.weatherapp.domain.exception.BackendUnavailableException
import com.eddiapps.weatherapp.domain.exception.CityAlreadyExistsException
import com.eddiapps.weatherapp.domain.model.City
import com.eddiapps.weatherapp.domain.model.CitySuggestion
import com.eddiapps.weatherapp.domain.model.Weather
import com.eddiapps.weatherapp.domain.repository.WeatherRepository
import retrofit2.HttpException
import java.io.IOException

class WeatherRepositoryImpl(
    private val api: WeatherApiService
) : WeatherRepository {

    override suspend fun getCities(): List<City> {
        return api.getCities().map { dto ->
            City(
                id = dto.id,
                name = dto.name,
                country = dto.country,
                region = dto.region,
                isSelected = dto.isSelected
            )
        }
    }


    override suspend fun getCurrentWeather(): Weather {
        val response = api.getCurrentWeather()

        return Weather(
            city = response.city,
            temperature = response.weather.temperature,
            rainProbability = response.weather.rainProbability,
            humidity = response.weather.humidity,
            windSpeed = response.weather.windSpeed,
            lastUpdatedUtc = response.weather.lastUpdatedUtc
        )
    }

    override suspend fun selectCity(cityId: Int) {
        api.selectCity(cityId)
    }


    override suspend fun searchCities(name: String): List<CitySuggestion> {
        try {
            return api.searchCities(name).map { dto ->
                CitySuggestion(
                    locationId = dto.id,
                    name = dto.name,
                    country = dto.country,
                    region = dto.region
                )
            }
        } catch (e: IOException) {
            throw BackendUnavailableException(e)
        }
    }


    override suspend fun addCity(locationId: Int) {
        try {
            api.addCity(CreateCityRequestDto(locationId))
        } catch (e: HttpException) {
            if (e.code() == 409) {
                throw CityAlreadyExistsException()
            }
            throw e
        }
    }

    override suspend fun deleteCity(cityId: Int) {
        api.deleteCity(cityId)
    }
}