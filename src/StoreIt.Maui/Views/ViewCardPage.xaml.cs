using Plugin.Maui.BottomSheet;
using Plugin.Maui.BottomSheet.Navigation;
using StoreIt.Maui.ViewModels;
using StoreIt.Navigation;

namespace StoreIt.Maui.Views;

public partial class ViewCardPage : BottomSheet, INavigationAware
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
    private bool _released;
    private readonly Window _presentationWindow;
    private readonly TaskCompletionSource _dismissed = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public ViewCardViewModel ViewModel { get; }
    public Task Dismissed => _dismissed.Task;

    public ViewCardPage(ViewCardViewModel viewModel)
    {
        InitializeComponent();
        ContainerView.SetBinding(VisualElement.BackgroundProperty,
            static (ViewCardPage sheet) => sheet.Background, source: this);
        BindingContext = ViewModel = viewModel;
        _presentationWindow = Shell.Current.Window
            ?? throw new InvalidOperationException("Het actieve venster kon niet gevonden worden.");
        UpdateSheetHeight();
        _presentationWindow.SizeChanged += OnWindowSizeChanged;
        SetZoomSize(0);
        Closing += OnClosing;
#if IOS
        Opening += OnOpening;
#endif
    }

    public void OnNavigatedTo(IBottomSheetNavigationParameters query)
    {
        if (!query.TryGetValue(NavigationParams.CardId, out var value) || value is not int id || id <= 0)
        {
            throw new ArgumentException("Een geldig kaart-ID is vereist.", nameof(query));
        }

        _cardId = id;
    }

    public Task InitializeAsync()
    {
        if (_released)
        {
            return Task.CompletedTask;
        }

        _loadingCancellation = new CancellationTokenSource();
        DeviceDisplay.KeepScreenOn = true;
        return ViewModel.InitializeAsync(_cardId, _loadingCancellation.Token);
    }

    public void OnNavigatedFrom(IBottomSheetNavigationParameters parameters)
    {
        Release();
        _dismissed.TrySetResult();
    }

    private void OnClosing(object? sender, EventArgs e) => Release();

#if IOS
    private void OnOpening(object? sender, EventArgs e)
    {
        if (Handler is not Plugin.Maui.BottomSheet.Handlers.BottomSheetHandler handler)
        {
            throw new InvalidOperationException("De native kaartweergave kon niet gevonden worden.");
        }

        // BottomSheet 10.0.7 otherwise applies PeekHeight only after the native presentation animation.
        handler.PlatformView.SetPeekHeight();
    }
#endif

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(Background) && Background is SolidColorBrush brush)
        {
            // BottomSheet 10.0.7 maps only this legacy property to the native safe-area background.
            SetValue(VisualElement.BackgroundColorProperty, brush.Color);
        }
    }

    public void Release()
    {
        if (_released)
        {
            return;
        }

        _released = true;
        _loadingCancellation?.Cancel();
        _loadingCancellation?.Dispose();
        _loadingCancellation = null;
        DeviceDisplay.KeepScreenOn = false;
        ViewModel.Deactivate();
        Closing -= OnClosing;
#if IOS
        Opening -= OnOpening;
#endif
        _presentationWindow.SizeChanged -= OnWindowSizeChanged;
    }

    private void OnWindowSizeChanged(object? sender, EventArgs e) => UpdateSheetHeight();

    private void UpdateSheetHeight()
    {
        var height = _presentationWindow.Height * 0.75;
#if IOS
        // The plugin adds the bottom safe area to its native peek detent.
        if (_presentationWindow.Handler?.PlatformView is UIKit.UIWindow nativeWindow)
        {
            height -= nativeWindow.SafeAreaInsets.Bottom;
        }
#endif
        PeekHeight = height;
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
            var scale = Math.Min(1, codeContainer.Width / _zoomSizes.Max(size => size.width));
            width *= scale;
            height *= scale;
        }

        barcodeGenerator.WidthRequest = width;
        barcodeGenerator.HeightRequest = height;
    }

    private void OnCodeContainerSizeChanged(object? sender, EventArgs e) => SetZoomSize(_currentZoomIndex);
}
