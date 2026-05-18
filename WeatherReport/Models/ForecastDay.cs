namespace WeatherReport.Models;

public class ForecastDay
{
    public string DayName { get; set; } = string.Empty;
    public double HighC { get; set; }
    public double LowC { get; set; }
    public string Icon { get; set; } = string.Empty;
    public double PrecipitationMm { get; set; }
    public int PrecipitationProbability { get; set; }
}
