namespace StoreIt.Maui.Services;

public interface IPlatformBrightnessService
{
    /// <summary>
    /// Gets the current system brightness level (0.0 to 1.0)
    /// </summary>
    float GetSystemBrightness();

    /// <summary>
    /// Sets the system brightness level (0.0 to 1.0)
    /// </summary>
    /// <param name="brightness">Brightness level between 0.0 and 1.0</param>
    void SetSystemBrightness(float brightness);

    /// <summary>
    /// Restores the original brightness level
    /// </summary>
    void RestoreOriginalBrightness();

    /// <summary>
    /// Saves the current brightness as the original level before making changes
    /// </summary>
    void SaveOriginalBrightness();

    /// <summary>
    /// Gets whether brightness control is supported on this platform
    /// </summary>
    bool IsBrightnessControlSupported { get; }
}
