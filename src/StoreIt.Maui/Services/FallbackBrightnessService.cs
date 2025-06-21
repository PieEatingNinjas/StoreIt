namespace StoreIt.Maui.Services;

/// <summary>
/// Fallback brightness service for platforms that don't support brightness control
/// </summary>
public class FallbackBrightnessService : IPlatformBrightnessService
{
    public bool IsBrightnessControlSupported => false;

    public float GetSystemBrightness()
    {
        return 0.8f; // Default value
    }

    public void SetSystemBrightness(float brightness)
    {
        // Not supported on this platform
    }

    public void SaveOriginalBrightness()
    {
        // Not applicable
    }

    public void RestoreOriginalBrightness()
    {
        // Not applicable
    }
}
