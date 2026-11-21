using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WeatherApi.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
     
        var (status, title, detail) = exception switch
        {
            InvalidLocationIdException => (
                400, "Bad Request", exception.Message),

            CityNotFoundException => (
                404, "Not Found", exception.Message),

            NoSelectedCityException => (
                404, "Not Found", exception.Message),

            CityAlreadyExistsException => (
                409, "Conflict", exception.Message),

            InvalidCityNameException => (
                400, "Bad Request", exception.Message),

            HttpRequestException => (
                502, "Bad Gateway", "Weather provider unavailable."),

            TaskCanceledException => (
                504, "Gateway Timeout", "Weather provider timed out."),

            WeatherProviderResponseException => (
                502,
                "Bad Gateway",
                "Weather provider returned invalid data."),

            _ => (
                500, "Internal Server Error", "An unexpected error occurred.")
        };

        if (status >= 500)
        {
            _logger.LogError(exception, "Request failed.");
        }

        context.Response.StatusCode = status;

        await context.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            },
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken
        );

        return true;

    }
}
