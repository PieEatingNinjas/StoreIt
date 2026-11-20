namespace StoreIt.Sharing.Services;

public sealed class InvalidCardPayloadException : Exception
{
    public InvalidCardPayloadException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
