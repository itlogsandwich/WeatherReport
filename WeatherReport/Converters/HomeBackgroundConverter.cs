using System.Globalization;

namespace WeatherReport.Converters;

/// <summary>True → <c>PrimaryContainer</c>, false → <c>Surface</c>. Looks up the current theme palette via Application resources.</summary>
public class HomeBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var key = value is bool b && b ? "PrimaryContainer" : "Surface";
        if (Application.Current?.Resources.TryGetValue(key, out var color) == true && color is Color c)
            return c;
        return Colors.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
