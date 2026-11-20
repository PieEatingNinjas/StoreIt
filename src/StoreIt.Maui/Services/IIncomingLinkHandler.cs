namespace StoreIt.Maui.Services;

public interface IIncomingLinkHandler
{
    event EventHandler? CardsImported;

    Task HandleAsync(Uri link, CancellationToken cancellationToken = default);
}
