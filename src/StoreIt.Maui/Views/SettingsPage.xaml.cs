using StoreIt.Maui.ViewModels;

namespace StoreIt.Maui.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnSystemThemeTapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is SettingsViewModel viewModel)
        {
            viewModel.IsSystemTheme = true;
        }
    }

    private void OnLightThemeTapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is SettingsViewModel viewModel)
        {
            viewModel.IsLightTheme = true;
        }
    }

    private void OnDarkThemeTapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is SettingsViewModel viewModel)
        {
            viewModel.IsDarkTheme = true;
        }
    }
}
