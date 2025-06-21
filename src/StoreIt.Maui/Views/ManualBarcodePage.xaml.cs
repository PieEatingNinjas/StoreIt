using StoreIt.Maui.ViewModels;

namespace StoreIt.Maui.Views;

public partial class ManualBarcodePage : ContentPage
{
    public ManualBarcodePage(ManualBarcodeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
