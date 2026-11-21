using WeatherApi.Models;

namespace WeatherApi.Repositories;

public interface ICityRepository
{
    Task<List<City>> GetAllAsync();

    Task<City> AddAsync(City city);

    Task DeleteAsync(City city);

    Task SaveChangesAsync();
}
