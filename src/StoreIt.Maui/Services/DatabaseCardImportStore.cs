using StoreIt.Maui.Models;
using StoreIt.Sharing.Models;
using StoreIt.Sharing.Services;

namespace StoreIt.Maui.Services;

public sealed class DatabaseCardImportStore(DatabaseService databaseService) : ICardImportStore
{
    public Task<int> SaveCardsAsync(
        IReadOnlyCollection<CardShareData> sharedCards,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sharedCards);
        var cards = sharedCards.Select(card => new CustomerCard
        {
            Name = card.Name,
            Description = card.Description ?? string.Empty,
            BarcodeData = card.BarcodeData,
            BarcodeFormat = card.BarcodeFormat,
            CustomCode = card.CustomCode,
            Color = card.Color ?? "#FF6B35",
            IsFavorite = card.IsFavorite,
            IsPrivate = card.IsPrivate
        }).ToArray();
        return databaseService.SaveCardsAsync(cards, cancellationToken);
    }
}
