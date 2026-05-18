using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherReport.Models;
using WeatherReport.Services;

namespace WeatherReport.ViewModels;

public partial class LocationsViewModel : ObservableObject
{
    private readonly IWeatherService _weather;
    private readonly ISavedLocationsService _locations;
    private readonly SettingsService _settings;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _errorMessage;

    public ObservableCollection<LocationCardViewModel> Cards { get; } = new();

    /// <summary>Cities in the catalog that aren't already saved — drives the picker dialog.</summary>
    public IReadOnlyList<LocationInfo> AvailableToAdd =>
        PhilippineCities.All
            .Where(c => !_locations.All.Any(s => s.City.Equals(c.City, StringComparison.OrdinalIgnoreCase)))
            .ToList();

    public LocationsViewModel(IWeatherService weather, ISavedLocationsService locations, SettingsService settings)
    {
        _weather   = weather;
        _locations = locations;
        _settings  = settings;

        _settings.UnitChanged += (_, _) => RefreshFormats();
        _locations.Changed    += async (_, _) => await RebuildAsync();
    }

    [RelayCommand]
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (IsLoading) return;
        await RebuildAsync(cancellationToken);
    }

    [RelayCommand]
    public void Remove(LocationCardViewModel card)
    {
        if (card.IsHome) return;   // home stays
        _locations.Remove(card.Location);
    }

    [RelayCommand]
    public void Add(LocationInfo location)
    {
        _locations.Add(location);
    }

    private async Task RebuildAsync(CancellationToken cancellationToken = default)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            // Rebuild the visible cards from the saved list, then fetch in parallel.
            Cards.Clear();
            var homeCity = _locations.Home.City;
            var cards = _locations.All
                .Select(l => new LocationCardViewModel(l, _settings, l.City == homeCity))
                .ToList();
            foreach (var card in cards)
                Cards.Add(card);

            var tasks = cards.Select(c => FetchAsync(c, cancellationToken)).ToList();
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Couldn't refresh locations: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task FetchAsync(LocationCardViewModel card, CancellationToken cancellationToken)
    {
        try
        {
            var info = await _weather.GetWeatherAsync(card.Location, cancellationToken);
            card.ApplyWeather(info);
        }
        catch
        {
            card.MarkError();
        }
    }

    private void RefreshFormats()
    {
        foreach (var card in Cards)
            card.RefreshFormats();
    }
}
