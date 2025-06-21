using StoreIt.Maui.ViewModels;

namespace StoreIt.Maui.Views;

public partial class ViewCardPage : ContentPage
{
    // Zoom sizes - cycle through these on tap
    private readonly (double width, double height)[] _zoomSizes = new[]
    {
        (250.0, 125.0),  // A - Normal size
        (350.0, 175.0),  // B - Large size  
        (150.0, 75.0)    // C - Small size
    };

    private int _currentZoomIndex = 0;
    private bool _shouldReloadCard = false;

    private readonly ViewCardViewModel ViewModel;

    public ViewCardPage(ViewCardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Set initial barcode size (A - Normal)
        SetZoomSize(0);

        // Keep screen on when viewing barcode
        DeviceDisplay.KeepScreenOn = true;

        if (_shouldReloadCard)
        {
            _shouldReloadCard = false;
            ViewModel.ReloadCardCommand.Execute(null);
        }
        // else if(ViewModel.Card?.HasBarcode ?? false)
        // {
        //     // Show zoom hint briefly, then fade it out
        //     ShowZoomHintBriefly();
        // }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Allow screen to turn off when leaving
        DeviceDisplay.KeepScreenOn = false;
    }

    private void OnBarcodeTapped(object sender, TappedEventArgs e)
    {
        // Cycle to next zoom size on double tap
        CycleZoom();

        // Hide the hint after first use
       // HideZoomHint();
    }

    // Zoom hint management
    // private async void ShowZoomHintBriefly()
    // {
    //     if (zoomHint != null)
    //     {
    //         zoomHint.Opacity = 1;
    //         zoomHint.IsVisible = true;

    //         // Wait 3 seconds, then fade out
    //         await Task.Delay(3000);
    //         await FadeOutZoomHint();
    //     }
    // }

    // private async void HideZoomHint()
    // {
    //     await FadeOutZoomHint();
    // }

    // private async Task FadeOutZoomHint()
    // {
    //     if (zoomHint != null && zoomHint.IsVisible)
    //     {
    //         await zoomHint.FadeTo(0, 500); // 500ms fade out
    //         zoomHint.IsVisible = false;
    //     }
    // }

    // Zoom methods
    private void CycleZoom()
    {
        _currentZoomIndex = (_currentZoomIndex + 1) % _zoomSizes.Length;
        SetZoomSize(_currentZoomIndex);
    }

    private void SetZoomSize(int index)
    {
        _currentZoomIndex = index;
        var (width, height) = _zoomSizes[index];

        barcodeGenerator.WidthRequest = width;
        barcodeGenerator.HeightRequest = height;
    }

    private async void OnMoreOptionsClicked(object sender, EventArgs e)
    {
        string action = await DisplayActionSheet($"Opties", "Annuleren", "Verwijderen", "Bewerken");
        switch (action)
        {
            case "Bewerken":
                _shouldReloadCard = true;
                await Shell.Current.GoToAsync($"addcard?cardId={ViewModel.CardId}");
                break;
            case "Verwijderen":
                await ViewModel.DeleteCardCommand.ExecuteAsync(null);
                break;
            case "Favoriet":
                ViewModel.ToggleFavoriteCommand.Execute(null);
                break;
        }
    }
}