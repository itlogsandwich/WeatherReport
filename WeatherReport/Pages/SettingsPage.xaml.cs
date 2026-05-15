namespace WeatherReport.Pages;

using WeatherReport.Services;
public partial class SettingsPage : ContentPage
{
    // Cebu City reference temp shown in the preview label
    private const double PreviewTempC = 33.0;

    private SettingsService Settings => SettingsService.Instance;

    // ─── Constructor ──────────────────────────────────────────────────────────
    public SettingsPage()
    {
        InitializeComponent();
        Settings.ThemeChanged += OnThemeChanged;
    }

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Always reflect the persisted setting when the page is shown
        ApplyToggleState(Settings.UseCelsius);
        ApplyAppearanceState(Settings.UseDarkMode);
    }

    // ─── Toggle handlers ──────────────────────────────────────────────────────

    private void OnCelsiusTapped(object? sender, TappedEventArgs e)
    {
        if (Settings.UseCelsius) return;    // already selected
        Settings.SetUnit(true);
        ApplyToggleState(true);
    }

    private void OnFahrenheitTapped(object? sender, TappedEventArgs e)
    {
        if (!Settings.UseCelsius) return;   // already selected
        Settings.SetUnit(false);
        ApplyToggleState(false);
    }

    private void OnAppearanceToggled(object? sender, ToggledEventArgs e)
    {
        Settings.SetTheme(e.Value);
        ApplyAppearanceState(e.Value);
        ApplyToggleState(Settings.UseCelsius);
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        ApplyAppearanceState(Settings.UseDarkMode);
        ApplyToggleState(Settings.UseCelsius);
    }

    // ─── UI state ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Updates the visual state of the segmented toggle and the preview label
    /// to reflect <paramref name="useCelsius"/>.
    /// </summary>
    private void ApplyToggleState(bool useCelsius)
    {
        // Active pill: filled primary background + light text
        // Inactive pill: transparent background + muted text
        var activeBackground = (Color)Application.Current!.Resources["Primary"];
        var activeText = (Color)Application.Current!.Resources["OnPrimary"];
        var inactiveText = (Color)Application.Current!.Resources["OnSurfaceVariant"];

        if (useCelsius)
        {
            // Celsius active
            CelsiusBorder.BackgroundColor = activeBackground;
            CelsiusLabel.TextColor = activeText;
            FahrenheitBorder.BackgroundColor = Colors.Transparent;
            FahrenheitLabel.TextColor = inactiveText;
        }
        else
        {
            // Fahrenheit active
            FahrenheitBorder.BackgroundColor = activeBackground;
            FahrenheitLabel.TextColor = activeText;
            CelsiusBorder.BackgroundColor = Colors.Transparent;
            CelsiusLabel.TextColor = inactiveText;
        }

        // Live preview with Cebu's current temperature
        PreviewLabel.Text = Settings.FormatTemp(PreviewTempC);
    }

    private void ApplyAppearanceState(bool useDarkMode)
    {
        AppearanceSwitch.Toggled -= OnAppearanceToggled;
        AppearanceSwitch.IsToggled = useDarkMode;
        AppearanceSwitch.Toggled += OnAppearanceToggled;

        AppearanceLabel.Text = useDarkMode ? "Dark mode" : "Light mode";
    }
}
