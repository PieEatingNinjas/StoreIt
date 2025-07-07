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
    public async Task LoadCardsAsync()
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
    public Task AddCardAsync() => _appNavigationService.NavigateToAddCardPage();

    [RelayCommand]
    public async Task ViewCardAsync(CustomerCard card)
    {
        if (card == null) return;
        
        await _databaseService.UpdateLastUsedAsync(card.Id);
        await _appNavigationService.NavigateToViewCardPage(card.Id);
    }

    [RelayCommand]
    public async Task ToggleFavoriteAsync(CustomerCard card)
    {
        if (card == null) return;

        card.IsFavorite = !card.IsFavorite;
        await _databaseService.SaveCardAsync(card);
        await LoadCardsAsync();
    }
}
