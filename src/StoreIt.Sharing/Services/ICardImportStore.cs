using StoreIt.Sharing.Models;

namespace StoreIt.Sharing.Services;

public interface ICardImportStore
{
    Task<int> SaveCardsAsync(
        IReadOnlyCollection<CardShareData> cards,
        CancellationToken cancellationToken = default);
}
