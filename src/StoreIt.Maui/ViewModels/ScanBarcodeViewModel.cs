using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
    private Task Cancel() => _navigationService.GoBack();

    [RelayCommand]
    private Task SwitchToManual() => _navigationService.SwitchToManualBarCodePage();

    private Task Accept()
    {
        if (!string.IsNullOrEmpty(ScannedData) && !string.IsNullOrEmpty(ScannedFormat))
        {
            WeakReferenceMessenger.Default.Send(new BarcodeResult
            {
                Data = ScannedData,
                Format = ScannedFormat
            });
            return _navigationService.GoBack();
        }

        return Task.CompletedTask;
    }

    public void OnBarcodeDetected(BarcodeDetectionEventArgs e)
    {
        if (IsScanning && e.Results?.FirstOrDefault() is { } barcode)
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