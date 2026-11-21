package com.eddiapps.weatherapp.domain.usecases

class WeatherUseCases(
    val getCities: GetCitiesUseCase,
    val getCurrentWeather: GetCurrentWeatherUseCase,
    val selectCity: SelectCityUseCase,
    val searchCities: SearchCitiesUseCase,
    val addCity: AddCityUseCase,
    val deleteCity: DeleteCityUseCase
)
