
using WeatherApi.Clients;
using WeatherApi.Exceptions;
using WeatherApi.Models;
using WeatherApi.Repositories;

namespace WeatherApi.Services;

public record CityWeather(string City, WeatherSnapshot Weather);

public class WeatherService
{
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(15);

    private readonly ICityRepository _cityRepository;
    private readonly IWeatherRepository _weatherRepository;
    private readonly IOpenMeteoClient _openMeteoClient;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(
        ICityRepository cityRepository,
        IWeatherRepository weatherRepository,
        IOpenMeteoClient openMeteoClient,
        ILogger<WeatherService> logger)
    {
        _cityRepository = cityRepository;
        _weatherRepository = weatherRepository;
        _openMeteoClient = openMeteoClient;
        _logger = logger;
    }

    public async Task<CityWeather> GetCurrentWeatherAsync()
    {
        var cities = await _cityRepository.GetAllAsync();

        var city = cities.FirstOrDefault(c => c.IsSelected)
            ?? throw new NoSelectedCityException();

        var cached = await _weatherRepository.GetAsync(city.Id);

        if (cached != null &&
            DateTimeOffset.UtcNow - cached.LastUpdatedUtc < CacheLifetime)
        {
            return new CityWeather(city.Name, ToSnapshot(cached));
        }

        WeatherSnapshot weather;

        try
        {
            weather = await _openMeteoClient.GetWeatherAsync(
                city.Latitude,
                city.Longitude
            );
        }
        catch (Exception ex) when (
            cached != null &&
            (ex is HttpRequestException
                or TaskCanceledException
                or WeatherProviderResponseException))
        {
            _logger.LogWarning(
                ex,
                "Weather API unavailable. Using cached data for {City}.",
                city.Name
            );

            return new CityWeather(city.Name, ToSnapshot(cached));
        }


        await _weatherRepository.SaveAsync(new WeatherCache
        {
            CityId = city.Id,
            Temperature = weather.Temperature,
            RainProbability = weather.RainProbability,
            Humidity = weather.Humidity,
            WindSpeed = weather.WindSpeed,
            LastUpdatedUtc = weather.LastUpdatedUtc
        });

        return new CityWeather(city.Name, weather);
    }

    private static WeatherSnapshot ToSnapshot(WeatherCache cache)
    {
        return new WeatherSnapshot(
            cache.Temperature,
            cache.RainProbability,
            cache.Humidity,
            cache.WindSpeed,
            cache.LastUpdatedUtc
        );
    }
}
