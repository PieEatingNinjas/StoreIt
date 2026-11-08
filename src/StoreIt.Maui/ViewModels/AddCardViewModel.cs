using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StoreIt.Maui.Helpers;
using StoreIt.Maui.Models;
using StoreIt.Maui.Services;
using StoreIt.Navigation;
using StoreIt.Services;

namespace StoreIt.Maui.ViewModels;

[QueryProperty(nameof(CardId), NavigationParams.CardId)]
public partial class AddCardViewModel : ObservableObject
{
    private const string TextDisplayOption = CodeTypeCatalog.TextFormat;

    private readonly DatabaseService _databaseService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IAppNavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly IBiometricService _biometricService;

    private CustomerCard? _originalCard;
    private bool _cannotChangeIsPrivate;
    private bool _isUpdatingCodeTypeOptions;
    private bool _keepSelectedCodeType;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string codeInput = string.Empty;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private int cardId;

    [ObservableProperty]
    private string selectedDisplayAs = TextDisplayOption;

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
    private bool isPrivate;

    [ObservableProperty]
    private bool isPrivateOptionAvailable;

    public List<CardColor> AvailableColors { get; } =
    [
        new CardColor { Name = "Oranje", Value = "#FF6B35" },
        new CardColor { Name = "Blauw", Value = "#2196F3" },
        new CardColor { Name = "Groen", Value = "#4CAF50" },
        new CardColor { Name = "Rood", Value = "#F44336" },
        new CardColor { Name = "Paars", Value = "#9C27B0" },
        new CardColor { Name = "Teal", Value = "#009688" },
        new CardColor { Name = "Indigo", Value = "#3F51B5" },
        new CardColor { Name = "Roze", Value = "#E91E63" }
    ];

    private readonly List<CodeTypeOption> _allCodeTypes = CodeTypeCatalog.CreateOptions();

    public ObservableCollection<CodeTypeOption> CodeTypeOptions { get; } = [];

    [ObservableProperty]
    private CodeTypeOption? selectedCodeType;

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
        UpdateCodeTypeOptions();

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
            IsEditing = true;
            _ = LoadCardAsync(value);
        }
        else
        {
            Title = "Nieuw Item";
            IsEditing = false;
            SubTitle = "Nieuw item toevoegen";
        }
    }

    partial void OnSelectedColorObjectChanged(CardColor? oldValue, CardColor? newValue)
    {
        oldValue?.IsSelected = false;
        newValue?.IsSelected = true;
    }

    partial void OnCodeInputChanged(string value)
    {
        UpdateCodeTypeOptions();
    }

    partial void OnSelectedDisplayAsChanged(string value)
    {
        if (!_isUpdatingCodeTypeOptions && value is not null)
        {
            UpdateCodeTypeOptions();
        }
    }

    partial void OnSelectedCodeTypeChanged(CodeTypeOption? value)
    {
        // The carousel may briefly report a different or null item while its items change.
        if (_isUpdatingCodeTypeOptions || value is null || value.Format == SelectedDisplayAs)
        {
            return;
        }

        SelectedDisplayAs = value.Format;
    }

    [RelayCommand]
    private void SelectCodeType(CodeTypeOption option)
    {
        SelectedDisplayAs = option.Format;
    }

    // Shows only the types that can represent the current input. While typing, an incompatible
    // selection falls back to plain text; a loaded or scanned type is always kept.
    private void UpdateCodeTypeOptions()
    {
        var code = CodeInput?.Trim() ?? string.Empty;

        foreach (var option in _allCodeTypes)
        {
            option.IsValid = !option.IsBarcode || code.Length == 0 ||
                             BarCodeHelper.IsValidBarcodeFormat(code, option.Format);
            option.IsSample = code.Length == 0;
            option.PreviewValue = code.Length == 0 ? option.SampleValue : option.IsValid ? code : string.Empty;
        }

        var selected = _allCodeTypes.FirstOrDefault(option => option.Format == SelectedDisplayAs)
                       ?? _allCodeTypes[0];
        if (!selected.IsValid && !_keepSelectedCodeType)
        {
            selected = _allCodeTypes[0];
        }

        foreach (var option in _allCodeTypes)
        {
            option.IsSelected = option == selected;
        }

        var desired = _allCodeTypes.Where(option => option.IsValid || option.IsSelected).ToList();

        _isUpdatingCodeTypeOptions = true;
        try
        {
            // Move the selection first so the carousel never points at an item that is about to be removed.
            SelectedDisplayAs = selected.Format;
            SelectedCodeType = selected;

            if (!CodeTypeOptions.SequenceEqual(desired))
            {
                for (var i = CodeTypeOptions.Count - 1; i >= 0; i--)
                {
                    if (!desired.Contains(CodeTypeOptions[i]))
                    {
                        CodeTypeOptions.RemoveAt(i);
                    }
                }

                for (var i = 0; i < desired.Count; i++)
                {
                    if (i >= CodeTypeOptions.Count || CodeTypeOptions[i] != desired[i])
                    {
                        CodeTypeOptions.Insert(i, desired[i]);
                    }
                }
            }

            SelectedCodeType = selected;
        }
        finally
        {
            _isUpdatingCodeTypeOptions = false;
        }
    }

    private async Task LoadCardAsync(int id)
    {
        try
        {
            _originalCard = await _databaseService.GetCardAsync(id);
            if (_originalCard != null)
            {
                Name = _originalCard.Name;
                Description = _originalCard.Description ?? string.Empty;
                SelectedColor = _originalCard.Color ?? "#FF6B35"; // Default to orange if no color
                IsPrivate = _originalCard.IsPrivate;

                SelectedColorObject = AvailableColors.FirstOrDefault(c => c.Value == SelectedColor) ??
                                      AvailableColors.First();

                var hasBarcode = !string.IsNullOrEmpty(_originalCard.BarcodeData);
                var displayAs = hasBarcode ? _originalCard.BarcodeFormat ?? "QrCode" : TextDisplayOption;
                ApplyCode(displayAs, _originalCard.BarcodeData ?? _originalCard.CustomCode ?? string.Empty);
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
    private Task OpenScanBarcodePage()
    {
        RegisterForBarcodeResult();
        return _navigationService.NavigateToScanBarCodePage();
    }

    private void RegisterForBarcodeResult()
    {
        if (WeakReferenceMessenger.Default.IsRegistered<BarcodeResult>(this))
        {
            return;
        }

        WeakReferenceMessenger.Default.Register<BarcodeResult>(this,
            (r, result) => ((AddCardViewModel)r).OnBarcodeReceived(result));
    }

    [RelayCommand]
    public async Task SetIsPrivateCardCommand(bool isPrivate)
    {
        bool reset = false;
        if (_cannotChangeIsPrivate)
        {
            //Force back to the original value
            OnPropertyChanged(nameof(IsPrivate));
            return;
        }

        _cannotChangeIsPrivate = true;

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

        _cannotChangeIsPrivate = false;
    }

    public void OnBarcodeReceived(BarcodeResult result)
    {
        WeakReferenceMessenger.Default.Unregister<BarcodeResult>(this);

        ApplyCode(result.Format, result.Data);

        // Show save hint when barcode is received
        ShowSaveHintBriefly();
    }

    private void ShowSaveHintBriefly()
    {
        // Only show hint if hints are enabled in settings
        if (!_userPreferencesService.GetHintsEnabled())
        {
            return;
        }

        ShowSaveHint = true;
    }

    // Applies a loaded or scanned code without auto-falling back to text.
    private void ApplyCode(string format, string code)
    {
        AddCodeTypeIfMissing(format);

        _keepSelectedCodeType = true;
        try
        {
            SelectedDisplayAs = format;
            CodeInput = code;
            UpdateCodeTypeOptions();
        }
        finally
        {
            _keepSelectedCodeType = false;
        }
    }

    private void AddCodeTypeIfMissing(string format)
    {
        if (_allCodeTypes.All(option => option.Format != format))
        {
            _allCodeTypes.Add(CodeTypeCatalog.CreateUnknown(format));
        }
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

            var hasCode = !string.IsNullOrWhiteSpace(CodeInput);

            if (!hasCode)
            {
                await _dialogService.DisplayAlert("Validatie",
                    "Voer een code of nummer in.", "OK");
                return;
            }

            var code = CodeInput.Trim();
            var isBarcode = SelectedDisplayAs != TextDisplayOption;
            if (isBarcode && !BarCodeHelper.IsValidBarcodeFormat(code, SelectedDisplayAs))
            {
                await _dialogService.DisplayAlert("Validatie",
                    $"De code is niet geldig voor {SelectedCodeType?.Title ?? SelectedDisplayAs}.", "OK");
                return;
            }

            var card = new CustomerCard
            {
                Id = CardId,
                Name = Name.Trim(),
                Description = Description.Trim(),
                BarcodeData = isBarcode ? code : null,
                BarcodeFormat = isBarcode ? SelectedDisplayAs : null,
                CustomCode = isBarcode ? null : code,
                Color = SelectedColor,
                IsFavorite = _originalCard?.IsFavorite ?? false,
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
