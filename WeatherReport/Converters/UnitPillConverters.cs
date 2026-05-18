using System.Globalization;

namespace WeatherReport.Converters;

/// <summary>
/// Helpers used by the Celsius/Fahrenheit toggle on the Settings page.
/// Pass <c>ConverterParameter="invert"</c> for the Fahrenheit pill so it
/// activates when <c>IsCelsius</c> is false.
/// </summary>
internal static class PillState
{
    public static bool IsActive(object? value, object? parameter)
    {
        var isCelsius = value is bool b && b;
        var invert    = parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase);
        return isCelsius != invert;
    }

    public static Color Resource(string key) =>
        Application.Current?.Resources.TryGetValue(key, out var c) == true && c is Color color
            ? color
            : Colors.Transparent;
}

public class UnitPillBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        PillState.IsActive(value, parameter) ? PillState.Resource("Primary") : Colors.Transparent;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

public class UnitPillTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        PillState.IsActive(value, parameter)
            ? PillState.Resource("OnPrimary")
            : PillState.Resource("OnSurfaceVariant");

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
