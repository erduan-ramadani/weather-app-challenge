namespace WeatherApi.Exceptions;

public sealed class InvalidCityNameException : Exception
{
    public InvalidCityNameException()
        : base("City name is required.")
    {
    }
}
