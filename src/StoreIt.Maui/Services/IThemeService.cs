namespace StoreIt.Maui.Services;

public enum ThemeOption
{
    System,  
    Light,   
    Dark     
}

public interface IThemeService
{
    ThemeOption GetCurrentTheme();
    void SetTheme(ThemeOption theme);
    AppTheme GetAppTheme();
    void InitializeTheme();
    // event EventHandler<ThemeOption> ThemeChanged;
}
