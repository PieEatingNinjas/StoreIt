using StoreIt.Maui.Services;

namespace StoreIt.Maui.Platforms.Android;

public class AndroidBrightnessService : IPlatformBrightnessService
{
    private float _originalBrightness = -1f;
    private bool _hasStoredOriginal = false;

    public bool IsBrightnessControlSupported => true;

    public float GetSystemBrightness()
    {
        var activity = Platform.CurrentActivity;
        if (activity?.Window?.Attributes != null)
        {
            var brightness = activity.Window.Attributes.ScreenBrightness;
            // Android returns -1 for system brightness, convert to actual value
            if (brightness < 0)
            {
                // Get system brightness from settings
                try
                {
                    var resolver = activity.ContentResolver;
                    if (resolver != null)
                    {
                        var systemBrightness = global::Android.Provider.Settings.System.GetInt(
                            resolver,
                            global::Android.Provider.Settings.System.ScreenBrightness,
                            255);
                        return systemBrightness / 255f;
                    }
                }
                catch
                {
                    // Fallback to default
                    return 0.5f;
                }
            }

            return Math.Max(0f, Math.Min(1f, brightness));
        }

        return 0.5f; // Default fallback
    }

    public void SetSystemBrightness(float brightness)
    {
        var activity = Platform.CurrentActivity;
        if (activity?.Window?.Attributes != null)
        {
            brightness = Math.Max(0.01f, Math.Min(1f, brightness)); // Ensure valid range

            var layoutParams = activity.Window.Attributes;
            layoutParams.ScreenBrightness = brightness;
            activity.Window.Attributes = layoutParams;
        }
    }

    public void SaveOriginalBrightness()
    {
        if (!_hasStoredOriginal)
        {
            _originalBrightness = GetSystemBrightness();
            _hasStoredOriginal = true;
        }
    }

    public void RestoreOriginalBrightness()
    {
        if (_hasStoredOriginal && _originalBrightness >= 0)
        {
            var activity = Platform.CurrentActivity;
            if (activity?.Window?.Attributes != null)
            {
                var layoutParams = activity.Window.Attributes;
                layoutParams.ScreenBrightness =
                    _originalBrightness >= 0 ? _originalBrightness : -1f; // -1 = system default
                activity.Window.Attributes = layoutParams;
            }
        }
    }
}