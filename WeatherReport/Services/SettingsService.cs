namespace WeatherReport.Services;

/// <summary>
/// Owns app-wide preferences: temperature unit (°C/°F) and theme. Persists via
/// <see cref="Preferences"/> and raises events when values change so the UI
/// can refresh.
/// </summary>
public class SettingsService
{
    private const string UseCelsiusKey  = "pref_use_celsius";
    private const string UseDarkModeKey = "pref_use_dark_mode";

    public event EventHandler? UnitChanged;
    public event EventHandler? ThemeChanged;

    public bool UseCelsius
    {
        get => Preferences.Get(UseCelsiusKey, true);
        private set => Preferences.Set(UseCelsiusKey, value);
    }

    public bool UseDarkMode
    {
        get => Preferences.Get(UseDarkModeKey, false);
        private set => Preferences.Set(UseDarkModeKey, value);
    }

    public string UnitLabel => UseCelsius ? "°C" : "°F";

    public void SetUnit(bool useCelsius)
    {
        if (UseCelsius == useCelsius) return;
        UseCelsius = useCelsius;
        RaiseUnitChanged();
    }

    public void SetTheme(bool useDarkMode)
    {
        if (UseDarkMode == useDarkMode)
        {
            ApplyTheme();
            return;
        }

        UseDarkMode = useDarkMode;
        ApplyTheme();
        RaiseThemeChanged();
    }

    public void ApplyTheme()
    {
        var app = Application.Current;
        if (app is null) return;

        app.UserAppTheme = UseDarkMode ? AppTheme.Dark : AppTheme.Light;
        ApplyThemeResources(app.Resources, UseDarkMode);
    }

    public string FormatTemp(double celsius)
    {
        if (UseCelsius)
            return $"{Math.Round(celsius):0}°C";

        var f = celsius * 9.0 / 5.0 + 32.0;
        return $"{Math.Round(f):0}°F";
    }

    public string FormatHiLo(double highC, double lowC) =>
        $"H: {FormatTemp(highC)}   L: {FormatTemp(lowC)}";

    private static void ApplyThemeResources(ResourceDictionary resources, bool useDarkMode)
    {
        resources["Primary"]               = Color.FromArgb(useDarkMode ? "#75B9D5" : "#29667E");
        resources["PrimaryDark"]           = Color.FromArgb(useDarkMode ? "#A8D7EA" : "#9BD4F0");
        resources["PrimaryDarkText"]       = Color.FromArgb(useDarkMode ? "#14303B" : "#153845");
        resources["Secondary"]             = Color.FromArgb(useDarkMode ? "#1E4D60" : "#D8EEF7");
        resources["SecondaryDarkText"]     = Color.FromArgb(useDarkMode ? "#9FCFE3" : "#195A72");
        resources["Background"]            = Color.FromArgb(useDarkMode ? "#0F1418" : "#F7F9FC");
        resources["Surface"]               = Color.FromArgb(useDarkMode ? "#182129" : "#FFFFFF");
        resources["SurfaceContainerLow"]   = Color.FromArgb(useDarkMode ? "#202B34" : "#F0F4F8");
        resources["OnSurface"]             = Color.FromArgb(useDarkMode ? "#D6E2E8" : "#2C3338");
        resources["OnSurfaceVariant"]      = Color.FromArgb(useDarkMode ? "#9EAFB8" : "#596065");
        resources["OnPrimary"]             = Color.FromArgb(useDarkMode ? "#0C2029" : "#FFFFFF");
        resources["PrimaryDim"]            = Color.FromArgb(useDarkMode ? "#8AC4DC" : "#195A72");
        resources["PrimaryContainer"]      = Color.FromArgb(useDarkMode ? "#193544" : "#DBEAFE");
        resources["SecondaryContainer"]    = Color.FromArgb(useDarkMode ? "#243C48" : "#E0F2FE");
        resources["Outline"]               = Color.FromArgb(useDarkMode ? "#78909B" : "#64748B");
        resources["OutlineVariant"]        = Color.FromArgb(useDarkMode ? "#33444E" : "#CBD5E1");
    }

    private void RaiseUnitChanged() =>
        MainThread.BeginInvokeOnMainThread(() => UnitChanged?.Invoke(this, EventArgs.Empty));

    private void RaiseThemeChanged() =>
        MainThread.BeginInvokeOnMainThread(() => ThemeChanged?.Invoke(this, EventArgs.Empty));
}
