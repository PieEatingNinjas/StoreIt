using Microsoft.Maui.Controls;

namespace StoreIt.Maui.Controls;

public partial class IconControl : ContentView
{
    public static readonly BindableProperty IconBackgroundColorProperty =
        BindableProperty.Create(nameof(IconBackgroundColor), typeof(Color), typeof(IconControl), Colors.Transparent);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(IconControl), string.Empty);

    public static readonly BindableProperty IconSizeProperty =
        BindableProperty.Create(nameof(IconSize), typeof(double), typeof(IconControl), 24.0);

    public static readonly BindableProperty BorderSizeProperty =
        BindableProperty.Create(nameof(BorderSize), typeof(double), typeof(IconControl), 50.0);

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(IconControl), 25.0);

    public Color IconBackgroundColor
    {
        get => (Color)GetValue(IconBackgroundColorProperty);
        set => SetValue(IconBackgroundColorProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public double BorderSize
    {
        get => (double)GetValue(BorderSizeProperty);
        set => SetValue(BorderSizeProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public IconControl()
    {
        InitializeComponent();
    }
}
