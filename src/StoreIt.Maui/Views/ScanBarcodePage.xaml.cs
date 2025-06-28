using StoreIt.Maui.ViewModels;
using ZXing.Net.Maui;

namespace StoreIt.Maui.Views;

public partial class ScanBarcodePage : ContentPage
{
    public ScanBarcodePage(ScanBarcodeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        cameraView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormat.Code128 | BarcodeFormat.Code39 | BarcodeFormat.Code93 |
                      BarcodeFormat.Ean13 | BarcodeFormat.Ean8 | BarcodeFormat.UpcA |
                      BarcodeFormat.UpcE | BarcodeFormat.Codabar | BarcodeFormat.Itf |
                      BarcodeFormat.QrCode | BarcodeFormat.DataMatrix | BarcodeFormat.Pdf417,
            AutoRotate = true,
            Multiple = false,
            TryHarder = false
        };
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
