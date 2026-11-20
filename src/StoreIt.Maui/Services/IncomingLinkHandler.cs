using Microsoft.Extensions.Logging;
using StoreIt.Services;
using StoreIt.Sharing.Services;

namespace StoreIt.Maui.Services;

public sealed class IncomingLinkHandler(
    ICardImportService importService,
    IDialogService dialogService,
    ILogger<IncomingLinkHandler> logger) : IIncomingLinkHandler
{
    public event EventHandler? CardsImported;

    public async Task HandleAsync(Uri link, CancellationToken cancellationToken = default)
    {
        try
        {
            var cards = await importService.ImportAsync(link, cancellationToken);
            CardsImported?.Invoke(this, EventArgs.Empty);
            var message = cards.Count == 1
                ? $"Kaart {cards[0].Name} toegevoegd."
                : $"{cards.Count} kaarten toegevoegd.";
            await dialogService.DisplayAlert("Kaart toegevoegd", message, "OK");
        }
        catch (UnsupportedCardVersionException)
        {
            await dialogService.DisplayAlert(
                "Update vereist",
                "Deze gedeelde kaart gebruikt een nieuwere versie. Update StoreIt om de kaart te importeren.",
                "OK");
        }
        catch (InvalidCardPayloadException ex)
        {
            logger.LogWarning(ex, "Rejected invalid StoreIt import link");
            await dialogService.DisplayAlert(
                "Importeren mislukt",
                "Deze StoreIt-link is ongeldig of beschadigd. Er is niets toegevoegd.",
                "OK");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not import StoreIt card link");
            await dialogService.DisplayAlert(
                "Importeren mislukt",
                "De kaart kon niet worden toegevoegd. Probeer het opnieuw.",
                "OK");
        }
    }
}
