namespace WeatherReport.Models;

public class WeatherInfo
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = "Philippines";
    public string LocalTime { get; set; } = string.Empty;

    public double TempC { get; set; }
    public double HighC { get; set; }
    public double LowC { get; set; }
    public double FeelsLikeC { get; set; }

    public string Condition { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public int Humidity { get; set; }
    public double DewPointC { get; set; }
    public double WindKph { get; set; }
    public double WindGustKph { get; set; }
    public string WindDir { get; set; } = string.Empty;
    public int UvIndex { get; set; }
    public string UvLabel { get; set; } = string.Empty;

    public List<ForecastDay> Forecast { get; set; } = new();
}
