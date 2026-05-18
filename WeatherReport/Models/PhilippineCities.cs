namespace WeatherReport.Models;

/// <summary>Catalog of supported Philippine cities (name + coordinates) for the city picker.</summary>
public static class PhilippineCities
{
    public static readonly LocationInfo CebuCity = new("Cebu City", 10.3157, 123.8854);

    public static readonly IReadOnlyList<LocationInfo> All = new[]
    {
        CebuCity,
        new LocationInfo("Manila",          14.5995, 120.9842),
        new LocationInfo("Quezon City",     14.6760, 121.0437),
        new LocationInfo("Davao City",       7.0731, 125.6128),
        new LocationInfo("Baguio",          16.4023, 120.5960),
        new LocationInfo("Iloilo City",     10.7202, 122.5621),
        new LocationInfo("Cagayan de Oro",   8.4542, 124.6319),
        new LocationInfo("Zamboanga City",   6.9214, 122.0790),
        new LocationInfo("Tagaytay",        14.1153, 120.9621),
        new LocationInfo("Puerto Princesa",  9.7407, 118.7359),
        new LocationInfo("Tacloban",        11.2447, 125.0048),
        new LocationInfo("General Santos",   6.1164, 125.1716),
    };
}
