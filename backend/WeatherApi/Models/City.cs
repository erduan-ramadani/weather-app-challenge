namespace WeatherApi.Models;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSelected { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int LocationId { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? Region { get; set; }
}
