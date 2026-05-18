using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherReport.Models;
using WeatherReport.Services;

namespace WeatherReport.ViewModels;

public partial class WeatherViewModel : ObservableObject
{
    private readonly IWeatherService _weather;
    private readonly ISavedLocationsService _locations;
    private readonly SettingsService _settings;

    private WeatherInfo? _current;

    [ObservableProperty] private string  _city = string.Empty;
    [ObservableProperty] private string  _country = "Philippines";
    [ObservableProperty] private string  _localTime = string.Empty;
    [ObservableProperty] private string  _tempDisplay = "—";
    [ObservableProperty] private string  _hiLoFeelsDisplay = string.Empty;
    [ObservableProperty] private string  _condition = string.Empty;
    [ObservableProperty] private string  _icon = string.Empty;

    [ObservableProperty] private string  _humidityDisplay = "—";
    [ObservableProperty] private string  _dewPointDisplay = string.Empty;
    [ObservableProperty] private string  _windDisplay = "—";
    [ObservableProperty] private string  _windDetailDisplay = string.Empty;
    [ObservableProperty] private string  _uvDisplay = "—";
    [ObservableProperty] private string  _uvAdviceDisplay = string.Empty;

    [ObservableProperty] private bool    _isLoading;
    [ObservableProperty] private string? _errorMessage;

    public ObservableCollection<ForecastDayViewModel> Forecast { get; } = new();

    public WeatherViewModel(IWeatherService weather, ISavedLocationsService locations, SettingsService settings)
    {
        _weather   = weather;
        _locations = locations;
        _settings  = settings;

        _settings.UnitChanged    += (_, _) => RefreshDisplays();
        _locations.Changed       += (_, _) => _ = LoadAsync();
    }

    [RelayCommand]
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (IsLoading) return;

        IsLoading    = true;
        ErrorMessage = null;
        try
        {
            _current = await _weather.GetWeatherAsync(_locations.Home, cancellationToken);
            RebuildForecast();
            RefreshDisplays();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Couldn't load weather: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void RebuildForecast()
    {
        Forecast.Clear();
        if (_current is null) return;

        foreach (var day in _current.Forecast)
            Forecast.Add(new ForecastDayViewModel(day, _settings));
    }

    private void RefreshDisplays()
    {
        if (_current is null) return;

        City      = _current.City;
        Country   = _current.Country;
        LocalTime = _current.LocalTime;
        Condition = _current.Condition;
        Icon      = _current.Icon;

        TempDisplay      = _settings.FormatTemp(_current.TempC);
        HiLoFeelsDisplay = $"H: {_settings.FormatTemp(_current.HighC)}   " +
                           $"L: {_settings.FormatTemp(_current.LowC)}  •  " +
                           $"Feels like {_settings.FormatTemp(_current.FeelsLikeC)}";

        HumidityDisplay   = $"{_current.Humidity}%";
        DewPointDisplay   = $"Dew pt {_settings.FormatTemp(_current.DewPointC)}";
        WindDisplay       = $"{_current.WindKph:0} kph";
        WindDetailDisplay = $"{_current.WindDir} gusts {_current.WindGustKph:0}";
        UvDisplay         = $"{_current.UvIndex} {_current.UvLabel}";
        UvAdviceDisplay   = UvAdvice(_current.UvIndex);

        foreach (var item in Forecast)
            item.RefreshFormats();
    }

    private static string UvAdvice(int uv) => uv switch
    {
        <  3 => "Safe outdoors",
        <  6 => "Wear sunscreen",
        <  8 => "Use SPF 30+",
        < 11 => "Use SPF 50+",
        _    => "Avoid sun exposure",
    };
}
