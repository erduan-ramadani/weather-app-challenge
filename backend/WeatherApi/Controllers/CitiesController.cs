
using Microsoft.AspNetCore.Mvc;
using WeatherApi.DTOs;
using WeatherApi.Models;
using WeatherApi.Services;

namespace WeatherApi.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly CityService _cityService;

    public CitiesController(CityService cityService)
    {
        _cityService = cityService;
    }


    [HttpGet]
    public async Task<ActionResult<List<CityResponse>>> GetAll()
    {
        var cities = await _cityService.GetAllAsync();

        var response = cities
            .Select(CityResponse.From)
            .ToList();

        return Ok(response);
    }



    [HttpPost]
    public async Task<IActionResult> Add(
        [FromBody] CreateCityRequest request)
    {
        var city = await _cityService.AddByLocationIdAsync(
            request.LocationId);

        return StatusCode(
            StatusCodes.Status201Created,
            CityResponse.From(city)
        );
    }


    [HttpPut("{id}/select")]
    public async Task<IActionResult> Select(int id)
    {
        var success = await _cityService.SelectAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _cityService.DeleteAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }


    [HttpGet("search")]
    public async Task<ActionResult<List<GeoLocation>>> Search(
        [FromQuery] string? name)
    {
        var locations = await _cityService.SearchLocationsAsync(
            name ?? string.Empty
        );

        return Ok(locations);
    }

}
