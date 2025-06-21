namespace StoreIt.Maui.Services;

public class ThemeService : IThemeService
{
    private readonly IUserPreferencesService _userPreferencesService;
    
    public event EventHandler<ThemeOption>? ThemeChanged;

    public ThemeService(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    public ThemeOption GetCurrentTheme()
    {
        return _userPreferencesService.GetSelectedTheme();
    }

    public void SetTheme(ThemeOption theme)
    {
        // Save preference
        _userPreferencesService.SetSelectedTheme(theme);
        
        // Apply theme to application
        ApplyTheme(theme);
        
        // Notify listeners
        ThemeChanged?.Invoke(this, theme);
    }

    public AppTheme GetAppTheme()
    {
        var currentTheme = GetCurrentTheme();
        
        return currentTheme switch
        {
            ThemeOption.Light => AppTheme.Light,
            ThemeOption.Dark => AppTheme.Dark,
            ThemeOption.System => AppTheme.Unspecified, // Follow system
            _ => AppTheme.Unspecified
        };
    }

    public void InitializeTheme()
    {
        var currentTheme = GetCurrentTheme();
        ApplyTheme(currentTheme);
    }

    private void ApplyTheme(ThemeOption theme)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = GetAppTheme();
        }
    }
}
