using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Maui.Models;
using StoreIt.Maui.Services;
using StoreIt.Services;

namespace StoreIt.Maui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const string CardSortModePreferenceKey = "CardSortMode";
    private const string SortLastAccessedLabel = "Laatst gebruikt";
    private const string SortNameAscendingLabel = "Naam (A-Z)";
    private const string SortNameDescendingLabel = "Naam (Z-A)";

    private readonly DatabaseService _databaseService;
    private readonly IAppNavigationService _appNavigationService;
    private readonly IDialogService _dialogService;
    private readonly IUserPreferencesService _userPreferences;

    [ObservableProperty]
    private List<CustomerCard> cards = [];

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isLoading = true;

    private CardSortMode _sortMode;

    public MainViewModel(DatabaseService databaseService,
        IAppNavigationService appNavigationService, IDialogService dialogService,
        IUserPreferencesService userPreferences)
    {
        _databaseService = databaseService;
        _appNavigationService = appNavigationService;
        _dialogService = dialogService;
        _userPreferences = userPreferences;

        var storedMode = _userPreferences.GetString(CardSortModePreferenceKey, nameof(CardSortMode.LastAccessed));
        if (!Enum.TryParse(storedMode, ignoreCase: true, out CardSortMode parsedMode) ||
            !Enum.IsDefined(typeof(CardSortMode), parsedMode))
        {
            parsedMode = CardSortMode.LastAccessed;
        }

        _sortMode = parsedMode;
    }

    [RelayCommand]
    private async Task LoadCardsAsync()
    {
        try
        {
            Cards = await _databaseService.GetCardsAsync(_sortMode);
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Ooops...", $"Jouw items konden niet geladen worden: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private Task AddCardAsync() => _appNavigationService.NavigateToAddCardPage();

    [RelayCommand]
    private async Task ViewCardAsync(CustomerCard card)
    {
        if (card is null) return;

        await _databaseService.UpdateLastUsedAsync(card.Id);
        await _appNavigationService.NavigateToViewCardPage(card.Id);
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(CustomerCard card)
    {
        if (card is null) return;

        card.IsFavorite = !card.IsFavorite;
        await _databaseService.SaveCardAsync(card);
        await LoadCardsAsync();
    }

    private async Task SortListAsync(CardSortMode selectedMode)
    {
        if (_sortMode == selectedMode)
        {
            return;
        }

        _sortMode = selectedMode;
        _userPreferences.SetString(CardSortModePreferenceKey, selectedMode.ToString());
        await LoadCardsAsync();
    }

    [RelayCommand]
    private async Task OpenSortPickerAsync()
    {
        var selection = await _dialogService.DisplayActionSheet(
            "Sorteren op",
            "Annuleren",
            SortLastAccessedLabel,
            SortNameAscendingLabel,
            SortNameDescendingLabel);

        switch (selection)
        {
            case SortLastAccessedLabel:
                await SortListAsync(CardSortMode.LastAccessed);
                break;
            case SortNameAscendingLabel:
                await SortListAsync(CardSortMode.NameAscending);
                break;
            case SortNameDescendingLabel:
                await SortListAsync(CardSortMode.NameDescending);
                break;
        }
    }
}