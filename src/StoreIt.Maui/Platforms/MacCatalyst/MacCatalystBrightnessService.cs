using StoreIt.Maui.Services;

namespace StoreIt.Maui.Platforms.MacCatalyst;

public class MacCatalystBrightnessService : IPlatformBrightnessService
{
    // macOS doesn't easily allow apps to control system brightness
    // This would require private APIs or system-level permissions
    public bool IsBrightnessControlSupported => false;

    public float GetSystemBrightness()
    {
        return 0.8f; // Default value since we can't read it
    }

    public void SetSystemBrightness(float brightness)
    {
        // Not supported on macOS from app level
        // Could potentially use Core Display APIs but requires entitlements
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
