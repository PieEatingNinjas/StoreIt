namespace StoreIt.Maui.Services;

public interface IBiometricService
{
    /// <summary>
    /// Controleert of biometrische authenticatie beschikbaar is op het apparaat
    /// </summary>
    Task<bool> IsAvailableAsync();

    /// <summary>
    /// Vraagt de gebruiker om biometrische authenticatie
    /// </summary>
    /// <param name="reason">De reden waarom authenticatie nodig is</param>
    /// <returns>True als authenticatie succesvol is, false anders</returns>
    Task<bool> AuthenticateAsync(string reason);

    /// <summary>
    /// Haalt de beschikbare biometrische types op (Face ID, Touch ID, etc.)
    /// </summary>
    Task<IEnumerable<string>> GetAvailableBiometricTypesAsync();
}