namespace WeatherReport.Services;

/// <summary>Maps WMO weather codes (Open-Meteo) to a display icon + human label.</summary>
internal static class WeatherCodeMapper
{
    public static (string Icon, string Label) Map(int code) => code switch
    {
        0           => ("☀️",  "Clear"),
        1           => ("🌤️", "Mostly Sunny"),
        2           => ("⛅",  "Partly Cloudy"),
        3           => ("☁️",  "Cloudy"),
        45 or 48    => ("🌫️", "Fog"),
        51 or 53 or 55 => ("🌦️", "Drizzle"),
        56 or 57    => ("🌨️", "Freezing Drizzle"),
        61 or 63 or 65 => ("🌧️", "Rain"),
        66 or 67    => ("🌨️", "Freezing Rain"),
        71 or 73 or 75 or 77 => ("❄️", "Snow"),
        80 or 81 or 82 => ("🌧️", "Rain Showers"),
        85 or 86    => ("🌨️", "Snow Showers"),
        95          => ("⛈️", "Thunderstorm"),
        96 or 99    => ("⛈️", "Thunderstorm w/ Hail"),
        _           => ("🌡️", "Unknown"),
    };

    /// <summary>Pick a UV label aligned with the WHO scale.</summary>
    public static string UvLabel(double uv) => uv switch
    {
        < 3  => "Low",
        < 6  => "Moderate",
        < 8  => "High",
        < 11 => "Very High",
        _    => "Extreme",
    };

    /// <summary>Convert wind direction degrees to a compact cardinal label.</summary>
    public static string WindCardinal(double degrees)
    {
        string[] points = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        var idx = (int)Math.Round(degrees / 45.0) % 8;
        if (idx < 0) idx += 8;
        return points[idx];
    }
}
