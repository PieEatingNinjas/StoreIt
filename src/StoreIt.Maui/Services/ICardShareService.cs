using StoreIt.Maui.Models;

namespace StoreIt.Maui.Services;

public interface ICardShareService
{
    Task ShareAsync(CustomerCard card, CancellationToken cancellationToken = default);
}
