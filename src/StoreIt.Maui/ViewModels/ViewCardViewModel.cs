using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Maui.Models;
using StoreIt.Maui.Services;
using StoreIt.Services;

namespace StoreIt.Maui.ViewModels;

public partial class ViewCardViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly IPlatformBrightnessService _brightnessService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IAppNavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly IBiometricService _biometricService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCardCommand), nameof(DeleteCardCommand), nameof(ToggleFavoriteCommand))]
    private CustomerCard? card;

    private readonly bool isBrightnessControlSupported;
    private bool _brightnessSaved;

    [ObservableProperty]
    private bool showZoomHint;

    [ObservableProperty]
    private bool showCopyHint;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCardCommand), nameof(DeleteCardCommand), nameof(ToggleFavoriteCommand))]
    private bool isAuthenticating = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCardCommand), nameof(DeleteCardCommand), nameof(ToggleFavoriteCommand))]
    private bool isLoading = true;

    public bool CanManageCard => Card is not null && !IsLoading && !IsAuthenticating;

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

    public Task InitializeAsync(int id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        return LoadCardWithBiometricAsync(id, cancellationToken);
    }

    private async Task LoadCardWithBiometricAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            IsAuthenticating = false;
            IsLoading = true;

            // Load the card first to check whether it is a private card
            var card = await _databaseService.GetCardAsync(id);
            cancellationToken.ThrowIfCancellationRequested();

            if (card == null)
            {
                IsAuthenticating = false;
                await _dialogService.DisplayAlert("Fout", "Item niet gevonden.", "OK");
                cancellationToken.ThrowIfCancellationRequested();
                await _navigationService.GoBack();
                return;
            }

            // Only require authentication for private cards
            if (card.IsPrivate)
            {
                IsAuthenticating = true;

                var isBiometricAvailable = await _biometricService.IsAvailableAsync();
                cancellationToken.ThrowIfCancellationRequested();

                if (isBiometricAvailable)
                {
                    // Require biometric authentication for private cards
                    var authResult = await _biometricService.AuthenticateAsync("Authenticeer om dit item te bekijken");
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!authResult)
                    {
                        // Authentication failed — navigate back
                        await _dialogService.DisplayAlert("Authenticatie vereist",
                            "Je moet je authenticeren om je item te kunnen bekijken.", "OK");
                        cancellationToken.ThrowIfCancellationRequested();
                        await _navigationService.GoBack();
                        return;
                    }
                }
                else
                {
                    // Biometrics unavailable but card is private
                    await _dialogService.DisplayAlert("Beveiliging niet beschikbaar",
                        "Dit item is beveiligd, maar je apparaat ondersteunt dit niet. Stel biometrische beveiliging (opnieuw) in op je apparaat.",
                        "OK");
                    cancellationToken.ThrowIfCancellationRequested();
                    await _navigationService.GoBack();
                    return;
                }
            }

            // Authentication succeeded or not required — load the card
            await SetCardAsync(card, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // The page was left while loading or authenticating.
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Fout", $"Er ging iets mis bij het authenticeren: {ex.Message}", "OK");
            if (!cancellationToken.IsCancellationRequested)
            {
                await _navigationService.GoBack();
            }
        }
        finally
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                IsAuthenticating = false;
                IsLoading = false;
            }
        }
    }

    private async Task SetCardAsync(CustomerCard? card, CancellationToken cancellationToken)
    {
        if (card is not null)
        {
            await _databaseService.UpdateLastUsedAsync(card.Id);
            cancellationToken.ThrowIfCancellationRequested();
            Card = card;

            if (isBrightnessControlSupported)
            {
                // Save original brightness when entering card view
                _brightnessService.SaveOriginalBrightness();
                _brightnessSaved = true;

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

    [RelayCommand(CanExecute = nameof(CanManageCard))]
    private async Task EditCardAsync()
    {
        try
        {
            await _navigationService.NavigateToEditCardPage(Card?.Id ?? 0);
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Fout", $"Item kon niet bewerkt worden: {ex.Message}", "OK");
        }
    }

    [RelayCommand(CanExecute = nameof(CanManageCard))]
    private async Task DeleteCardAsync()
    {
        var selectedCard = Card;
        if (selectedCard == null)
        {
            return;
        }

        bool confirm = await _dialogService.DisplayAlert("Ben je zeker?",
            $"Ben je zeker dat je item '{selectedCard.Name}' wil verwijderen?", "Ja", "Nee");

        if (confirm)
        {
            try
            {
                await _databaseService.DeleteCardAsync(selectedCard);
                await _navigationService.GoBack();
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Fout", $"Item kon niet verwijderd worden: {ex.Message}", "OK");
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanManageCard))]
    private async Task ToggleFavoriteAsync()
    {
        var selectedCard = Card;
        if (selectedCard == null)
        {
            return;
        }

        var wasFavorite = selectedCard.IsFavorite;
        try
        {
            selectedCard.IsFavorite = !wasFavorite;
            await _databaseService.SaveCardAsync(selectedCard);
            OnPropertyChanged(nameof(Card));
        }
        catch (Exception ex)
        {
            selectedCard.IsFavorite = wasFavorite;
            OnPropertyChanged(nameof(Card));
            await _dialogService.DisplayAlert("Fout", $"Favoriet kon niet gewijzigd worden: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private Task GoBackAsync() => _navigationService.GoBack();

    public void Deactivate()
    {
        if (_brightnessSaved)
        {
            _brightnessService.RestoreOriginalBrightness();
            _brightnessSaved = false;
        }

        Card = null;
        IsLoading = true;
        IsAuthenticating = false;
        ShowZoomHint = false;
        ShowCopyHint = false;
    }
}
