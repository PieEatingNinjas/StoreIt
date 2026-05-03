using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Maui.Models;
using StoreIt.Maui.Services;
using StoreIt.Services;

namespace StoreIt.Maui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly IAppNavigationService _appNavigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private List<CustomerCard> cards = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isLoading = true;

    public MainViewModel(DatabaseService databaseService,
        IAppNavigationService appNavigationService, IDialogService dialogService)
    {
        _databaseService = databaseService;
        _appNavigationService = appNavigationService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async Task LoadCardsAsync()
    {
        try
        {
            var cards = await _databaseService.GetCardsAsync();
            Cards = [.. cards];
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
}