using WeatherReport.Models;
using WeatherReport.ViewModels;

namespace WeatherReport.Pages;

public partial class LocationsPage : ContentPage
{
    private readonly LocationsViewModel _vm;
    private bool _loadedOnce;

    public LocationsPage(LocationsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_loadedOnce) return;
        _loadedOnce = true;
        await _vm.RefreshAsync();
    }

    private async void OnAddLocationClicked(object? sender, EventArgs e)
    {
        var available = _vm.AvailableToAdd;
        if (available.Count == 0)
        {
            await DisplayAlertAsync("Add Location", "All catalog cities are already saved.", "OK");
            return;
        }

        var names = available.Select(c => c.City).ToArray();
        var choice = await DisplayActionSheetAsync("Add a city", "Cancel", null, names);

        if (string.IsNullOrEmpty(choice) || choice == "Cancel")
            return;

        var pick = available.FirstOrDefault(c => c.City == choice);
        if (pick is not null)
            _vm.AddCommand.Execute(pick);
    }
}
