using WeatherReport.Services;

namespace WeatherReport
{
    public partial class App : Application
    {
        public App(SettingsService settings)
        {
            InitializeComponent();
            settings.ApplyTheme();
        }

        protected override Window CreateWindow(IActivationState? activationState) =>
            new Window(new AppShell());
    }
}
