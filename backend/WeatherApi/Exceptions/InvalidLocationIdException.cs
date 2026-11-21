namespace WeatherApi.Exceptions;

public sealed class InvalidLocationIdException : Exception
{
    public InvalidLocationIdException()
        : base("Invalid location ID.") { }
}
