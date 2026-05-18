namespace WeatherReport.Controls;

public partial class TopAppBar : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TopAppBar), "WeatherReport");

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public TopAppBar()
    {
        InitializeComponent();
    }
}
