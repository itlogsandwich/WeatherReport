using CommunityToolkit.Mvvm.ComponentModel;
using WeatherReport.Models;
using WeatherReport.Services;

namespace WeatherReport.ViewModels;

public partial class LocationCardViewModel : ObservableObject
{
    private readonly SettingsService _settings;
    private double _tempC;

    [ObservableProperty] private string _city = string.Empty;
    [ObservableProperty] private string _country = "Philippines";
    [ObservableProperty] private string _condition = "Loading…";
    [ObservableProperty] private string _icon = "⏳";
    [ObservableProperty] private string _tempDisplay = "—";
    [ObservableProperty] private string _localTime = string.Empty;
    [ObservableProperty] private bool _isHome;

    public LocationInfo Location { get; }

    public LocationCardViewModel(LocationInfo location, SettingsService settings, bool isHome)
    {
        Location = location;
        _settings = settings;

        City    = location.City;
        Country = location.Country;
        IsHome  = isHome;
    }

    public void ApplyWeather(WeatherInfo info)
    {
        _tempC    = info.TempC;
        Condition = info.Condition;
        Icon      = info.Icon;
        LocalTime = info.LocalTime;
        RefreshFormats();
    }

    public void MarkError()
    {
        Condition = "Unavailable";
        Icon      = "⚠️";
        TempDisplay = "—";
    }

    public void RefreshFormats()
    {
        TempDisplay = _settings.FormatTemp(_tempC);
    }
}
