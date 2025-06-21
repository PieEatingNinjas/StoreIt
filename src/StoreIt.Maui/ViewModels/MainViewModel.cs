using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using StoreIt.Maui.Models;
using StoreIt.Maui.Services;

namespace StoreIt.Maui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private List<CustomerCard> cards = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isLoading = true;

    public MainViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    public async Task LoadCardsAsync()
    {
        try
        {
            IsRefreshing = true;
            var cards = await _databaseService.GetCardsAsync();
            Cards = [.. cards];
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Unable to load cards: {ex.Message}", "OK");
        }
        finally
        {
            IsRefreshing = false;
            IsLoading = false;
        }
    }

    // [RelayCommand]
    // public async Task SearchCardsAsync()
    // {
    //     try
    //     {
    //         if (string.IsNullOrWhiteSpace(SearchText))
    //         {
    //             await LoadCardsAsync();
    //             return;
    //         }

    //         var cards = await _databaseService.SearchCardsAsync(SearchText);
    //         Cards.Clear();
    //         foreach (var card in cards)
    //         {
    //             Cards.Add(card);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         await Shell.Current.DisplayAlert("Error", $"Unable to search cards: {ex.Message}", "OK");
    //     }
    // }

    [RelayCommand]
    public async Task AddCardAsync()
    {
        await Shell.Current.GoToAsync("addcard");
    }

    [RelayCommand]
    public async Task ViewCardAsync(CustomerCard card)
    {
        if (card == null) return;
        
        await _databaseService.UpdateLastUsedAsync(card.Id);
        await Shell.Current.GoToAsync($"viewcard?cardId={card.Id}");
    }

    [RelayCommand]
    public async Task DeleteCardAsync(CustomerCard card)
    {
        if (card == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Delete Card", 
            $"Are you sure you want to delete '{card.Name}'?", "Yes", "No");
        
        if (confirm)
        {
            await _databaseService.DeleteCardAsync(card);
            Cards.Remove(card);
        }
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
