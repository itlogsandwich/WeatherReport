# WeatherReport

IT-ELECT MAUI Finals Project.

A small .NET MAUI weather app for Philippine cities. Live data from
[Open-Meteo](https://open-meteo.com/) (no API key required).

## Stack

- .NET MAUI (net10.0-android)
- CommunityToolkit.Mvvm (source-gen `[ObservableProperty]` / `[RelayCommand]`)
- Open-Meteo forecast API

## Structure

```
WeatherReport/
├── Controls/        Shared XAML controls (TopAppBar)
├── Converters/      IValueConverter implementations
├── Models/          WeatherInfo, ForecastDay, LocationInfo, PhilippineCities
├── Pages/           WeatherPage, LocationPage, SettingsPage
├── Platforms/       Android only
├── Resources/       Fonts, images, styles
├── Services/        SettingsService, WeatherService, SavedLocationsService
└── ViewModels/      WeatherViewModel, LocationsViewModel, SettingsViewModel
```

## Run

```
dotnet build -t:Run -f net10.0-android
```

(Open the solution in Visual Studio and hit F5 with an Android emulator running.)
