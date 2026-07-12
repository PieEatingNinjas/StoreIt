using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Maui.Models;
using StoreIt.Maui.Services;
using StoreIt.Maui.Sorting;
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
    private List<CustomerCard> cards = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isLoading = true;

    [ObservableProperty]
    private CardSortMode sortMode;

    public MainViewModel(DatabaseService databaseService,
        IAppNavigationService appNavigationService, IDialogService dialogService,
        IUserPreferencesService userPreferences)
    {
        _databaseService = databaseService;
        _appNavigationService = appNavigationService;
        _dialogService = dialogService;
        _userPreferences = userPreferences;

        var storedMode = _userPreferences.GetString(CardSortModePreferenceKey, nameof(CardSortMode.LastAccessed));
        SortMode = Enum.TryParse(storedMode, out CardSortMode parsedMode)
            ? parsedMode
            : CardSortMode.LastAccessed;
    }

    partial void OnSortModeChanged(CardSortMode value)
    {
        _userPreferences.SetString(CardSortModePreferenceKey, value.ToString());
        Cards = [.. CardSorter.Sort(Cards, value)];
    }

    [RelayCommand]
    private async Task LoadCardsAsync()
    {
        try
        {
            var cards = await _databaseService.GetCardsAsync();
            Cards = [.. CardSorter.Sort(cards, SortMode)];
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
                SortMode = CardSortMode.LastAccessed;
                break;
            case SortNameAscendingLabel:
                SortMode = CardSortMode.NameAscending;
                break;
            case SortNameDescendingLabel:
                SortMode = CardSortMode.NameDescending;
                break;
        }
    }
}