namespace WeatherApi.Exceptions;

public sealed class WeatherProviderResponseException : Exception
{
    public WeatherProviderResponseException(Exception innerException)
        : base("Weather provider returned invalid data.", innerException)
    {
    }
}
