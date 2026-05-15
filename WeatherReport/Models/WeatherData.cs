namespace WeatherReport.Models;

/// <summary>Current weather snapshot for a single location.</summary>
public class WeatherInfo
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string LocalTime { get; set; } = string.Empty;

    // Temperatures stored in Celsius; the SettingsService converts for display.
    public double TempC { get; set; }
    public double HighC { get; set; }
    public double LowC { get; set; }
    public double FeelsLikeC { get; set; }

    public string Condition { get; set; } = string.Empty;
    /// <summary>Unicode emoji used as the weather icon (no extra font required).</summary>
    public string Icon { get; set; } = string.Empty;

    public int Humidity { get; set; }        // percent
    public double DewPointC { get; set; }
    public double WindKph { get; set; }
    public double WindGustKph { get; set; }
    public string WindDir { get; set; } = string.Empty;
    public int UvIndex { get; set; }
    public string UvLabel { get; set; } = string.Empty;

    public List<ForecastDay> Forecast { get; set; } = new();
}

/// <summary>One row in the 5-day forecast.</summary>
public class ForecastDay
{
    public string DayName { get; set; } = string.Empty;
    public double HighC { get; set; }
    public double LowC { get; set; }
    public string Icon { get; set; } = string.Empty;
}

/// <summary>
/// Static factory that returns hard-coded weather data for the four
/// Philippine cities used in the Locations screen.
/// Replace with a live API call when ready.
/// </summary>
public static class WeatherDataFactory
{
    public static WeatherInfo GetCebuCity() => new()
    {
        City = "Cebu City",
        Country = "Philippines",
        LocalTime = "14:30",
        TempC = 33,
        HighC = 35,
        LowC = 28,
        FeelsLikeC = 37,
        Condition = "Sunny",
        Icon = "☀️",
        Humidity = 78,
        DewPointC = 27,
        WindKph = 15,
        WindGustKph = 22,
        WindDir = "Southeast",
        UvIndex = 8,
        UvLabel = "High",
        Forecast = new List<ForecastDay>
        {
            new() { DayName = "Today", HighC = 35, LowC = 28, Icon = "☀️"  },
            new() { DayName = "Tue",   HighC = 34, LowC = 27, Icon = "⛅"  },
            new() { DayName = "Wed",   HighC = 30, LowC = 25, Icon = "🌧️" },
            new() { DayName = "Thu",   HighC = 29, LowC = 24, Icon = "🌧️" },
            new() { DayName = "Fri",   HighC = 32, LowC = 26, Icon = "⛅"  },
        }
    };

    public static List<WeatherInfo> GetSavedLocations() => new()
    {
        // Cebu City is the "home" location — highlighted at the top.
        new()
        {
            City      = "Cebu City",
            Country   = "Philippines",
            LocalTime = "14:30",
            TempC     = 33,
            Condition = "Sunny",
            Icon      = "☀️",
        },
        new()
        {
            City      = "Manila",
            Country   = "Philippines",
            LocalTime = "14:30",
            TempC     = 31,
            Condition = "Cloudy",
            Icon      = "☁️",
        },
        new()
        {
            City      = "Davao City",
            Country   = "Philippines",
            LocalTime = "14:30",
            TempC     = 28,
            Condition = "Rain Showers",
            Icon      = "🌧️",
        },
        new()
        {
            City      = "Baguio",
            Country   = "Philippines",
            LocalTime = "14:30",
            TempC     = 22,
            Condition = "Mist",
            Icon      = "🌫️",
        },
    };
}
