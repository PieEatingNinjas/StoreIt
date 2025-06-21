using StoreIt.Maui.Services;
using UIKit;

namespace StoreIt.Maui.Platforms.iOS;

public class iOSBrightnessService : IPlatformBrightnessService
{
    private nfloat _originalBrightness = -1f;
    private bool _hasStoredOriginal = false;

    public bool IsBrightnessControlSupported => true;

    public float GetSystemBrightness()
    {
        return (float)UIScreen.MainScreen.Brightness;
    }

    public void SetSystemBrightness(float brightness)
    {
        brightness = Math.Max(0.01f, Math.Min(1f, brightness)); // Ensure valid range
        UIScreen.MainScreen.Brightness = brightness;
    }

    public void SaveOriginalBrightness()
    {
        if (!_hasStoredOriginal)
        {
            _originalBrightness = UIScreen.MainScreen.Brightness;
            _hasStoredOriginal = true;
        }
    }

    public void RestoreOriginalBrightness()
    {
        if (_hasStoredOriginal && _originalBrightness >= 0)
        {
            UIScreen.MainScreen.Brightness = _originalBrightness;
        }
    }
}
