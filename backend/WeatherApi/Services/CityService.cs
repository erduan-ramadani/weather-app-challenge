
using WeatherApi.Clients;
using WeatherApi.Exceptions;
using WeatherApi.Models;
using WeatherApi.Repositories;

namespace WeatherApi.Services;


public class CityService
{
    private readonly ICityRepository _repository;
    private readonly IOpenMeteoClient _openMeteoClient;

    public CityService(
    ICityRepository repository,
    IOpenMeteoClient openMeteoClient)
    {
        _repository = repository;
        _openMeteoClient = openMeteoClient;
    }

    public Task<List<City>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<bool> SelectAsync(int id)
    {
        var cities = await _repository.GetAllAsync();

        var selectedCity = cities.FirstOrDefault(c => c.Id == id);

        if (selectedCity == null)
        {
            return false;
        }

        foreach (var city in cities)
        {
            city.IsSelected = city.Id == id;
        }

        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cities = await _repository.GetAllAsync();

        var city = cities.FirstOrDefault(c => c.Id == id);

        if (city == null)
        {
            return false;
        }

        if (city.IsSelected)
        {
            var nextCity = cities.FirstOrDefault(c => c.Id != id);

            if (nextCity != null)
            {
                nextCity.IsSelected = true;
            }
        }

        await _repository.DeleteAsync(city);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<List<GeoLocation>> SearchLocationsAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidCityNameException();
        }

        return await _openMeteoClient.SearchLocationsAsync(name.Trim());
    }

    public async Task<City> AddByLocationIdAsync(int locationId)
    {
        if (locationId <= 0)
        {
            throw new InvalidLocationIdException();
        }

        var cities = await _repository.GetAllAsync();

        if (cities.Any(c => c.LocationId == locationId))
        {
            throw new CityAlreadyExistsException();
        }

        var location = await _openMeteoClient
            .GetLocationByIdAsync(locationId);

        if (location == null)
        {
            throw new CityNotFoundException();
        }


        foreach (var existingCity in cities)
        {
            existingCity.IsSelected = false;
        }

        var city = new City
        {
            LocationId = location.Id,
            Name = location.Name,
            Country = location.Country,
            Region = location.Region,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            IsSelected = true
        };

        return await _repository.AddAsync(city);

    }


}
