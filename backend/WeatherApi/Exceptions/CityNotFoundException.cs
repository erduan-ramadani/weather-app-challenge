namespace WeatherApi.Exceptions;

public sealed class CityNotFoundException : Exception
{
    public CityNotFoundException()
        : base("City not found.") { }
}
