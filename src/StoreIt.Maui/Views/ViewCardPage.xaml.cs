using StoreIt.Maui.ViewModels;
using StoreIt.Navigation;

namespace StoreIt.Maui.Views;

public partial class ViewCardPage : ContentPage, IQueryAttributable
{
    // Zoom sizes - cycle through these on tap
    private readonly (double width, double height)[] _zoomSizes =
    [
        (250.0, 125.0),
        (350.0, 175.0),
        (150.0, 75.0)
    ];

    private int _currentZoomIndex = 0;
    private int _cardId;
    private CancellationTokenSource? _loadingCancellation;
    public ViewCardViewModel ViewModel { get; }

    public ViewCardPage(ViewCardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel = viewModel;
        SetZoomSize(0);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue(NavigationParams.CardId, out var value) || value is not int id || id <= 0)
        {
            throw new ArgumentException("Een geldig kaart-ID is vereist.", nameof(query));
        }

        _cardId = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _loadingCancellation = new CancellationTokenSource();
        DeviceDisplay.KeepScreenOn = true;
        await ViewModel.InitializeAsync(_cardId, _loadingCancellation.Token);
    }

    protected override void OnDisappearing()
    {
        _loadingCancellation?.Cancel();
        _loadingCancellation?.Dispose();
        _loadingCancellation = null;
        DeviceDisplay.KeepScreenOn = false;
        ViewModel.Deactivate();
        base.OnDisappearing();
    }

    private async void OnCodeTapped(object sender, TappedEventArgs e)
    {
        if (ViewModel.Card?.HasBarcode ?? false)
        {
            CycleZoom();
        }
        else if (ViewModel.Card?.HasCustomCode ?? false)
        {
            await Clipboard.SetTextAsync(ViewModel.Card.CustomCode);
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
        if (codeContainer.Width > 0)
        {
            var scale = Math.Min(1, codeContainer.Width / width);
            width *= scale;
            height *= scale;
        }

        barcodeGenerator.WidthRequest = width;
        barcodeGenerator.HeightRequest = height;
    }

    private void OnCodeContainerSizeChanged(object? sender, EventArgs e) => SetZoomSize(_currentZoomIndex);
}
