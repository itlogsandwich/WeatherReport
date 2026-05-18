using WeatherReport.Models;

namespace WeatherReport.Services;

public interface ISavedLocationsService
{
    IReadOnlyList<LocationInfo> All { get; }
    LocationInfo Home { get; }

    event EventHandler? Changed;

    void Add(LocationInfo location);
    void Remove(LocationInfo location);
    void SetHome(LocationInfo location);
}
