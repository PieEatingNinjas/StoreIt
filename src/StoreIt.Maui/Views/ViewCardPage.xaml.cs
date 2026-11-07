using StoreIt.Maui.ViewModels;

namespace StoreIt.Maui.Views;

public partial class ViewCardPage : ContentPage
{
    // Zoom sizes - cycle through these on tap
    private readonly (double width, double height)[] _zoomSizes =
    [
        (250.0, 125.0),
        (350.0, 175.0),
        (150.0, 75.0)
    ];

    private int _currentZoomIndex = 0;
    private bool _shouldReloadCard = false;

    private readonly ViewCardViewModel ViewModel;

    public ViewCardPage(ViewCardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel = viewModel;
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = true });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        SetZoomSize(0);

        // Keep screen on when viewing barcode
        DeviceDisplay.KeepScreenOn = true;

        if (_shouldReloadCard)
        {
            _shouldReloadCard = false;
            ViewModel.ReloadCardCommand.Execute(null);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Allow screen to turn off when leaving
        DeviceDisplay.KeepScreenOn = false;
    }

    private void OnCodeTapped(object sender, TappedEventArgs e)
    {
        if (ViewModel.Card?.HasBarcode ?? false)
        {
            CycleZoom();
        }
        else if (ViewModel.Card?.HasCustomCode ?? false)
        {
            Clipboard.SetTextAsync(ViewModel.Card.CustomCode);
            copyHint.IsShowing = true;
        }
    }

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

    private void OnEditCardClicked(object sender, EventArgs e)
    {
        _shouldReloadCard = true;
    }
}
