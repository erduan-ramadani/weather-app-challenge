using WeatherApi.Models;

namespace WeatherApi.DTOs;

public record CityResponse(
    int Id,
    int LocationId,
    string Name,
    string Country,
    string? Region,
    double Latitude,
    double Longitude,
    bool IsSelected
)
{
    public static CityResponse From(City city) => new(
        city.Id,
        city.LocationId,
        city.Name,
        city.Country,
        city.Region,
        city.Latitude,
        city.Longitude,
        city.IsSelected
    );
}
