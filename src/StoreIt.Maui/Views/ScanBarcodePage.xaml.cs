using StoreIt.Maui.ViewModels;
using ZXing.Net.Maui;

namespace StoreIt.Maui.Views;

public partial class ScanBarcodePage : ContentPage
{
    public ScanBarcodePage(ScanBarcodeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (BindingContext is ScanBarcodeViewModel vm)
        {
            vm.OnBarcodeDetected(e);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        cameraView.IsDetecting = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cameraView.IsDetecting = false;
    }
}
