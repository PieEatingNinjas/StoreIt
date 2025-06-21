namespace StoreIt.Maui.Services;

public class UserPreferencesService : IUserPreferencesService
{
    private const string ThemePreferenceKey = "SelectedTheme";
    private const string HintsEnabledKey = "HintsEnabled";
    private readonly IPreferences _preferences;

    public UserPreferencesService(IPreferences preferences)
    {
        _preferences = preferences;
    }

    public ThemeOption GetSelectedTheme()
    {
        var savedTheme = GetString(ThemePreferenceKey, ThemeOption.System.ToString());
        
        if (Enum.TryParse<ThemeOption>(savedTheme, out var theme))
        {
            return theme;
        }
        
        return ThemeOption.System;
    }

    public void SetSelectedTheme(ThemeOption theme)
    {
        SetString(ThemePreferenceKey, theme.ToString());
    }

    public string GetString(string key, string defaultValue = "")
    {
        return _preferences.Get(key, defaultValue);
    }

    public void SetString(string key, string value)
    {
        _preferences.Set(key, value);
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        return _preferences.Get(key, defaultValue);
    }

    public void SetBool(string key, bool value)
    {
        _preferences.Set(key, value);
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        return _preferences.Get(key, defaultValue);
    }

    public void SetInt(string key, int value)
    {
        _preferences.Set(key, value);
    }

    public void Remove(string key)
    {
        _preferences.Remove(key);
    }

    public bool ContainsKey(string key)
    {
        return _preferences.ContainsKey(key);
    }

    public void Clear()
    {
        _preferences.Clear();
    }
    
    public bool GetHintsEnabled()
    {
        return GetBool(HintsEnabledKey, true); // Default to true (hints enabled)
    }
    
    public void SetHintsEnabled(bool enabled)
    {
        SetBool(HintsEnabledKey, enabled);
    }
}
