using Moq;
using WeatherApi.Clients;
using WeatherApi.Exceptions;
using WeatherApi.Models;
using WeatherApi.Repositories;
using WeatherApi.Services;

namespace WeatherApi.Tests;

public class CityServiceTests
{

    [Fact]
    public async Task SelectAsync_WhenCityExists_SelectsOnlyTargetCity()
    {
        var cities = new List<City>
    {
        new City
        {
            Id = 1,
            Name = "Heidelberg",
            IsSelected = true
        },
        new City
        {
            Id = 2,
            Name = "Berlin",
            IsSelected = false
        }
    };

        var cityRepository = new Mock<ICityRepository>();
        cityRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(cities);

        var service = new CityService(
            cityRepository.Object,
            Mock.Of<IOpenMeteoClient>()
        );

        var result = await service.SelectAsync(2);

        Assert.True(result);
        Assert.False(cities[0].IsSelected);
        Assert.True(cities[1].IsSelected);

        cityRepository.Verify(
            r => r.SaveChangesAsync(),
            Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_WhenSelectedCityDeleted_SelectsAnotherCity()
    {
        var cities = new List<City>
    {
        new City
        {
            Id = 1,
            Name = "Heidelberg",
            IsSelected = true
        },
        new City
        {
            Id = 2,
            Name = "Berlin",
            IsSelected = false
        }
    };

        var cityRepository = new Mock<ICityRepository>();

        cityRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(cities);

        cityRepository
            .Setup(r => r.DeleteAsync(It.IsAny<City>()))
            .Callback<City>(city => cities.Remove(city))
            .Returns(Task.CompletedTask);

        var service = new CityService(
            cityRepository.Object,
            Mock.Of<IOpenMeteoClient>()
        );

        var result = await service.DeleteAsync(1);

        Assert.True(result);
        Assert.Single(cities);
        Assert.Equal("Berlin", cities[0].Name);
        Assert.True(cities[0].IsSelected);

        cityRepository.Verify(
            r => r.DeleteAsync(It.Is<City>(c => c.Id == 1)),
            Times.Once);

        cityRepository.Verify(
            r => r.SaveChangesAsync(),
            Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_WhenCityDoesNotExist_ReturnsFalse()
    {
        var cityRepository = new Mock<ICityRepository>();

        cityRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<City>
            {
            new City
            {
                Id = 1,
                Name = "Heidelberg",
                IsSelected = true
            }
            });

        var service = new CityService(
            cityRepository.Object,
            Mock.Of<IOpenMeteoClient>()
        );

        var result = await service.DeleteAsync(999);

        Assert.False(result);

        cityRepository.Verify(
            r => r.DeleteAsync(It.IsAny<City>()),
            Times.Never);

        cityRepository.Verify(
            r => r.SaveChangesAsync(),
            Times.Never);
    }


    [Fact]
    public async Task AddByLocationId_WhenLocationExists_AddsCorrectCity()
    {
        var cityRepository = new Mock<ICityRepository>();
        var openMeteoClient = new Mock<IOpenMeteoClient>();

        cityRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<City>());

        var location = new GeoLocation(
            2925533,
            "Frankfurt am Main",
            "Germany",
            "Hesse",
            50.11552,
            8.68417
        );

        openMeteoClient
            .Setup(c => c.GetLocationByIdAsync(2925533))
            .ReturnsAsync(location);

        cityRepository
            .Setup(r => r.AddAsync(It.IsAny<City>()))
            .ReturnsAsync((City city) => city);

        var service = new CityService(
            cityRepository.Object,
            openMeteoClient.Object
        );

        var result = await service.AddByLocationIdAsync(2925533);

        Assert.Equal(2925533, result.LocationId);
        Assert.Equal("Frankfurt am Main", result.Name);
        Assert.Equal(50.11552, result.Latitude);
        Assert.Equal(8.68417, result.Longitude);
        Assert.True(result.IsSelected);

        openMeteoClient.Verify(
            c => c.GetLocationByIdAsync(2925533),
            Times.Once);

        cityRepository.Verify(
            r => r.AddAsync(It.Is<City>(c =>
                c.LocationId == 2925533 &&
                c.Name == "Frankfurt am Main")),
            Times.Once);
    }


    [Fact]
    public async Task AddByLocationId_WhenLocationAlreadyExists_ThrowsException()
    {
        var cityRepository = new Mock<ICityRepository>();
        var openMeteoClient = new Mock<IOpenMeteoClient>();

        cityRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<City>
            {
            new City
            {
                Id = 1,
                LocationId = 2925533,
                Name = "Frankfurt am Main",
                IsSelected = true
            }
            });

        var service = new CityService(
            cityRepository.Object,
            openMeteoClient.Object
        );

        await Assert.ThrowsAsync<CityAlreadyExistsException>(
            () => service.AddByLocationIdAsync(2925533)
        );

        cityRepository.Verify(
            r => r.AddAsync(It.IsAny<City>()),
            Times.Never);

        openMeteoClient.Verify(
            c => c.GetLocationByIdAsync(It.IsAny<int>()),
            Times.Never);
    }


    [Fact]
    public async Task AddByLocationId_WhenLocationNotFound_ThrowsCityNotFoundException()
    {
        var cityRepository = new Mock<ICityRepository>();
        var openMeteoClient = new Mock<IOpenMeteoClient>();

        cityRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<City>());

        openMeteoClient
            .Setup(c => c.GetLocationByIdAsync(99999999))
            .ReturnsAsync((GeoLocation?)null);

        var service = new CityService(
            cityRepository.Object,
            openMeteoClient.Object
        );

        await Assert.ThrowsAsync<CityNotFoundException>(
            () => service.AddByLocationIdAsync(99999999)
        );

        cityRepository.Verify(
            r => r.AddAsync(It.IsAny<City>()),
            Times.Never);

        openMeteoClient.Verify(
            c => c.GetLocationByIdAsync(99999999),
            Times.Once);
    }


    [Fact]
    public async Task AddByLocationId_WhenCityAlreadySelected_SelectsNewCity()
    {
        var existingCity = new City
        {
            Id = 1,
            LocationId = 2910554,
            Name = "Heidelberg",
            IsSelected = true
        };

        var cityRepository = new Mock<ICityRepository>();

        cityRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<City> { existingCity });

        cityRepository
            .Setup(r => r.AddAsync(It.IsAny<City>()))
            .ReturnsAsync((City city) => city);

        var openMeteoClient = new Mock<IOpenMeteoClient>();

        openMeteoClient
            .Setup(c => c.GetLocationByIdAsync(2925533))
            .ReturnsAsync(new GeoLocation(
                2925533,
                "Frankfurt am Main",
                "Germany",
                "Hesse",
                50.11552,
                8.68417
            ));

        var service = new CityService(
            cityRepository.Object,
            openMeteoClient.Object
        );

        var result = await service.AddByLocationIdAsync(2925533);

        Assert.True(result.IsSelected);
        Assert.False(existingCity.IsSelected);

        cityRepository.Verify(
            r => r.AddAsync(It.IsAny<City>()),
            Times.Once
        );
    }

}
