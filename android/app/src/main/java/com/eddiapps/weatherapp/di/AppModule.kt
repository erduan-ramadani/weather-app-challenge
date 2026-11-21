package com.eddiapps.weatherapp.di

import com.eddiapps.weatherapp.BuildConfig
import com.eddiapps.weatherapp.data.remote.WeatherApiService
import com.eddiapps.weatherapp.data.repository.WeatherRepositoryImpl
import com.eddiapps.weatherapp.domain.repository.WeatherRepository
import com.eddiapps.weatherapp.domain.usecases.AddCityUseCase
import com.eddiapps.weatherapp.domain.usecases.DeleteCityUseCase
import com.eddiapps.weatherapp.domain.usecases.GetCitiesUseCase
import com.eddiapps.weatherapp.domain.usecases.GetCurrentWeatherUseCase
import com.eddiapps.weatherapp.domain.usecases.SearchCitiesUseCase
import com.eddiapps.weatherapp.domain.usecases.SelectCityUseCase
import com.eddiapps.weatherapp.domain.usecases.WeatherUseCases
import com.eddiapps.weatherapp.presentation.weather.WeatherViewModel
import org.koin.core.module.dsl.viewModel
import org.koin.dsl.module
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

val appModule = module {

    single<WeatherRepository> {
        WeatherRepositoryImpl(get())
    }
    
    single { GetCitiesUseCase(get()) }
    single { GetCurrentWeatherUseCase(get()) }
    single { SearchCitiesUseCase(get()) }
    single { SelectCityUseCase(get()) }
    single { AddCityUseCase(get()) }
    single { DeleteCityUseCase(get()) }

    single<WeatherApiService> {
        Retrofit.Builder()
            .baseUrl(BuildConfig.BASE_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(WeatherApiService::class.java)
    }

    single {
        WeatherUseCases(
            getCities = get(),
            getCurrentWeather = get(),
            searchCities = get(),
            selectCity = get(),
            addCity = get(),
            deleteCity = get()
        )
    }

    viewModel { WeatherViewModel(get()) }
}
