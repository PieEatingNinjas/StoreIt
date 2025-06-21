using CommunityToolkit.Maui.Behaviors;
using StoreIt.Maui.ViewModels;

namespace StoreIt.Maui.Views;

public partial class MainPage : ContentPage
{
    public MainViewModel ViewModel{ get; }
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MainViewModel vm)
        {
            await vm.LoadCardsCommand.ExecuteAsync(null);
        }
    }

    private async void OnMoreOptionsClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Models.CustomerCard card)
        {
            string action = await DisplayActionSheet($"Opties voor {card.Name}", "Annuleren", "Verwijderen", "Bewerken", "Favoriet");
            
            if (BindingContext is MainViewModel vm)
            {
                switch (action)
                {
                    case "Bewerken":
                        await Shell.Current.GoToAsync($"addcard?cardId={card.Id}");
                        break;
                    case "Verwijderen":
                        await vm.DeleteCardCommand.ExecuteAsync(card);
                        break;
                    case "Favoriet":
                        await vm.ToggleFavoriteCommand.ExecuteAsync(card);
                        break;
                }
            }
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            element.Animate("OpacityAnimation",
                (d) => element.Opacity = d, start: 1, end: 0.75, length: 250, easing: Easing.CubicInOut, finished: (v, c) =>
                {
                    element.Opacity = 1; // Reset opacity after animation
                });
        }
    }
}
