using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZXing.Net.Maui;

namespace StoreIt.Maui.ViewModels;

public partial class ScanBarcodeViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isScanning = true;

    [ObservableProperty]
    private string? scannedData;

    [ObservableProperty]
    private string? scannedFormat;

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task SwitchToManual()
    {
        await Shell.Current.GoToAsync("../manualbarcode");
    }

    public async Task Accept()
    {
        if (!string.IsNullOrEmpty(ScannedData) && !string.IsNullOrEmpty(ScannedFormat))
        {
            var navigationParameter = new Dictionary<string, object>
            {
                ["barcodeData"] = ScannedData,
                ["barcodeFormat"] = ScannedFormat
            };
            
            await Shell.Current.GoToAsync("..", navigationParameter);
        }
    }

    public void OnBarcodeDetected(BarcodeDetectionEventArgs e)
    {
        if (e.Results?.FirstOrDefault() is { } barcode)
        {
            IsScanning = false;
            ScannedData = barcode.Value;
            ScannedFormat = barcode.Format.ToString();
            _= Accept();
        }
    }
}

public class BarcodeResult
{
    public string Data { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
}
