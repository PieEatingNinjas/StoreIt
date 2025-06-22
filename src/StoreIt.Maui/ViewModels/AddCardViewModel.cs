using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Maui.Models;
using StoreIt.Maui.Services;
using StoreIt.Navigation;
using StoreIt.Services;

namespace StoreIt.Maui.ViewModels;

[QueryProperty(nameof(CardId), NavigationParams.CardId)]
[QueryProperty(nameof(ReceivedBarcodeData), NavigationParams.BarcodeData)]
[QueryProperty(nameof(ReceivedBarcodeFormat), NavigationParams.BarcodeFormat)]
public partial class AddCardViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IAppNavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string? barcodeData;

    [ObservableProperty]
    private string? barcodeFormat;

    [ObservableProperty]
    private bool showBarcodePreview;

    [ObservableProperty]
    private string customCode = string.Empty;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private int cardId;

    [ObservableProperty]
    private string? receivedBarcodeData;

    [ObservableProperty]
    private string? receivedBarcodeFormat;

    [ObservableProperty]
    private bool showBarcodeScanning;

    [ObservableProperty]
    private bool showCustomCodeInput;

    [ObservableProperty]
    private string selectedColor = "#FF6B35"; // Default orange color

    [ObservableProperty]
    private CardColor? selectedColorObject;

    [ObservableProperty]
    private bool showSaveHint;

    [ObservableProperty]
    private string title = "Nieuwe Kaart";
    [ObservableProperty]
    private string subTitle = "Nieuwe kaart toevoegen";
    [ObservableProperty]
    private string saveButtonText = "Opslaan";

    public List<CardColor> AvailableColors { get; } = new()
    {
        new CardColor { Name = "Oranje", Value = "#FF6B35" },
        new CardColor { Name = "Blauw", Value = "#2196F3" },
        new CardColor { Name = "Groen", Value = "#4CAF50" },
        new CardColor { Name = "Rood", Value = "#F44336" },
        new CardColor { Name = "Paars", Value = "#9C27B0" },
        new CardColor { Name = "Teal", Value = "#009688" },
        new CardColor { Name = "Indigo", Value = "#3F51B5" },
        new CardColor { Name = "Roze", Value = "#E91E63" }
    };

    public AddCardViewModel(DatabaseService databaseService,
        IUserPreferencesService userPreferencesService,
        IAppNavigationService navigationService, IDialogService dialogService)
    {
        _databaseService = databaseService;
        _userPreferencesService = userPreferencesService;
        _navigationService = navigationService; 
        _dialogService = dialogService;

        // Initialize with random color for new cards
        SelectRandomColor();
        
        ShowBarcodeScanning = false;
        ShowCustomCodeInput = false;
    }

    partial void OnCardIdChanged(int value)
    {
        if (value > 0)
        {
            Title = "Kaart bewerken";
            SubTitle = "Kaart bewerken";
            SaveButtonText = "Bijwerken";
            IsEditing = true;
            _ = LoadCardAsync(value);
        }
        else
        {
            Title = "Nieuwe Kaart";
            SaveButtonText = "Opslaan";
            IsEditing = false;
            SubTitle = "Nieuwe kaart toevoegen";
        }
    }

    partial void OnReceivedBarcodeDataChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(ReceivedBarcodeFormat))
        {
            var result = new BarcodeResult
            {
                Data = value,
                Format = ReceivedBarcodeFormat
            };
            OnBarcodeReceived(result);
            
            // Clear the received values to prevent re-processing
            ReceivedBarcodeData = null;
            ReceivedBarcodeFormat = null;
        }
    }

    partial void OnReceivedBarcodeFormatChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(ReceivedBarcodeData))
        {
            var result = new BarcodeResult
            {
                Data = ReceivedBarcodeData,
                Format = value
            };
            OnBarcodeReceived(result);
            
            // Clear the received values to prevent re-processing
            ReceivedBarcodeData = null;
            ReceivedBarcodeFormat = null;
        }
    }

    private async Task LoadCardAsync(int id)
    {
        try
        {
            var card = await _databaseService.GetCardAsync(id);
            if (card != null)
            {
                Name = card.Name;
                Description = card.Description ?? string.Empty;
                BarcodeData = card.BarcodeData;
                BarcodeFormat = card.BarcodeFormat;
                CustomCode = card.CustomCode ?? string.Empty;
                SelectedColor = card.Color ?? "#FF6B35"; // Default to orange if no color
                
                SelectedColorObject = AvailableColors.FirstOrDefault(c => c.Value == SelectedColor) ?? AvailableColors.First();
                
                if (!string.IsNullOrEmpty(card.BarcodeData))
                {
                    ShowBarcodeScanning = true;
                    ShowCustomCodeInput = false;
                    ShowBarcodePreview = true;
                }
                else if (!string.IsNullOrEmpty(card.CustomCode))
                {
                    ShowBarcodeScanning = false;
                    ShowCustomCodeInput = true;
                }
            }
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Ooops...", $"Kaart kon niet geladen worden: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public void SelectColor(CardColor color)
    {
        SelectedColorObject = color;
        SelectedColor = color.Value;
    }

    [RelayCommand]
    public void SelectBarcodeOption()
    {
        ShowBarcodeScanning = true;
        ShowCustomCodeInput = false;
        CustomCode = string.Empty; // Clear custom code when switching to barcode
    }

    [RelayCommand]
    public void SelectCustomCodeOption()
    {
        ShowBarcodeScanning = false;
        ShowCustomCodeInput = true;
        BarcodeData = null; // Clear barcode when switching to custom code
        BarcodeFormat = null;
        ShowBarcodePreview = false;
    }

    [RelayCommand]
    public Task OpenScanBarcodePage() => _navigationService.NavigateToScanBarCodePage();

    [RelayCommand]
    public Task OpenManualBarcodePage() => _navigationService.NavigateToAddBarCodePage(BarcodeData, BarcodeFormat);

    public void OnBarcodeReceived(BarcodeResult result)
    {
        BarcodeData = result.Data;
        BarcodeFormat = result.Format;
        ShowBarcodePreview = !string.IsNullOrEmpty(BarcodeData);

        // Show save hint when barcode is received
        ShowSaveHintBriefly();
    }
    
    private void ShowSaveHintBriefly()
    {
        // Only show hint if hints are enabled in settings
        if (!_userPreferencesService.GetHintsEnabled())
            return;
            
        ShowSaveHint = true;
    }

    private void SelectRandomColor()
    {
        if (AvailableColors.Count > 0)
        {
            var random = new Random();
            var randomIndex = random.Next(AvailableColors.Count);
            var randomColor = AvailableColors[randomIndex];
            
            SelectedColor = randomColor.Value;
            SelectedColorObject = randomColor;
        }
    }

    [RelayCommand]
    public async Task SaveCardAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await _dialogService.DisplayAlert("Validatie", "Voer een naam in voor de kaart.", "OK");
                return;
            }

            var hasBarcodeData = !string.IsNullOrWhiteSpace(BarcodeData);
            var hasCustomCode = !string.IsNullOrWhiteSpace(CustomCode);
            
            if (!hasBarcodeData && !hasCustomCode)
            {
                await _dialogService.DisplayAlert("Validatie", "Kies eerst het type kaart (barcode of code) en vul de gegevens in.", "OK");
                return;
            }

            var card = new CustomerCard
            {
                Id = CardId,
                Name = Name.Trim(),
                Description = Description.Trim(),
                BarcodeData = hasBarcodeData ? BarcodeData : null,
                BarcodeFormat = hasBarcodeData ? BarcodeFormat : null,
                CustomCode = hasCustomCode ? CustomCode.Trim() : null,
                Color = SelectedColor
            };

            await _databaseService.SaveCardAsync(card);
            
            await _dialogService.DisplayAlert("Succes", 
                IsEditing ? "Kaart bijgewerkt!" : "Kaart opgeslagen!", "OK");
            
            await _navigationService.GoBack();
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Error", $"Kan kaart niet opslaan: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public Task CancelAsync() => _navigationService.GoBack();
}
