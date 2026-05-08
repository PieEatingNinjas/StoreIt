using StoreIt.Maui.ViewModels;

namespace StoreIt.Maui.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel = viewModel;
    }
}