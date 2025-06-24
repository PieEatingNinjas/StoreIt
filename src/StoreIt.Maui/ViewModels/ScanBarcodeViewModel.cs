using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Navigation;
using StoreIt.Services;
using ZXing.Net.Maui;

namespace StoreIt.Maui.ViewModels;

public partial class ScanBarcodeViewModel : ObservableObject
{
    private readonly IAppNavigationService _navigationService;

    [ObservableProperty]
    private bool isScanning = true;

    [ObservableProperty]
    private string? scannedData;

    [ObservableProperty]
    private string? scannedFormat;

    [RelayCommand]
    public Task Cancel() => _navigationService.GoBack();

    [RelayCommand]
    public Task SwitchToManual() => _navigationService.SwitchToManualBarCodePage();

    public Task Accept()
    {
        if (!string.IsNullOrEmpty(ScannedData) && !string.IsNullOrEmpty(ScannedFormat))
        {
            return _navigationService.GoBack(new Dictionary<string, object>
            {
                [NavigationParams.BarcodeData] = ScannedData,
                [NavigationParams.BarcodeFormat] = ScannedFormat
            });
        }
        return Task.CompletedTask;
    }

    public void OnBarcodeDetected(BarcodeDetectionEventArgs e)
    {
        if (e.Results?.FirstOrDefault() is { } barcode)
        {
            IsScanning = false;
            ScannedData = barcode.Value;
            ScannedFormat = barcode.Format.ToString();
            App.Current!.Dispatcher.DispatchAsync(Accept);
        }
    }
    
    public ScanBarcodeViewModel(IAppNavigationService appNavigationService)
    {
       _navigationService = appNavigationService;
    }
}

public class BarcodeResult
{
    public string Data { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
}
