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
    private readonly IBiometricService _biometricService;

    [ObservableProperty]
    private CustomerCard? card;

    [ObservableProperty]
    private int cardId;

    private bool isBrightnessControlSupported;

    [ObservableProperty]
    private bool showZoomHint;

    [ObservableProperty]
    private bool showCopyHint;

    [ObservableProperty]
    private bool isAuthenticating = true;

    [ObservableProperty]
    private bool isLoading = true;

    public ViewCardViewModel(DatabaseService databaseService,
        IPlatformBrightnessService brightnessService,
        IUserPreferencesService userPreferencesService,
        IAppNavigationService appNavigationService,
        IDialogService dialogService,
        IBiometricService biometricService)
    {
        _databaseService = databaseService;
        _brightnessService = brightnessService;
        _userPreferencesService = userPreferencesService;
        _navigationService = appNavigationService;
        _dialogService = dialogService;
        _biometricService = biometricService;
        isBrightnessControlSupported = _brightnessService.IsBrightnessControlSupported;
    }

    partial void OnCardIdChanged(int value)
    {
        if (value > 0)
        {
            _ = LoadCardWithBiometricAsync(value);
        }
    }

    private async Task LoadCardWithBiometricAsync(int id)
    {
        try
        {
            IsAuthenticating = false;
            IsLoading = true;

            // Eerst de kaart laden om te controleren of het een privé kaart is
            var card = await _databaseService.GetCardAsync(id);

            if (card == null)
            {
                IsAuthenticating = false;
                await _dialogService.DisplayAlert("Fout", "Item niet gevonden.", "OK");
                await _navigationService.GoBack();
                return;
            }

            // Alleen authenticatie vereisen voor privé kaarten
            if (card.IsPrivate)
            {
                IsAuthenticating = false;

                var isBiometricAvailable = await _biometricService.IsAvailableAsync();

                if (isBiometricAvailable)
                {
                    IsAuthenticating = true;
                    // Biometrische authenticatie vereisen voor privé kaarten
                    var authResult = await _biometricService.AuthenticateAsync("Authenticeer om je item te bekijken");

                    if (!authResult)
                    {
                        // Authenticatie mislukt - ga terug
                        await _dialogService.DisplayAlert("Authenticatie vereist",
                            "Je moet je authenticeren om je item te kunnen bekijken.", "OK");
                        await _navigationService.GoBack();
                        return;
                    }
                }
                else
                {
                    // Biometrie niet beschikbaar maar kaart is privé
                    IsAuthenticating = false;
                    await _dialogService.DisplayAlert("Beveiliging niet beschikbaar",
                        "Dit item is beveiligd, maar je apparaat ondersteunt dit niet. Stel biometrische beveiliging in op je apparaat.", "OK");
                    await _navigationService.GoBack();
                    return;
                }
            }

            // Authenticatie succesvol of niet nodig - laad de kaart volledig
            await SetCardAsync(card);
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Fout", $"Er ging iets mis bij het authenticeren: {ex.Message}", "OK");
            await _navigationService.GoBack();
        }
        finally
        {
            IsAuthenticating = false;
            IsLoading = false;
        }
    }

    private async Task SetCardAsync(CustomerCard? card)
    {
        Card = card;

        if (card is not null)
        {
            await _databaseService.UpdateLastUsedAsync(card.Id);

            if (isBrightnessControlSupported)
            {
                // Save original brightness when entering card view
                _brightnessService.SaveOriginalBrightness();

                if (card.HasBarcode)
                {
                    // For barcode cards: automatically set max brightness and hide slider
                    _brightnessService.SetSystemBrightness(1.0f);
                }
            }
            LoadHints();
        }
        else
        {
            await _dialogService.DisplayAlert("Fout", "Item niet gevonden.", "OK");
            await _navigationService.GoBack();
        }
    }

    private async Task LoadCardAsync(int id)
    {
        try
        {
            var card = await _databaseService.GetCardAsync(id);
            await SetCardAsync(card);
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Ooops...", $"Item kon niet geladen worden: {ex.Message}", "OK");
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
            $"Ben je zeker dat je item '{Card.Name}' wil verwijderen?", "Ja", "Nee");

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
