
using Microsoft.Extensions.Logging;
using Moq;
using WeatherApi.Clients;
using WeatherApi.Models;
using WeatherApi.Repositories;
using WeatherApi.Services;

namespace WeatherApi.Tests;

public class WeatherServiceTests
{
    [Fact]
    public async Task GetCurrentWeather_WhenApiFails_ReturnsStaleCache()
    {
        var city = new City
        {
            Id = 1,
            Name = "Heidelberg",
            IsSelected = true,
            Latitude = 49.4,
            Longitude = 8.7
        };

        var cached = new WeatherCache
        {
            CityId = 1,
            Temperature = 12.5,
            RainProbability = 30,
            Humidity = 70,
            WindSpeed = 10,
            LastUpdatedUtc = DateTimeOffset.UtcNow.AddMinutes(-20)
        };

        var cityRepository = new Mock<ICityRepository>();
        cityRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<City> { city });

        var weatherRepository = new Mock<IWeatherRepository>();
        weatherRepository.Setup(r => r.GetAsync(1))
            .ReturnsAsync(cached);

        var openMeteoClient = new Mock<IOpenMeteoClient>();
        openMeteoClient
            .Setup(c => c.GetWeatherAsync(49.4, 8.7))
            .ThrowsAsync(new HttpRequestException("API unavailable"));

        var logger = Mock.Of<ILogger<WeatherService>>();

        var service = new WeatherService(
            cityRepository.Object,
            weatherRepository.Object,
            openMeteoClient.Object,
            logger
        );

        var result = await service.GetCurrentWeatherAsync();

        Assert.Equal("Heidelberg", result.City);
        Assert.Equal(12.5, result.Weather.Temperature);
        Assert.Equal(cached.LastUpdatedUtc,
            result.Weather.LastUpdatedUtc);

        openMeteoClient.Verify(
            c => c.GetWeatherAsync(49.4, 8.7),
            Times.Once);
    }


    [Fact]
    public async Task GetCurrentWeather_WhenCacheIsFresh_DoesNotCallApi()
    {
        var city = new City
        {
            Id = 1,
            Name = "Heidelberg",
            IsSelected = true
        };

        var cached = new WeatherCache
        {
            CityId = 1,
            Temperature = 15.5,
            RainProbability = 20,
            Humidity = 65,
            WindSpeed = 8,
            LastUpdatedUtc = DateTimeOffset.UtcNow.AddMinutes(-5)
        };

        var cityRepository = new Mock<ICityRepository>();
        cityRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<City> { city });

        var weatherRepository = new Mock<IWeatherRepository>();
        weatherRepository.Setup(r => r.GetAsync(1))
            .ReturnsAsync(cached);

        var openMeteoClient = new Mock<IOpenMeteoClient>();

        var service = new WeatherService(
            cityRepository.Object,
            weatherRepository.Object,
            openMeteoClient.Object,
            Mock.Of<ILogger<WeatherService>>()
        );

        var result = await service.GetCurrentWeatherAsync();

        Assert.Equal(15.5, result.Weather.Temperature);
        Assert.Equal(cached.LastUpdatedUtc,
            result.Weather.LastUpdatedUtc);

        openMeteoClient.Verify(
            c => c.GetWeatherAsync(
                It.IsAny<double>(),
                It.IsAny<double>()),
            Times.Never);
    }


    [Fact]
    public async Task GetCurrentWeather_WhenCacheExpired_RefreshesCache()
    {
        var city = new City
        {
            Id = 1,
            Name = "Heidelberg",
            IsSelected = true,
            Latitude = 49.4,
            Longitude = 8.7
        };

        var cached = new WeatherCache
        {
            CityId = 1,
            Temperature = 10,
            RainProbability = 20,
            Humidity = 70,
            WindSpeed = 5,
            LastUpdatedUtc = DateTimeOffset.UtcNow.AddMinutes(-20)
        };

        var updatedAt = DateTimeOffset.UtcNow;

        var freshWeather = new WeatherSnapshot(
            18.5,
            40,
            60,
            12,
            updatedAt
        );

        var cityRepository = new Mock<ICityRepository>();
        cityRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<City> { city });

        var weatherRepository = new Mock<IWeatherRepository>();
        weatherRepository.Setup(r => r.GetAsync(1))
            .ReturnsAsync(cached);

        var openMeteoClient = new Mock<IOpenMeteoClient>();
        openMeteoClient
            .Setup(c => c.GetWeatherAsync(49.4, 8.7))
            .ReturnsAsync(freshWeather);

        var service = new WeatherService(
            cityRepository.Object,
            weatherRepository.Object,
            openMeteoClient.Object,
            Mock.Of<ILogger<WeatherService>>()
        );

        var result = await service.GetCurrentWeatherAsync();

        Assert.Equal(18.5, result.Weather.Temperature);
        Assert.Equal(updatedAt, result.Weather.LastUpdatedUtc);

        openMeteoClient.Verify(
            c => c.GetWeatherAsync(49.4, 8.7),
            Times.Once);

        weatherRepository.Verify(
            r => r.SaveAsync(It.Is<WeatherCache>(w =>
                w.CityId == 1 &&
                w.Temperature == 18.5 &&
                w.LastUpdatedUtc == updatedAt)),
            Times.Once);
    }


    [Fact]
    public async Task GetCurrentWeather_WhenNoCacheAndApiFails_ThrowsException()
    {
        var city = new City
        {
            Id = 1,
            Name = "Heidelberg",
            IsSelected = true,
            Latitude = 49.4,
            Longitude = 8.7
        };

        var cityRepository = new Mock<ICityRepository>();
        cityRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<City> { city });

        var weatherRepository = new Mock<IWeatherRepository>();
        weatherRepository.Setup(r => r.GetAsync(1))
            .ReturnsAsync((WeatherCache?)null);

        var openMeteoClient = new Mock<IOpenMeteoClient>();
        openMeteoClient
            .Setup(c => c.GetWeatherAsync(49.4, 8.7))
            .ThrowsAsync(new HttpRequestException("API unavailable"));

        var service = new WeatherService(
            cityRepository.Object,
            weatherRepository.Object,
            openMeteoClient.Object,
            Mock.Of<ILogger<WeatherService>>()
        );

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetCurrentWeatherAsync()
        );

        weatherRepository.Verify(
            r => r.SaveAsync(It.IsAny<WeatherCache>()),
            Times.Never);
    }


}
