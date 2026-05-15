using Microsoft.Extensions.Logging;
using WeatherReport.Pages;
using WeatherReport.Services;

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
            // Services
            builder.Services.AddSingleton(SettingsService.Instance);

            // Pages
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
