using SQLite;
using StoreIt.Maui.Models;

namespace StoreIt.Maui.Services;

public static class SqliteExtensions 
{
    public static AsyncTableQuery<CustomerCard> OrderByMode(this AsyncTableQuery<CustomerCard> query, CardSortMode sortMode) =>
        sortMode switch
        {
            CardSortMode.NameAscending => query.OrderByDescending(c => c.IsFavorite)
                                               .ThenBy(c => c.Name)
                                               .ThenBy(c => c.Id),
            CardSortMode.NameDescending => query.OrderByDescending(c => c.IsFavorite)
                                                .ThenByDescending(c => c.Name)
                                                .ThenBy(c => c.Id),
            _ => query.OrderByDescending(c => c.IsFavorite)
                      .ThenByDescending(c => c.LastUsed)
                      .ThenBy(c => c.Id),
        };
}
