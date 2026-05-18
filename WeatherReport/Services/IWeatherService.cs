using WeatherReport.Models;

namespace WeatherReport.Services;

public interface IWeatherService
{
    Task<WeatherInfo> GetWeatherAsync(LocationInfo location, CancellationToken cancellationToken = default);
}
