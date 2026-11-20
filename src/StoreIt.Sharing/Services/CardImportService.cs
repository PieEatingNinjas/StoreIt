using StoreIt.Sharing.Models;

namespace StoreIt.Sharing.Services;

public sealed class CardImportService(ICardSerializer serializer, ICardImportStore store) : ICardImportService
{
    public async Task<IReadOnlyList<CardShareData>> ImportAsync(
        Uri link,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(link);
        cancellationToken.ThrowIfCancellationRequested();
        var cards = serializer.Deserialize(link);
        cancellationToken.ThrowIfCancellationRequested();
        await store.SaveCardsAsync(cards, cancellationToken);
        return cards;
    }
}
