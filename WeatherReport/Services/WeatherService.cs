using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using WeatherReport.Models;

namespace WeatherReport.Services;

public class WeatherService : IWeatherService
{
    private const string BaseUrl = "https://api.open-meteo.com/v1/forecast";
    private const string Timezone = "Asia/Manila";
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(12);

    private readonly HttpClient _http;

    public WeatherService(HttpClient http)
    {
        _http = http;
    }

    public async Task<WeatherInfo> GetWeatherAsync(LocationInfo location, CancellationToken cancellationToken = default)
    {
        var lat = location.Latitude.ToString("0.0000", CultureInfo.InvariantCulture);
        var lon = location.Longitude.ToString("0.0000", CultureInfo.InvariantCulture);

        var url = $"{BaseUrl}?latitude={lat}&longitude={lon}" +
            "&current=temperature_2m,relative_humidity_2m,apparent_temperature," +
            "wind_speed_10m,wind_gusts_10m,wind_direction_10m,uv_index,weather_code,dew_point_2m," +
            "precipitation,rain,showers" +
            "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum,precipitation_probability_max" +
            $"&timezone={Uri.EscapeDataString(Timezone)}&forecast_days=5";

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(RequestTimeout);

        OpenMeteoResponse? dto;
        try
        {
            dto = await _http.GetFromJsonAsync<OpenMeteoResponse>(url, timeout.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("Open-Meteo took too long to respond.");
        }

        if (dto is null)
            throw new InvalidOperationException("Weather API returned an empty response.");

        return Map(location, dto);
    }

    private static WeatherInfo Map(LocationInfo location, OpenMeteoResponse dto)
    {
        var current = dto.Current ?? throw new InvalidOperationException("Missing current weather block.");
        var daily   = dto.Daily   ?? throw new InvalidOperationException("Missing daily forecast block.");

        var (icon, condition) = WeatherCodeMapper.Map(current.WeatherCode);

        var info = new WeatherInfo
        {
            City        = location.City,
            Country     = location.Country,
            LocalTime   = FormatLocalTime(current.Time),
            TempC       = current.Temperature,
            FeelsLikeC  = current.ApparentTemperature,
            HighC       = daily.MaxTemps.Length > 0 ? daily.MaxTemps[0] : current.Temperature,
            LowC        = daily.MinTemps.Length > 0 ? daily.MinTemps[0] : current.Temperature,
            Condition   = condition,
            Icon        = icon,
            Humidity    = (int)Math.Round(current.Humidity),
            DewPointC   = current.DewPoint,
            WindKph     = current.WindSpeed,
            WindGustKph = current.WindGusts,
            WindDir     = WeatherCodeMapper.WindCardinal(current.WindDirection),
            UvIndex     = (int)Math.Round(current.UvIndex),
            UvLabel     = WeatherCodeMapper.UvLabel(current.UvIndex),
            PrecipitationMm          = current.Precipitation,
            RainMm                   = current.Rain,
            ShowersMm                = current.Showers,
            DailyPrecipitationMm     = daily.PrecipitationSums.Length > 0 ? daily.PrecipitationSums[0] : 0,
            PrecipitationProbability = daily.PrecipitationProbabilityMax.Length > 0
                ? (int)Math.Round(daily.PrecipitationProbabilityMax[0])
                : 0,
        };

        var dayCount = Math.Min(5, Math.Min(daily.Times.Length, Math.Min(daily.MaxTemps.Length, daily.MinTemps.Length)));
        for (int i = 0; i < dayCount; i++)
        {
            var (dayIcon, _) = WeatherCodeMapper.Map(daily.WeatherCodes.Length > i ? daily.WeatherCodes[i] : 0);
            info.Forecast.Add(new ForecastDay
            {
                DayName = i == 0 ? "Today" : daily.Times[i].ToString("ddd", CultureInfo.InvariantCulture),
                HighC   = daily.MaxTemps[i],
                LowC    = daily.MinTemps[i],
                Icon    = dayIcon,
                PrecipitationMm = daily.PrecipitationSums.Length > i ? daily.PrecipitationSums[i] : 0,
                PrecipitationProbability = daily.PrecipitationProbabilityMax.Length > i
                    ? (int)Math.Round(daily.PrecipitationProbabilityMax[i])
                    : 0,
            });
        }

        return info;
    }

    private static string FormatLocalTime(DateTime time) =>
        time.ToString("HH:mm", CultureInfo.InvariantCulture);

    // ── DTOs ──────────────────────────────────────────────────────────────
    private sealed class OpenMeteoResponse
    {
        [JsonPropertyName("current")] public CurrentBlock? Current { get; set; }
        [JsonPropertyName("daily")]   public DailyBlock?   Daily   { get; set; }
    }

    private sealed class CurrentBlock
    {
        [JsonPropertyName("time")]                 public DateTime Time { get; set; }
        [JsonPropertyName("temperature_2m")]       public double Temperature { get; set; }
        [JsonPropertyName("relative_humidity_2m")] public double Humidity { get; set; }
        [JsonPropertyName("apparent_temperature")] public double ApparentTemperature { get; set; }
        [JsonPropertyName("wind_speed_10m")]       public double WindSpeed { get; set; }
        [JsonPropertyName("wind_gusts_10m")]       public double WindGusts { get; set; }
        [JsonPropertyName("wind_direction_10m")]   public double WindDirection { get; set; }
        [JsonPropertyName("uv_index")]             public double UvIndex { get; set; }
        [JsonPropertyName("weather_code")]         public int WeatherCode { get; set; }
        [JsonPropertyName("dew_point_2m")]         public double DewPoint { get; set; }
        [JsonPropertyName("precipitation")]        public double Precipitation { get; set; }
        [JsonPropertyName("rain")]                 public double Rain { get; set; }
        [JsonPropertyName("showers")]              public double Showers { get; set; }
    }

    private sealed class DailyBlock
    {
        [JsonPropertyName("time")]                          public DateTime[] Times                       { get; set; } = Array.Empty<DateTime>();
        [JsonPropertyName("weather_code")]                  public int[]      WeatherCodes                { get; set; } = Array.Empty<int>();
        [JsonPropertyName("temperature_2m_max")]            public double[]   MaxTemps                    { get; set; } = Array.Empty<double>();
        [JsonPropertyName("temperature_2m_min")]            public double[]   MinTemps                    { get; set; } = Array.Empty<double>();
        [JsonPropertyName("precipitation_sum")]             public double[]   PrecipitationSums           { get; set; } = Array.Empty<double>();
        [JsonPropertyName("precipitation_probability_max")] public double[]   PrecipitationProbabilityMax { get; set; } = Array.Empty<double>();
    }
}
