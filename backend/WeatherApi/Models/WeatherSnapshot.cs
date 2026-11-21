namespace WeatherApi.Models;

public record WeatherSnapshot(
    double Temperature,
    int? RainProbability,
    int Humidity,
    double WindSpeed,
    DateTimeOffset LastUpdatedUtc
);
