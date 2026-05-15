using WeatherReport.Models;
using WeatherReport.Services;

namespace WeatherReport.Pages;

public partial class WeatherPage : ContentPage
{
    // ─── Cebu City weather data (Celsius) ─────────────────────────────────────
    private readonly WeatherInfo _weather = WeatherDataFactory.GetCebuCity();

    // Shorthand
    private SettingsService Settings => SettingsService.Instance;

    // ─── Constructor ──────────────────────────────────────────────────────────
    public WeatherPage()
    {
        InitializeComponent();

        // Subscribe so the UI updates immediately when the user changes the
        // unit on the Settings page (even without navigating away and back).
        Settings.UnitChanged += OnUnitChanged;
    }

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshTemperatureDisplays();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Keep subscription alive (page is cached by Shell), so no unsub here.
    }

    // ─── Event handlers ───────────────────────────────────────────────────────

    private void OnUnitChanged(object? sender, EventArgs e)
        => RefreshTemperatureDisplays();

    // ─── UI refresh ───────────────────────────────────────────────────────────

    private void RefreshTemperatureDisplays()
    {
        var svc = Settings;

        // Hero section
        TempLabel.Text = svc.FormatTemp(_weather.TempC);

        HiLoLabel.Text =
            $"H: {svc.FormatTemp(_weather.HighC)}   " +
            $"L: {svc.FormatTemp(_weather.LowC)}  •  " +
            $"Feels like {svc.FormatTemp(_weather.FeelsLikeC)}";

        // Dew point (metric card)
        DewPointLabel.Text = svc.UseCelsius
            ? $"Dew pt {_weather.DewPointC:0}°C"
            : $"Dew pt {CToF(_weather.DewPointC):0}°F";

        // Forecast rows
        UpdateForecastRow(Day0High, Day0Low, _weather.Forecast[0]);
        UpdateForecastRow(Day1High, Day1Low, _weather.Forecast[1]);
        UpdateForecastRow(Day2High, Day2Low, _weather.Forecast[2]);
        UpdateForecastRow(Day3High, Day3Low, _weather.Forecast[3]);
        UpdateForecastRow(Day4High, Day4Low, _weather.Forecast[4]);
    }

    private void UpdateForecastRow(Label highLabel, Label lowLabel, ForecastDay day)
    {
        highLabel.Text = Settings.FormatTemp(day.HighC);
        lowLabel.Text = Settings.FormatTemp(day.LowC);
    }

    // ─── Helper ───────────────────────────────────────────────────────────────

    private static double CToF(double c) => c * 9.0 / 5.0 + 32.0;
}
