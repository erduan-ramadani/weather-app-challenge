using WeatherApi.Models;

namespace WeatherApi.Clients;

public interface IOpenMeteoClient
{
   
    Task<WeatherSnapshot> GetWeatherAsync(
        double latitude,
        double longitude);

    Task<List<GeoLocation>> SearchLocationsAsync(string name);
    Task<GeoLocation?> GetLocationByIdAsync(int locationId);
}
