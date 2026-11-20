using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using StoreIt.Maui.Services;

namespace StoreIt.Maui;

public partial class App : Application
{
    readonly IWhatsNewService _whatsNewService;
    readonly IIncomingLinkHandler _incomingLinkHandler;
    readonly ILogger<App> _logger;
    readonly ConcurrentQueue<Uri> _pendingLinks = new();
    readonly SemaphoreSlim _linkHandlingLock = new(1, 1);
    bool _startupCompleted;
    bool _windowReady;

    public App(
        IWhatsNewService whatsNewService,
        IIncomingLinkHandler incomingLinkHandler,
        ILogger<App> logger)
    {
        InitializeComponent();
        _whatsNewService = whatsNewService;
        _incomingLinkHandler = incomingLinkHandler;
        _logger = logger;
    }

    protected override async void OnStart()
    {
        try
        {
            await _whatsNewService.ShowLatestWhatsNewAsync();
        }
        catch
        {
            //
        }
        finally
        {
            _startupCompleted = true;
            await DrainPendingLinksAsync();
        }
    }

    protected override async void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);
        _pendingLinks.Enqueue(uri);
        await DrainPendingLinksAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());

        // Initialize theme after the window is created
        window.Created += (s, e) => InitializeTheme();
        window.Activated += OnWindowActivated;

        return window;
    }

    private async void OnWindowActivated(object? sender, EventArgs e)
    {
        _windowReady = true;
        await DrainPendingLinksAsync();
    }

    private async Task DrainPendingLinksAsync()
    {
        if (!_startupCompleted || !_windowReady)
        {
            return;
        }

        await _linkHandlingLock.WaitAsync();
        try
        {
            while (_pendingLinks.TryDequeue(out var link))
            {
                try
                {
                    await _incomingLinkHandler.HandleAsync(link);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not display StoreIt import feedback");
                }
            }
        }
        finally
        {
            _linkHandlingLock.Release();
        }
    }

    private void InitializeTheme()
    {
        try
        {
            // Try to get theme service and initialize theme
            var handler = Windows.FirstOrDefault()?.Handler;
            var themeService = handler?.MauiContext?.Services?.GetService<IThemeService>();
            themeService?.InitializeTheme();
        }
        catch (Exception ex)
        {
            // Fallback - if service not available yet, use system theme
            System.Diagnostics.Debug.WriteLine($"Theme initialization failed: {ex.Message}");
            UserAppTheme = AppTheme.Unspecified;
        }
    }
}
