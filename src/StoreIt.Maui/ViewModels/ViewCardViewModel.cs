using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Maui.Models;
using StoreIt.Maui.Services;
using StoreIt.Navigation;
using StoreIt.Services;

namespace StoreIt.Maui.ViewModels;

[QueryProperty(nameof(CardId), NavigationParams.CardId)]
public partial class ViewCardViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly IPlatformBrightnessService _brightnessService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IAppNavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private CustomerCard? card;

    [ObservableProperty]
    private int cardId;

    private bool isBrightnessControlSupported;

    [ObservableProperty]
    private bool showZoomHint;

    [ObservableProperty]
    private bool showCopyHint;

    public ViewCardViewModel(DatabaseService databaseService,
        IPlatformBrightnessService brightnessService,
        IUserPreferencesService userPreferencesService,
        IAppNavigationService appNavigationService,
        IDialogService dialogService)
    {
        _databaseService = databaseService;
        _brightnessService = brightnessService;
        _userPreferencesService = userPreferencesService;
        _navigationService = appNavigationService;
        _dialogService = dialogService;
        isBrightnessControlSupported = _brightnessService.IsBrightnessControlSupported;
    }

    partial void OnCardIdChanged(int value)
    {
        if (value > 0)
        {
            _ = LoadCardAsync(value);
        }
    }

    private async Task LoadCardAsync(int id)
    {
        try
        {
            bool showHints = _userPreferencesService.GetHintsEnabled();

            Card = await _databaseService.GetCardAsync(id);
            if (Card != null)
            {
                await _databaseService.UpdateLastUsedAsync(Card.Id);

                if (isBrightnessControlSupported)
                {
                    // Save original brightness when entering card view
                    _brightnessService.SaveOriginalBrightness();

                    if (Card.HasBarcode)
                    {
                        // For barcode cards: automatically set max brightness and hide slider
                        _brightnessService.SetSystemBrightness(1.0f);

                        // Only show zoom hint if hints are enabled in settings
                    }
                }

                LoadHints();
            }
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Ooops...", $"Kaart kon niet geladen worden: {ex.Message}", "OK");
        }
    }

    private void LoadHints()
    {
        if (_userPreferencesService.GetHintsEnabled())
        {
            ShowZoomHint = Card?.HasBarcode == true;
            ShowCopyHint = Card?.HasCustomCode == true;
        }
        else
        {
            ShowZoomHint = false;
            ShowCopyHint = false;
        }
    }

    [RelayCommand]
    public Task EditCardAsync() => _navigationService.NavigateToEditCardPage(Card?.Id ?? 0);

    [RelayCommand]
    public async Task ReloadCardAsync()
    {
        if (Card != null)
        {
            await LoadCardAsync(Card.Id);
        }
    }

    [RelayCommand]
    public async Task DeleteCardAsync()
    {
        if (Card == null) return;

        bool confirm = await _dialogService.DisplayAlert("Ben je zeker?",
            $"Ben je zeker dat je kaart '{Card.Name}' wil verwijderen?", "Ja", "Nee");

        if (confirm)
        {
            await _databaseService.DeleteCardAsync(Card);
            await _navigationService.GoBack();
        }
    }

    [RelayCommand]
    public async Task ToggleFavoriteAsync()
    {
        if (Card == null) return;

        Card.IsFavorite = !Card.IsFavorite;
        await _databaseService.SaveCardAsync(Card);
        OnPropertyChanged(nameof(Card));
    }

    [RelayCommand]
    public Task GoBackAsync()
    {
        // Always restore original brightness when leaving card view
        if (isBrightnessControlSupported && (Card?.HasBarcode ?? false))
        {
            _brightnessService.RestoreOriginalBrightness();
        }

        return _navigationService.GoBack();
    }
}
