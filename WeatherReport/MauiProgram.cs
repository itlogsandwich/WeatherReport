using Microsoft.Extensions.Logging;
using WeatherReport.Pages;
using WeatherReport.Services;
using WeatherReport.ViewModels;

namespace WeatherReport
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // ── Services ──────────────────────────────────────────────────
            builder.Services.AddSingleton(_ => new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15),
            });
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<ISavedLocationsService, SavedLocationsService>();
            builder.Services.AddSingleton<IWeatherService, WeatherService>();

            // ── ViewModels ────────────────────────────────────────────────
            builder.Services.AddTransient<WeatherViewModel>();
            builder.Services.AddTransient<LocationsViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();

            // ── Pages ─────────────────────────────────────────────────────
            builder.Services.AddTransient<WeatherPage>();
            builder.Services.AddTransient<LocationsPage>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
