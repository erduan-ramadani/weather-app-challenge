
using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using WeatherApi.Models;
namespace WeatherApi.Repositories;

public class WeatherRepository : IWeatherRepository
{
    private readonly AppDbContext _context;

    public WeatherRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WeatherCache?> GetAsync(int cityId)
    {
        return await _context.WeatherCaches
            .FirstOrDefaultAsync(w => w.CityId == cityId);
    }

    public async Task SaveAsync(WeatherCache cache)
    {
        var existing = await GetAsync(cache.CityId);

        if (existing == null)
        {
            _context.WeatherCaches.Add(cache);
        }
        else
        {
            existing.Temperature = cache.Temperature;
            existing.RainProbability = cache.RainProbability;
            existing.Humidity = cache.Humidity;
            existing.WindSpeed = cache.WindSpeed;
            existing.LastUpdatedUtc = cache.LastUpdatedUtc;
        }

        await _context.SaveChangesAsync();
    }
}
