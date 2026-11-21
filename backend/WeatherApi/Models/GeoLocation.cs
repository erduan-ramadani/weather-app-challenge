namespace WeatherApi.Models;

public record GeoLocation(
    int Id,
    string Name,
    string Country,
    string? Region,
    double Latitude,
    double Longitude
);
