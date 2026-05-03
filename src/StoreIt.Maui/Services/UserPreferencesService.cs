namespace StoreIt.Maui.Services;

public class UserPreferencesService : IUserPreferencesService
{
    private const string ThemePreferenceKey = "SelectedTheme";
    private const string HintsEnabledKey = "HintsEnabled";
    private const string WhatsNewVersionKey = "WhatsNewVersion";
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
        => SetString(ThemePreferenceKey, theme.ToString());

    public string GetString(string key, string defaultValue = "")
        => _preferences.Get(key, defaultValue);

    public void SetString(string key, string value)
        => _preferences.Set(key, value);

    public bool GetBool(string key, bool defaultValue = false)
        => _preferences.Get(key, defaultValue);

    public void SetBool(string key, bool value)
        => _preferences.Set(key, value);

    public int GetInt(string key, int defaultValue = 0)
        => _preferences.Get(key, defaultValue);

    public void SetInt(string key, int value)
        => _preferences.Set(key, value);

    public void Remove(string key)
        => _preferences.Remove(key);

    public bool ContainsKey(string key)
        => _preferences.ContainsKey(key);


    public void Clear()
        => _preferences.Clear();

    public bool GetHintsEnabled()
        => GetBool(HintsEnabledKey, true); // Default to true (hints enabled)

    public void SetHintsEnabled(bool enabled)
        => SetBool(HintsEnabledKey, enabled);

    public int? GetLastViewedWhatsNewId()
    {
        var value = _preferences.Get(WhatsNewVersionKey, int.MinValue);
        return value == int.MinValue ? null : value;
    }

    public void SetLastViewedWhatsNewId(int id)
        => _preferences.Set(WhatsNewVersionKey, id);
}
