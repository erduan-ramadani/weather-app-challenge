using WeatherApi.Models;

namespace WeatherApi.Repositories;

public interface IWeatherRepository
{
    Task<WeatherCache?> GetAsync(int cityId);

    Task SaveAsync(WeatherCache cache);
}
