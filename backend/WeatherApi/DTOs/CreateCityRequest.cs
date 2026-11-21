using System.ComponentModel.DataAnnotations;

namespace WeatherApi.DTOs;

public class CreateCityRequest
{
    [Range(1, int.MaxValue)]
    public int LocationId { get; set; }
}
