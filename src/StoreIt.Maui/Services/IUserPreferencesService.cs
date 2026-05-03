namespace StoreIt.Maui.Services;

public interface IUserPreferencesService
{
    /// <summary>
    /// Gets the selected theme from user preferences
    /// </summary>
    /// <returns>The selected theme option</returns>
    ThemeOption GetSelectedTheme();

    /// <summary>
    /// Sets the selected theme in user preferences
    /// </summary>
    /// <param name="theme">The theme to save</param>
    void SetSelectedTheme(ThemeOption theme);

    /// <summary>
    /// Gets a string preference value
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <param name="defaultValue">Default value if key doesn't exist</param>
    /// <returns>The preference value</returns>
    string GetString(string key, string defaultValue = "");

    /// <summary>
    /// Sets a string preference value
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <param name="value">The value to save</param>
    void SetString(string key, string value);

    /// <summary>
    /// Gets a boolean preference value
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <param name="defaultValue">Default value if key doesn't exist</param>
    /// <returns>The preference value</returns>
    bool GetBool(string key, bool defaultValue = false);

    /// <summary>
    /// Sets a boolean preference value
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <param name="value">The value to save</param>
    void SetBool(string key, bool value);

    /// <summary>
    /// Gets an integer preference value
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <param name="defaultValue">Default value if key doesn't exist</param>
    /// <returns>The preference value</returns>
    int GetInt(string key, int defaultValue = 0);

    /// <summary>
    /// Sets an integer preference value
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <param name="value">The value to save</param>
    void SetInt(string key, int value);

    /// <summary>
    /// Removes a preference key
    /// </summary>
    /// <param name="key">The preference key to remove</param>
    void Remove(string key);

    /// <summary>
    /// Checks if a preference key exists
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <returns>True if the key exists</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// Clears all preferences
    /// </summary>
    void Clear();

    /// <summary>
    /// Gets whether hints are enabled
    /// </summary>
    /// <returns>True if hints are enabled, default is true</returns>
    bool GetHintsEnabled();

    /// <summary>
    /// Sets whether hints are enabled
    /// </summary>
    /// <param name="enabled">True to enable hints, false to disable</param>
    void SetHintsEnabled(bool enabled);

    /// <summary>
    /// Gets the last viewed WhatsNew ID
    /// </summary>
    /// <returns>The last viewed WhatsNew ID, or null if not set</returns>
    int? GetLastViewedWhatsNewId();

    /// <summary>
    /// Sets the last viewed WhatsNew ID
    /// </summary>
    /// <param name="id">The ID to set</param>
    void SetLastViewedWhatsNewId(int id);
}