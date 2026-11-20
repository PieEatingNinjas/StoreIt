using Microsoft.Maui.ApplicationModel.DataTransfer;
using StoreIt.Maui.Models;
using StoreIt.Sharing.Models;
using StoreIt.Sharing.Services;

namespace StoreIt.Maui.Services;

public sealed class CardShareService(ICardSerializer serializer, IShare share) : ICardShareService
{
    private const string DownloadUrl = "https://pieeatingninjas.github.io/StoreIt/";

    public async Task ShareAsync(CustomerCard card, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(card);
        cancellationToken.ThrowIfCancellationRequested();
        var link = serializer.CreateLink(new CardShareData(
            card.Name,
            card.Description,
            card.BarcodeData,
            card.BarcodeFormat,
            card.CustomCode,
            card.Color,
            card.IsFavorite,
            card.IsPrivate));
        await share.RequestAsync(new ShareTextRequest
        {
            Title = "StoreIt-kaart delen",
            Text = $"Ik deel de StoreIt-kaart \"{card.Name}\" met je.\n\n" +
                   $"{link.AbsoluteUri}\n\n" +
                   $"StoreIt nog niet geïnstalleerd? Download de app via {DownloadUrl}",
            Uri = link.AbsoluteUri
        });
        cancellationToken.ThrowIfCancellationRequested();
    }
}
