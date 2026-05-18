using CommunityToolkit.Mvvm.ComponentModel;
using WeatherReport.Models;
using WeatherReport.Services;

namespace WeatherReport.ViewModels;

public partial class ForecastDayViewModel : ObservableObject
{
    private readonly SettingsService _settings;
    private readonly ForecastDay _model;

    [ObservableProperty] private string _dayName = string.Empty;
    [ObservableProperty] private string _icon = string.Empty;
    [ObservableProperty] private string _highDisplay = string.Empty;
    [ObservableProperty] private string _lowDisplay = string.Empty;

    public ForecastDayViewModel(ForecastDay model, SettingsService settings)
    {
        _model = model;
        _settings = settings;

        DayName = model.DayName;
        Icon    = model.Icon;
        RefreshFormats();
    }

    public void RefreshFormats()
    {
        HighDisplay = _settings.FormatTemp(_model.HighC);
        LowDisplay  = _settings.FormatTemp(_model.LowC);
    }
}
