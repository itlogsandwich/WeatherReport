using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherReport.Services;

namespace WeatherReport.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private const double PreviewTempC = 33.0;

    private readonly SettingsService _settings;

    [ObservableProperty] private bool _isCelsius;
    [ObservableProperty] private bool _isDarkMode;
    [ObservableProperty] private string _previewDisplay = string.Empty;
    [ObservableProperty] private string _appearanceLabel = string.Empty;

    public SettingsViewModel(SettingsService settings)
    {
        _settings = settings;

        IsCelsius  = settings.UseCelsius;
        IsDarkMode = settings.UseDarkMode;
        RefreshPreviews();

        settings.UnitChanged  += (_, _) => { IsCelsius  = settings.UseCelsius;  RefreshPreviews(); };
        settings.ThemeChanged += (_, _) => { IsDarkMode = settings.UseDarkMode; RefreshPreviews(); };
    }

    [RelayCommand] private void SetCelsius()     => _settings.SetUnit(true);
    [RelayCommand] private void SetFahrenheit()  => _settings.SetUnit(false);

    partial void OnIsDarkModeChanged(bool value)
    {
        if (_settings.UseDarkMode != value)
            _settings.SetTheme(value);
        RefreshPreviews();
    }

    private void RefreshPreviews()
    {
        PreviewDisplay  = _settings.FormatTemp(PreviewTempC);
        AppearanceLabel = IsDarkMode ? "Dark mode" : "Light mode";
    }
}
