using WeatherReport.ViewModels;

namespace WeatherReport.Pages;

public partial class WeatherPage : ContentPage
{
    private readonly WeatherViewModel _vm;
    private bool _loadedOnce;

    public WeatherPage(WeatherViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_loadedOnce) return;
        _loadedOnce = true;
        await _vm.LoadAsync();
    }
}
