using Plugin.Maui.Biometric;

namespace StoreIt.Maui.Services;

public class BiometricService : IBiometricService
{
    private readonly IBiometric _biometric;

    public BiometricService(IBiometric biometric)
    {
        _biometric = biometric;
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            if (!_biometric.IsPlatformSupported)
                return false;

            var status = await _biometric.GetAuthenticationStatusAsync();
            return status == BiometricHwStatus.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AuthenticateAsync(string reason)
    {
        try
        {
            var isAvailable = await IsAvailableAsync();
            if (!isAvailable)
                return false;

            var request = new AuthenticationRequest
            {
                Title = "Biometrische authenticatie",
                Subtitle = reason,
                NegativeText = "Annuleren",
                Description = reason,
                AllowPasswordAuth = false
            };

            var result = await _biometric.AuthenticateAsync(request, CancellationToken.None);
            return result.Status == BiometricResponseStatus.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetAvailableBiometricTypesAsync()
    {
        try
        {
            var types = await _biometric.GetEnrolledBiometricTypesAsync();
            return types.Select(t => t.ToString());
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }
}