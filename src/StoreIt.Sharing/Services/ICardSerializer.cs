using StoreIt.Sharing.Models;

namespace StoreIt.Sharing.Services;

public interface ICardSerializer
{
    Uri CreateLink(CardShareData card);
    IReadOnlyList<CardShareData> Deserialize(Uri link);
}
