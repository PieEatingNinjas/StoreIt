namespace StoreIt.Maui.Services;

public enum ThemeOption
{
    System,  // Follow device setting
    Light,   // Always light
    Dark     // Always dark
}

public interface IThemeService
{
    ThemeOption GetCurrentTheme();
    void SetTheme(ThemeOption theme);
    AppTheme GetAppTheme();
    void InitializeTheme();
    event EventHandler<ThemeOption> ThemeChanged;
}
