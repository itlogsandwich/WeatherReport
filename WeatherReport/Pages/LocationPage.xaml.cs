namespace WeatherReport.Pages;

using System.Globalization;
using WeatherReport.Services;

public partial class LocationsPage : ContentPage
{
    private const string AddedLocationsKey = "pref_added_locations";

    // Temperature data for each saved location (Celsius)
    private readonly double _cebuC = 33;
    private readonly double _manilaC = 31;
    private readonly double _davaoC = 28;
    private readonly double _baguioC = 22;
    private readonly List<AddedLocation> _addedLocations = new();
    private readonly List<(Label Label, double TempC)> _addedTemperatureLabels = new();

    private SettingsService Settings => SettingsService.Instance;

    // ─── Constructor ──────────────────────────────────────────────────────────
    public LocationsPage()
    {
        InitializeComponent();
        LoadAddedLocations();
        RenderAddedLocations();
        Settings.UnitChanged += OnUnitChanged;
    }

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshTemperatures();
    }

    // ─── Event handlers ───────────────────────────────────────────────────────

    private void OnUnitChanged(object? sender, EventArgs e)
        => RefreshTemperatures();

    private async void OnAddLocationClicked(object? sender, EventArgs e)
    {
        var city = await DisplayPromptAsync(
            "Add Location",
            "City name",
            accept: "Next",
            cancel: "Cancel",
            placeholder: "Iloilo City",
            maxLength: 40);

        if (string.IsNullOrWhiteSpace(city))
            return;

        var condition = await DisplayPromptAsync(
            "Add Location",
            "Weather condition",
            accept: "Next",
            cancel: "Cancel",
            placeholder: "Partly cloudy",
            maxLength: 40);

        if (string.IsNullOrWhiteSpace(condition))
            return;

        var tempText = await DisplayPromptAsync(
            "Add Location",
            "Temperature in Celsius",
            accept: "Add",
            cancel: "Cancel",
            placeholder: "30",
            keyboard: Keyboard.Numeric);

        if (!double.TryParse(tempText, NumberStyles.Float, CultureInfo.InvariantCulture, out var tempC))
        {
            await DisplayAlertAsync("Invalid Temperature", "Enter a number like 30 or 28.5.", "OK");
            return;
        }

        var location = new AddedLocation(city.Trim(), "Philippines", condition.Trim(), tempC);
        _addedLocations.Add(location);
        SaveAddedLocations();
        AddLocationCard(location);
    }

    // ─── UI refresh ───────────────────────────────────────────────────────────

    private void RefreshTemperatures()
    {
        CebuTempLabel.Text = Settings.FormatTemp(_cebuC);
        ManilaTempLabel.Text = Settings.FormatTemp(_manilaC);
        DavaoTempLabel.Text = Settings.FormatTemp(_davaoC);
        BaguioTempLabel.Text = Settings.FormatTemp(_baguioC);

        foreach (var (label, tempC) in _addedTemperatureLabels)
            label.Text = Settings.FormatTemp(tempC);
    }

    private void LoadAddedLocations()
    {
        _addedLocations.Clear();

        var saved = Preferences.Get(AddedLocationsKey, string.Empty);
        if (string.IsNullOrWhiteSpace(saved))
            return;

        foreach (var row in saved.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = row.Split('|');
            if (parts.Length != 4)
                continue;

            if (!double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var tempC))
                continue;

            _addedLocations.Add(new AddedLocation(parts[0], parts[1], parts[2], tempC));
        }
    }

    private void SaveAddedLocations()
    {
        var rows = _addedLocations.Select(location => string.Join(
            "|",
            Sanitize(location.City),
            Sanitize(location.Country),
            Sanitize(location.Condition),
            location.TempC.ToString(CultureInfo.InvariantCulture)));

        Preferences.Set(AddedLocationsKey, string.Join('\n', rows));
    }

    private void RenderAddedLocations()
    {
        AddedLocationsStack.Clear();
        _addedTemperatureLabels.Clear();

        foreach (var location in _addedLocations)
            AddLocationCard(location);
    }

    private void AddLocationCard(AddedLocation location)
    {
        var tempLabel = new Label
        {
            Text = Settings.FormatTemp(location.TempC),
            FontSize = 28,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.End
        };
        tempLabel.SetDynamicResource(Label.TextColorProperty, "Primary");

        var details = BuildLocationDetails(location);
        var trailing = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Spacing = 0,
            Children =
            {
                new Label
                {
                    Text = PickIcon(location.Condition),
                    FontSize = 40,
                    HorizontalOptions = LayoutOptions.End
                },
                tempLabel
            }
        };
        Grid.SetColumn(trailing, 1);

        var card = new Border
        {
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                Children = { details, trailing }
            }
        };
        card.SetDynamicResource(StyleProperty, "LocationCardStyle");

        AddedLocationsStack.Add(card);
        _addedTemperatureLabels.Add((tempLabel, location.TempC));
    }

    private static VerticalStackLayout BuildLocationDetails(AddedLocation location)
    {
        var timeLabel = new Label
        {
            Text = $"ADDED  {DateTime.Now:HH:mm}",
            FontSize = 10,
            FontAttributes = FontAttributes.Bold
        };
        timeLabel.SetDynamicResource(Label.TextColorProperty, "Outline");

        var cityLabel = new Label
        {
            Text = location.City,
            FontSize = 22,
            FontAttributes = FontAttributes.Bold
        };
        cityLabel.SetDynamicResource(Label.TextColorProperty, "OnSurface");

        var countryLabel = new Label
        {
            Text = location.Country,
            FontSize = 13
        };
        countryLabel.SetDynamicResource(Label.TextColorProperty, "OnSurfaceVariant");

        var conditionLabel = new Label
        {
            Text = location.Condition,
            FontSize = 13
        };
        conditionLabel.SetDynamicResource(Label.TextColorProperty, "OnSurfaceVariant");

        return new VerticalStackLayout
        {
            Spacing = 3,
            Children = { timeLabel, cityLabel, countryLabel, conditionLabel }
        };
    }

    private static string PickIcon(string condition)
    {
        var text = condition.ToLowerInvariant();
        if (text.Contains("rain") || text.Contains("shower"))
            return "🌧️";
        if (text.Contains("cloud"))
            return "☁️";
        if (text.Contains("mist") || text.Contains("fog"))
            return "🌫️";
        if (text.Contains("storm"))
            return "⛈️";

        return "☀️";
    }

    private static string Sanitize(string value) =>
        value.Replace("|", " ").Replace("\r", " ").Replace("\n", " ").Trim();

    private sealed record AddedLocation(string City, string Country, string Condition, double TempC);
}
