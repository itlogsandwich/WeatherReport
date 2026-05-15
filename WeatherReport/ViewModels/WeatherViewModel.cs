using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WeatherReport.ViewModels
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        private static WeatherViewModel? _instance;
        public static WeatherViewModel Current => _instance ??= new WeatherViewModel();

        private bool _isCelsius = true;
        public bool IsCelsius
        {
            get => _isCelsius;
            set
            {
                if (_isCelsius != value)
                {
                    _isCelsius = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentTemp));
                    OnPropertyChanged(nameof(HighTemp));
                    OnPropertyChanged(nameof(LowTemp));
                    OnPropertyChanged(nameof(CelsiusBg));
                    OnPropertyChanged(nameof(FahrenheitBg));
                }
            }
        }

        public string Location => "Cebu City, Philippines";
        public string CurrentTemp => IsCelsius ? "33°" : "91°";
        public string HighTemp => IsCelsius ? "H: 35°" : "H: 95°";
        public string LowTemp => IsCelsius ? "L: 26°" : "L: 79°";

        public Color CelsiusBg => IsCelsius ? Color.FromArgb("#29667e") : Color.FromArgb("#dce3e9");
        public Color FahrenheitBg => !IsCelsius ? Color.FromArgb("#29667e") : Color.FromArgb("#dce3e9");

        public ICommand SetCelsiusCommand { get; }
        public ICommand SetFahrenheitCommand { get; }

        public WeatherViewModel()
        {
            SetCelsiusCommand = new Command(() => IsCelsius = true);
            SetFahrenheitCommand = new Command(() => IsCelsius = false);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
