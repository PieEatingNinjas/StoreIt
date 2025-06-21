using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Maui.Helpers;

namespace StoreIt.Maui.ViewModels;

[QueryProperty(nameof(ExistingBarcodeData), "barcodeData")]
[QueryProperty(nameof(ExistingBarcodeFormat), "barcodeFormat")]
public partial class ManualBarcodeViewModel : ObservableObject
{
    [ObservableProperty]
    private string barcodeInput = string.Empty;

    [ObservableProperty]
    private string selectedBarcodeType = "Code128";

    [ObservableProperty]
    private bool showBarcodePreview = false;

    [ObservableProperty]
    private bool canAccept = false;

    [ObservableProperty]
    private string? existingBarcodeData;

    [ObservableProperty]
    private string? existingBarcodeFormat;

    public List<string> AvailableBarcodeTypes { get; } = BarCodeHelper.GetSupportedBarcodeFormats();

    partial void OnExistingBarcodeDataChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            BarcodeInput = value;
        }
    }

    partial void OnExistingBarcodeFormatChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value) && AvailableBarcodeTypes.Contains(value))
        {
            SelectedBarcodeType = value;
        }
    }

    partial void OnBarcodeInputChanged(string value)
    {
        UpdatePreview();
    }

    partial void OnSelectedBarcodeTypeChanged(string value)
    {
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        if (string.IsNullOrWhiteSpace(BarcodeInput))
        {
            ShowBarcodePreview = false;
           
            CanAccept = false;
            return;
        }

        ShowBarcodePreview = true;

        try
        {
            CanAccept = BarCodeHelper.IsValidBarcodeFormat(BarcodeInput, SelectedBarcodeType);
        }
        catch
        {
            CanAccept = false;
        }
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task SwitchToScan()
    {
        await Shell.Current.GoToAsync("../scanbarcode");
    }

    [RelayCommand]
    public async Task Accept()
    {
        if (CanAccept && !string.IsNullOrEmpty(BarcodeInput))
        {
            var navigationParameter = new Dictionary<string, object>
            {
                ["barcodeData"] = BarcodeInput,
                ["barcodeFormat"] = SelectedBarcodeType
            };
            
            await Shell.Current.GoToAsync("..", navigationParameter);
        }
    }
}
