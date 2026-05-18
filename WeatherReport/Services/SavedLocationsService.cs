using System.Text.Json;
using WeatherReport.Models;

namespace WeatherReport.Services;

public class SavedLocationsService : ISavedLocationsService
{
    private const string SavedKey = "pref_saved_locations";
    private const string HomeKey  = "pref_home_city";

    private readonly List<LocationInfo> _saved = new();
    private LocationInfo _home = PhilippineCities.CebuCity;

    public SavedLocationsService()
    {
        Load();
    }

    public IReadOnlyList<LocationInfo> All => _saved;
    public LocationInfo Home => _home;

    public event EventHandler? Changed;

    public void Add(LocationInfo location)
    {
        if (_saved.Any(l => l.City.Equals(location.City, StringComparison.OrdinalIgnoreCase)))
            return;

        _saved.Add(location);
        Persist();
        RaiseChanged();
    }

    public void Remove(LocationInfo location)
    {
        var existing = _saved.FirstOrDefault(l => l.City.Equals(location.City, StringComparison.OrdinalIgnoreCase));
        if (existing is null) return;

        _saved.Remove(existing);
        // Don't allow removing the home city — promote a different one if it was home
        if (_home.City.Equals(existing.City, StringComparison.OrdinalIgnoreCase))
            _home = _saved.FirstOrDefault() ?? PhilippineCities.CebuCity;

        Persist();
        RaiseChanged();
    }

    public void SetHome(LocationInfo location)
    {
        if (_home.City.Equals(location.City, StringComparison.OrdinalIgnoreCase))
            return;

        // Ensure home is part of the saved list
        if (!_saved.Any(l => l.City.Equals(location.City, StringComparison.OrdinalIgnoreCase)))
            _saved.Insert(0, location);

        _home = location;
        Persist();
        RaiseChanged();
    }

    private void Load()
    {
        var json = Preferences.Get(SavedKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(json))
        {
            try
            {
                var stored = JsonSerializer.Deserialize<List<LocationInfo>>(json);
                if (stored is not null && stored.Count > 0)
                    _saved.AddRange(stored);
            }
            catch (JsonException)
            {
                // Corrupt preference — fall back to seed.
            }
        }

        if (_saved.Count == 0)
            _saved.AddRange(SeedDefaults());

        var homeName = Preferences.Get(HomeKey, PhilippineCities.CebuCity.City);
        _home = _saved.FirstOrDefault(l => l.City.Equals(homeName, StringComparison.OrdinalIgnoreCase))
                ?? _saved.First();
    }

    private void Persist()
    {
        Preferences.Set(SavedKey, JsonSerializer.Serialize(_saved));
        Preferences.Set(HomeKey, _home.City);
    }

    private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);

    private static IEnumerable<LocationInfo> SeedDefaults() => new[]
    {
        PhilippineCities.CebuCity,
        PhilippineCities.All.First(c => c.City == "Manila"),
        PhilippineCities.All.First(c => c.City == "Davao City"),
        PhilippineCities.All.First(c => c.City == "Baguio"),
    };
}
