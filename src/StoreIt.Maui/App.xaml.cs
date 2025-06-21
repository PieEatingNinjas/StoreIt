using StoreIt.Maui.Services;

namespace StoreIt.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());
		
		// Initialize theme after the window is created
		window.Created += (s, e) => InitializeTheme();
		
		return window;
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