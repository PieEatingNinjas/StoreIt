using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Maui.Models;
using StoreIt.Maui.Services;

namespace StoreIt.Maui.ViewModels;

[QueryProperty(nameof(CardId), "cardId")]
public partial class ViewCardViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly IPlatformBrightnessService _brightnessService;
    private readonly IUserPreferencesService _userPreferencesService;

    [ObservableProperty]
    private CustomerCard? card;

    [ObservableProperty]
    private int cardId;

    private bool isBrightnessControlSupported;

    [ObservableProperty]
    private bool showZoomHint;

    public ViewCardViewModel(DatabaseService databaseService, IPlatformBrightnessService brightnessService, IUserPreferencesService userPreferencesService)
    {
        _databaseService = databaseService;
        _brightnessService = brightnessService;
        _userPreferencesService = userPreferencesService;
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
                        // BrightnessLevel = 1.0f;
                        
                        // Only show zoom hint if hints are enabled in settings
                        if (_userPreferencesService.GetHintsEnabled())
                        {
                            ShowZoomHint = true;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Unable to load card: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public async Task EditCardAsync()
    {
        if (Card != null)
        {
            await Shell.Current.GoToAsync($"addcard?cardId={Card.Id}");
        }
    }

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

        bool confirm = await Shell.Current.DisplayAlert("Delete Card",
            $"Are you sure you want to delete '{Card.Name}'?", "Yes", "No");

        if (confirm)
        {
            await _databaseService.DeleteCardAsync(Card);
            await Shell.Current.GoToAsync("/main");
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
    public async Task GoBackAsync()
    {
        // Always restore original brightness when leaving card view
        if (isBrightnessControlSupported && (Card?.HasBarcode ?? false))
        {
            _brightnessService.RestoreOriginalBrightness();
        }

        await Shell.Current.GoToAsync("..");
    }
}
