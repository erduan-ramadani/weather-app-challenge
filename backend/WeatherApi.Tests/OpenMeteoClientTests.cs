using System.Net;
using Moq;
using Moq.Protected;
using WeatherApi.Clients;
using WeatherApi.Exceptions;

namespace WeatherApi.Tests;

public class OpenMeteoClientTests
{
    [Fact]
    public async Task GetWeatherAsync_WhenRequiredFieldMissing_ThrowsProviderException()
    {
        var handler = new Mock<HttpMessageHandler>();

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    """
                    {
                      "current": {
                        "temperature_2m": 12.5
                      },
                      "hourly": {
                        "time": [],
                        "precipitation_probability": []
                      }
                    }
                    """)
            });

        using var httpClient = new HttpClient(handler.Object);
        var client = new OpenMeteoClient(httpClient);

        await Assert.ThrowsAsync<WeatherProviderResponseException>(
            () => client.GetWeatherAsync(49.4, 8.7));
    }


    [Fact]
    public async Task SearchLocationsAsync_WhenLocationNameIsEmpty_ThrowsProviderException()
    {
        var handler = new Mock<HttpMessageHandler>();

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    """
                {
                    "results": [
                        {
                            "id": 2925533,
                            "name": "",
                            "country": "Germany",
                            "latitude": 50.11552,
                            "longitude": 8.68417
                        }
                    ]
                }
                """)
            });

        using var httpClient = new HttpClient(handler.Object);
        var client = new OpenMeteoClient(httpClient);

        await Assert.ThrowsAsync<WeatherProviderResponseException>(
            () => client.SearchLocationsAsync("Frankfurt"));

        handler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }


    [Fact]
    public async Task GetWeatherAsync_WhenHumidityIsInvalid_ThrowsProviderException()
    {
        var handler = new Mock<HttpMessageHandler>();

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    """
                {
                  "current": {
                    "temperature_2m": 15,
                    "relative_humidity_2m": 150,
                    "wind_speed_10m": 10,
                    "time": "2026-11-17T10:00"
                  },
                  "hourly": {
                    "time": ["2026-11-17T10:00"],
                    "precipitation_probability": [30]
                  }
                }
                """)
            });

        using var httpClient = new HttpClient(handler.Object);
        var client = new OpenMeteoClient(httpClient);

        await Assert.ThrowsAsync<WeatherProviderResponseException>(
            () => client.GetWeatherAsync(49.4, 8.7));
    }


    [Fact]
    public async Task GetLocationByIdAsync_WhenLocationExists_ReturnsCorrectLocation()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    """
                {
                    "id": 2925533,
                    "name": "Frankfurt am Main",
                    "country": "Germany",
                    "admin1": "Hesse",
                    "latitude": 50.11552,
                    "longitude": 8.68417
                }
                """)
            });

        using var httpClient = new HttpClient(handler.Object);
        var client = new OpenMeteoClient(httpClient);

        // Act
        var result = await client.GetLocationByIdAsync(2925533);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2925533, result.Id);
        Assert.Equal("Frankfurt am Main", result.Name);
        Assert.Equal("Germany", result.Country);
        Assert.Equal("Hesse", result.Region);
        Assert.Equal(50.11552, result.Latitude);
        Assert.Equal(8.68417, result.Longitude);

        handler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(request =>
                request.Method == HttpMethod.Get &&
                request.RequestUri != null &&
                request.RequestUri.AbsolutePath == "/v1/get" &&
                request.RequestUri.Query == "?id=2925533"),
            ItExpr.IsAny<CancellationToken>());
    }

}
