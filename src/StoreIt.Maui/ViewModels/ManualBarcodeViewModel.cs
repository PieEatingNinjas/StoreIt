using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StoreIt.Maui.Helpers;
using StoreIt.Navigation;
using StoreIt.Services;

namespace StoreIt.Maui.ViewModels;

[QueryProperty(nameof(ExistingBarcodeData), NavigationParams.BarcodeData)]
[QueryProperty(nameof(ExistingBarcodeFormat), NavigationParams.BarcodeFormat)]
public partial class ManualBarcodeViewModel : ObservableObject
{
    private readonly IAppNavigationService _appNavigationService;

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

    public ManualBarcodeViewModel(IAppNavigationService appNavigationService)
    {
        _appNavigationService = appNavigationService;
    }

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
        => UpdatePreview();

    partial void OnSelectedBarcodeTypeChanged(string value)
        => UpdatePreview();

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
    public Task Cancel() => _appNavigationService.GoBack();

    [RelayCommand]
    public Task SwitchToScan() => _appNavigationService.SwitchToScanBarCodePage();

    [RelayCommand]
    public Task Accept()
    {
        if (CanAccept && !string.IsNullOrEmpty(BarcodeInput))
        {
            WeakReferenceMessenger.Default.Send(new BarcodeResult
            {
                Data = BarcodeInput,
                Format = SelectedBarcodeType
            });
            return _appNavigationService.GoBack();
        }
        return Task.CompletedTask; 
    }
}
