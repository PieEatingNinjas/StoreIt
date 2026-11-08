using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;
using MauiApplication = Microsoft.Maui.Controls.Application;

namespace StoreIt.Maui;

[Activity(Theme = "@style/splashscreen", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        UpdateSystemBars();
    }

    protected override void OnResume()
    {
        base.OnResume();

        if (MauiApplication.Current is { } application)
        {
            application.RequestedThemeChanged += OnRequestedThemeChanged;
        }

        UpdateSystemBars();
    }

    protected override void OnPause()
    {
        if (MauiApplication.Current is { } application)
        {
            application.RequestedThemeChanged -= OnRequestedThemeChanged;
        }

        base.OnPause();
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
        => UpdateSystemBars();

    public override void OnWindowFocusChanged(bool hasFocus)
    {
        base.OnWindowFocusChanged(hasFocus);

        if (hasFocus)
        {
            UpdateSystemBars();
        }
    }

    private void UpdateSystemBars()
    {
        if (Window is not { } window)
        {
            return;
        }

        var isLight = MauiApplication.Current?.RequestedTheme != AppTheme.Dark;
        var backgroundColor = isLight
            ? Android.Graphics.Color.White
            : new Android.Graphics.Color(0x1F, 0x1F, 0x1F);
        var controller = WindowCompat.GetInsetsController(window, window.DecorView)
            ?? throw new InvalidOperationException("Android system bar controller is unavailable.");
        controller.AppearanceLightStatusBars = isLight;
        controller.AppearanceLightNavigationBars = isLight;

        if (OperatingSystem.IsAndroidVersionAtLeast(35))
        {
            // Android 15+ draws transparent system bars over the window background.
            window.DecorView.SetBackgroundColor(backgroundColor);
        }
        else
        {
#pragma warning disable CA1422 // Bar colors remain supported below Android 15.
            window.SetStatusBarColor(backgroundColor);
            window.SetNavigationBarColor(backgroundColor);
#pragma warning restore CA1422
        }
    }
}
