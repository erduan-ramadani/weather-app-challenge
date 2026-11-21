package com.eddiapps.weatherapp.data.remote

import com.eddiapps.weatherapp.data.remote.dto.CityDto
import com.eddiapps.weatherapp.data.remote.dto.CityWeatherDto
import com.eddiapps.weatherapp.data.remote.dto.CreateCityRequestDto
import com.eddiapps.weatherapp.data.remote.dto.GeoLocationDto
import retrofit2.http.Body
import retrofit2.http.DELETE
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path
import retrofit2.http.Query

interface WeatherApiService {
    @GET("api/cities")
    suspend fun getCities(): List<CityDto>

    @GET("api/weather/current")
    suspend fun getCurrentWeather(): CityWeatherDto

    @GET("api/cities/search")
    suspend fun searchCities(
        @Query("name") name: String
    ): List<GeoLocationDto>

    @POST("api/cities")
    suspend fun addCity(
        @Body request: CreateCityRequestDto
    ): CityDto

    @PUT("api/cities/{id}/select")
    suspend fun selectCity(@Path("id") cityId: Int)


    @DELETE("api/cities/{id}")
    suspend fun deleteCity(
        @Path("id") cityId: Int
    )

}