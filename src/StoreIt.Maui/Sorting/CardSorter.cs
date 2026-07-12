using StoreIt.Maui.Models;

namespace StoreIt.Maui.Sorting;

/// <summary>
/// Pure, MAUI-free ordering logic for the main card list.
/// Favorites are always placed before non-favorites; the secondary key
/// depends on the selected <see cref="CardSortMode"/>. Uses stable LINQ
/// ordering so tied values keep their input order.
/// </summary>
public static class CardSorter
{
    public static IReadOnlyList<CustomerCard> Sort(IEnumerable<CustomerCard> cards, CardSortMode mode)
    {
        if (cards is null)
        {
            return [];
        }

        var favoritesFirst = cards.OrderByDescending(card => card.IsFavorite);

        var sorted = mode switch
        {
            CardSortMode.NameAscending
                => favoritesFirst.ThenBy(card => card.Name, StringComparer.OrdinalIgnoreCase),
            CardSortMode.NameDescending
                => favoritesFirst.ThenByDescending(card => card.Name, StringComparer.OrdinalIgnoreCase),
            _ => favoritesFirst.ThenByDescending(card => card.LastUsed),
        };

        return sorted.ToList();
    }
}
