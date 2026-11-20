namespace StoreIt.Sharing.Services;

public sealed class UnsupportedCardVersionException : Exception
{
    public UnsupportedCardVersionException(int version)
        : base($"Sharing format version {version} is not supported.")
    {
    }
}
