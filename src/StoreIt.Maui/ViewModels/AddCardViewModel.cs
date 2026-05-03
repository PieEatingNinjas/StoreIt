using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StoreIt.Maui.Models;
using StoreIt.Maui.Services;
using StoreIt.Navigation;
using StoreIt.Services;

namespace StoreIt.Maui.ViewModels;

[QueryProperty(nameof(CardId), NavigationParams.CardId)]
public partial class AddCardViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IAppNavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly IBiometricService _biometricService;

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
    private string title = "Nieuw Item";

    [ObservableProperty]
    private string subTitle = "Nieuw item toevoegen";

    [ObservableProperty]
    private string saveButtonText = "Opslaan";

    [ObservableProperty]
    private bool isPrivate;

    [ObservableProperty]
    private bool isPrivateOptionAvailable;

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
        IAppNavigationService navigationService,
        IDialogService dialogService,
        IBiometricService biometricService)
    {
        _databaseService = databaseService;
        _userPreferencesService = userPreferencesService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _biometricService = biometricService;

        // Initialize with random color for new cards
        SelectRandomColor();

        ShowBarcodeScanning = false;
        ShowCustomCodeInput = false;
        _ = Init();
    }

    private async Task Init()
    {
        IsPrivateOptionAvailable = await _biometricService.IsAvailableAsync();
    }

    partial void OnCardIdChanged(int value)
    {
        if (value > 0)
        {
            Title = "Item bewerken";
            SubTitle = "Item bewerken";
            SaveButtonText = "Bijwerken";
            IsEditing = true;
            _ = LoadCardAsync(value);
        }
        else
        {
            Title = "Nieuw Item";
            SaveButtonText = "Opslaan";
            IsEditing = false;
            SubTitle = "Nieuw item toevoegen";
        }
    }

    CustomerCard? _originalCard;

    private async Task LoadCardAsync(int id)
    {
        try
        {
            _originalCard = await _databaseService.GetCardAsync(id);
            if (_originalCard != null)
            {
                Name = _originalCard.Name;
                Description = _originalCard.Description ?? string.Empty;
                BarcodeData = _originalCard.BarcodeData;
                BarcodeFormat = _originalCard.BarcodeFormat;
                CustomCode = _originalCard.CustomCode ?? string.Empty;
                SelectedColor = _originalCard.Color ?? "#FF6B35"; // Default to orange if no color
                IsPrivate = _originalCard.IsPrivate;

                SelectedColorObject = AvailableColors.FirstOrDefault(c => c.Value == SelectedColor) ??
                                      AvailableColors.First();

                if (!string.IsNullOrEmpty(_originalCard.BarcodeData))
                {
                    ShowBarcodeScanning = true;
                    ShowCustomCodeInput = false;
                    ShowBarcodePreview = true;
                }
                else if (!string.IsNullOrEmpty(_originalCard.CustomCode))
                {
                    ShowBarcodeScanning = false;
                    ShowCustomCodeInput = true;
                }
            }
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Ooops...", $"Item kon niet geladen worden: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private void SelectColor(CardColor color)
    {
        SelectedColorObject = color;
        SelectedColor = color.Value;
    }

    [RelayCommand]
    private void SelectBarcodeOption()
    {
        ShowBarcodeScanning = true;
        ShowCustomCodeInput = false;
        CustomCode = string.Empty; // Clear custom code when switching to barcode
    }

    [RelayCommand]
    private void SelectCustomCodeOption()
    {
        ShowBarcodeScanning = false;
        ShowCustomCodeInput = true;
        BarcodeData = null; // Clear barcode when switching to custom code
        BarcodeFormat = null;
        ShowBarcodePreview = false;
    }

    [RelayCommand]
    private Task OpenScanBarcodePage()
    {
        RegisterForBarcodeResult();
        return _navigationService.NavigateToScanBarCodePage();
    }

    [RelayCommand]
    private Task OpenManualBarcodePage()
    {
        RegisterForBarcodeResult();
        return _navigationService.NavigateToAddBarCodePage(BarcodeData, BarcodeFormat);
    }

    private void RegisterForBarcodeResult()
    {
        if (WeakReferenceMessenger.Default.IsRegistered<BarcodeResult>(this))
            return;

        WeakReferenceMessenger.Default.Register<BarcodeResult>(this,
            (r, result) => ((AddCardViewModel)r).OnBarcodeReceived(result));
    }

    bool cannotChangeIsPrivate = false;

    [RelayCommand]
    public async Task SetIsPrivateCardCommand(bool isPrivate)
    {
        bool reset = false;
        if (cannotChangeIsPrivate)
        {
            //Force back to the original value
            OnPropertyChanged(nameof(IsPrivate));
            return;
        }

        cannotChangeIsPrivate = true;

        bool shouldAuthenticate;
        if (_originalCard is not null && _originalCard.IsPrivate)
        {
            // If the original card is private, we need to authenticate if the new value is different
            shouldAuthenticate = _originalCard.IsPrivate != isPrivate;
        }
        else
        {
            // If the original card is not private, we only need to authenticate if the new value is true
            shouldAuthenticate = isPrivate;
        }

        if (shouldAuthenticate)
        {
            if (await _biometricService.IsAvailableAsync())
            {
                if (await _biometricService.AuthenticateAsync("Authenticeer om de privacy-instelling te wijzigen."))
                {
                    //All good!
                    IsPrivate = isPrivate;
                }
                else
                {
                    reset = true;
                }
            }
            else
            {
                reset = true;
            }
        }

        if (reset)
        {
            //Force back to the original value
            OnPropertyChanged(nameof(IsPrivate));
        }

        cannotChangeIsPrivate = false;
    }

    public void OnBarcodeReceived(BarcodeResult result)
    {
        WeakReferenceMessenger.Default.Unregister<BarcodeResult>(this);

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
    private async Task SaveCardAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await _dialogService.DisplayAlert("Validatie", "Voer een naam in.", "OK");
                return;
            }

            var hasBarcodeData = !string.IsNullOrWhiteSpace(BarcodeData);
            var hasCustomCode = !string.IsNullOrWhiteSpace(CustomCode);

            if (!hasBarcodeData && !hasCustomCode)
            {
                await _dialogService.DisplayAlert("Validatie",
                    "Kies eerst het type (barcode of code) en vul de gegevens in.", "OK");
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
                Color = SelectedColor,
                IsPrivate = IsPrivate
            };

            await _databaseService.SaveCardAsync(card);

            await _dialogService.DisplayAlert("Succes",
                IsEditing ? "Item bijgewerkt!" : "Item opgeslagen!", "OK");

            await _navigationService.GoBack();
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Error", $"Kan Item niet opslaan: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private Task CancelAsync() => _navigationService.GoBack();
}