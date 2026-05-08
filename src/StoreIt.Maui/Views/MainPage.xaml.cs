using StoreIt.Maui.ViewModels;

namespace StoreIt.Maui.Views;

public partial class MainPage : ContentPage
{
    public MainViewModel ViewModel { get; }

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

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            element.Animate("OpacityAnimation",
                (d) => element.Opacity = d, start: 1, end: 0.75, length: 250, easing: Easing.CubicInOut,
                finished: (v, c) =>
                {
                    element.Opacity = 1; // Reset opacity after animation
                });
        }
    }
}