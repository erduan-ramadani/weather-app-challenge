using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using WeatherApi.Models;
using WeatherApi.Repositories;

namespace WeatherApi.Tests;

public class CityRepositoryTests
{
    [Fact]
    public async Task DeleteAsync_WhenCityHasWeatherCache_RemovesBoth()
    {

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var city = new City
        {
            Id = 1,
            Name = "Heidelberg",
            LocationId = 2910554,
            IsSelected = true
        };

        var cache = new WeatherCache
        {
            CityId = 1,
            Temperature = 15,
            RainProbability = 20,
            Humidity = 70,
            WindSpeed = 10,
            LastUpdatedUtc = DateTimeOffset.UtcNow
        };

        context.Cities.Add(city);
        context.WeatherCaches.Add(cache);
        await context.SaveChangesAsync();

        var repository = new CityRepository(context);


        await repository.DeleteAsync(city);

        Assert.Equal(EntityState.Deleted, context.Entry(city).State);
        Assert.Equal(EntityState.Deleted, context.Entry(cache).State);

        await repository.SaveChangesAsync();


        Assert.False(
            await context.Cities.AnyAsync(c => c.Id == 1));

        Assert.False(
            await context.WeatherCaches.AnyAsync(w => w.CityId == 1));
    }
}
