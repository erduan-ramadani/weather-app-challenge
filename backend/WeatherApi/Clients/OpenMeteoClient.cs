using System.Globalization;
using System.Net;
using System.Text.Json;
using WeatherApi.Exceptions;
using WeatherApi.Models;

namespace WeatherApi.Clients;

public class OpenMeteoClient : IOpenMeteoClient
{
    private readonly HttpClient _httpClient;

    public OpenMeteoClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherSnapshot> GetWeatherAsync(
        double latitude,
        double longitude)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);

        var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}"
            + "&current=temperature_2m,relative_humidity_2m,wind_speed_10m"
            + "&hourly=precipitation_probability"
            + "&forecast_days=1&timezone=auto";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        try
        {
            using var json = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync());

            var root = json.RootElement;
            var current = root.GetProperty("current");
            var hourly = root.GetProperty("hourly");

            var temperature = current.GetProperty("temperature_2m").GetDouble();
            var humidity = current.GetProperty("relative_humidity_2m").GetInt32();
            var windSpeed = current.GetProperty("wind_speed_10m").GetDouble();

            var currentTime = ParseProviderTime(
                current.GetProperty("time").GetString());

            var times = hourly.GetProperty("time");
            var probabilities = hourly.GetProperty("precipitation_probability");

            if (times.GetArrayLength() != probabilities.GetArrayLength())
            {
                throw new InvalidOperationException("Mismatched hourly weather data.");
            }

            int? rainProbability = null;

            for (int i = 0; i < times.GetArrayLength(); i++)
            {
                var hour = ParseProviderTime(times[i].GetString());

                if (hour.Date == currentTime.Date &&
                    hour.Hour == currentTime.Hour)
                {
                    if (probabilities[i].ValueKind == JsonValueKind.Number)
                    {
                        rainProbability = probabilities[i].GetInt32();
                    }

                    break;
                }
            }

            if (!double.IsFinite(temperature) ||
                !double.IsFinite(windSpeed) ||
                windSpeed < 0 ||
                humidity is < 0 or > 100 ||
                rainProbability is < 0 or > 100)
            {
                throw new InvalidOperationException("Invalid weather data.");
            }

            return new WeatherSnapshot(
                temperature,
                rainProbability,
                humidity,
                windSpeed,
                DateTimeOffset.UtcNow
            );
        }
        catch (Exception ex) when (
            ex is JsonException
            or KeyNotFoundException
            or InvalidOperationException
            or FormatException
            or ArgumentOutOfRangeException
            or IndexOutOfRangeException)
        {
            throw new WeatherProviderResponseException(ex);
        }
    }

    public async Task<List<GeoLocation>> SearchLocationsAsync(string name)
    {
        var url = "https://geocoding-api.open-meteo.com/v1/search"
            + $"?name={Uri.EscapeDataString(name)}&count=10&language=de";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        try
        {
            using var json = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync());

            var locations = new List<GeoLocation>();

            if (!json.RootElement.TryGetProperty("results", out var results))
            {
                return locations;
            }

            foreach (var item in results.EnumerateArray())
            {
                locations.Add(ParseLocation(item));
            }

            return locations;
        }
        catch (Exception ex) when (
            ex is JsonException
            or KeyNotFoundException
            or InvalidOperationException
            or FormatException)
        {
            throw new WeatherProviderResponseException(ex);
        }
    }

    public async Task<GeoLocation?> GetLocationByIdAsync(int locationId)
    {
        var url = $"https://geocoding-api.open-meteo.com/v1/get?id={locationId}";

        using var response = await _httpClient.GetAsync(url);

        if (response.StatusCode is
            HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        try
        {
            using var json = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync());

            var item = json.RootElement;

            if (item.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return ParseLocation(item, locationId);
        }
        catch (Exception ex) when (
            ex is JsonException
            or KeyNotFoundException
            or InvalidOperationException
            or FormatException)
        {
            throw new WeatherProviderResponseException(ex);
        }
    }

    private static DateTime ParseProviderTime(string? value)
    {
        if (!DateTime.TryParseExact(
                value,
                ["yyyy-MM-dd'T'HH:mm", "yyyy-MM-dd'T'HH:mm:ss"],
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var time))
        {
            throw new InvalidOperationException("Invalid weather time.");
        }

        return time;
    }

    private static GeoLocation ParseLocation(
        JsonElement item,
        int? expectedId = null)
    {
        var id = item.GetProperty("id").GetInt32();
        var name = item.GetProperty("name").GetString();
        var country = item.GetProperty("country").GetString();
        var latitude = item.GetProperty("latitude").GetDouble();
        var longitude = item.GetProperty("longitude").GetDouble();

        if (id <= 0 ||
            (expectedId.HasValue && id != expectedId.Value) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(country) ||
            !double.IsFinite(latitude) ||
            !double.IsFinite(longitude) ||
            latitude is < -90 or > 90 ||
            longitude is < -180 or > 180)
        {
            throw new InvalidOperationException("Invalid location data.");
        }

        var region = item.TryGetProperty("admin1", out var admin1)
            ? admin1.GetString()
            : null;

        return new GeoLocation(
            id,
            name,
            country,
            region,
            latitude,
            longitude
        );
    }
}
