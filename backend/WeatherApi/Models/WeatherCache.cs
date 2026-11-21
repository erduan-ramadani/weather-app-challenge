using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Models;

public class WeatherCache
{
    [Key]
    public int CityId { get; set; }

    public double Temperature { get; set; }
    public int? RainProbability { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public DateTimeOffset LastUpdatedUtc { get; set; }
}
