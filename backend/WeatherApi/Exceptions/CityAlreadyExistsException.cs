
namespace WeatherApi.Exceptions;

public class CityAlreadyExistsException : Exception
{
    public CityAlreadyExistsException()
        : base("City already exists.")
    {
    }
}
