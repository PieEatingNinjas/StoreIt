using StoreIt.Sharing.Models;

namespace StoreIt.Sharing.Services;

public interface ICardImportService
{
    Task<IReadOnlyList<CardShareData>> ImportAsync(
        Uri link,
        CancellationToken cancellationToken = default);
}
