namespace WeatherApi.Exceptions;

public sealed class NoSelectedCityException : Exception
{
    public NoSelectedCityException()
        : base("No city selected.")
    {
    }
}
