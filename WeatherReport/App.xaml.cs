using Microsoft.Extensions.DependencyInjection;

namespace WeatherReport
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Services.SettingsService.Instance.ApplyTheme();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
