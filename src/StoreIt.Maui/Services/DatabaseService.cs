using SQLite;
using StoreIt.Maui.Models;

namespace StoreIt.Maui.Services;

public class DatabaseService
{
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private SQLiteAsyncConnection? _database;

    private async Task Init()
    {
        if (_database is not null)
        {
            return;
        }

        await _initializationLock.WaitAsync();
        try
        {
            if (_database is not null)
            {
                return;
            }

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "CustomerCards.db3");
            var database = new SQLiteAsyncConnection(databasePath);
            await database.CreateTableAsync<CustomerCard>();

            // Migrations: safely add columns introduced in later versions
            await TryAlterTableAsync(database, "ALTER TABLE CustomerCards ADD COLUMN CustomCode TEXT");
            await TryAlterTableAsync(database, "ALTER TABLE CustomerCards ADD COLUMN IsPrivate INTEGER DEFAULT 0");
            _database = database;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public async Task<List<CustomerCard>> GetCardsAsync(CardSortMode sortMode = CardSortMode.LastAccessed)
    {
        await Init();
        return await _database!.Table<CustomerCard>().OrderByMode(sortMode).ToListAsync();
    }

    public async Task<CustomerCard?> GetCardAsync(int id)
    {
        await Init();
        return await _database!.Table<CustomerCard>()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveCardAsync(CustomerCard card)
    {
        await Init();

        if (card.Id != 0)
        {
            return await _database!.UpdateAsync(card);
        }
        else
        {
            card.DateAdded = DateTime.Now;
            card.LastUsed = DateTime.Now;
            return await _database!.InsertAsync(card);
        }
    }

    public async Task<int> SaveCardsAsync(
        IReadOnlyCollection<CustomerCard> cards,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cards);
        if (cards.Count == 0)
        {
            return 0;
        }

        await Init();
        cancellationToken.ThrowIfCancellationRequested();
        var now = DateTime.Now;
        var cardsToSave = cards.ToArray();
        foreach (var card in cardsToSave)
        {
            card.Id = 0;
            card.DateAdded = now;
            card.LastUsed = now;
        }

        await _database!.RunInTransactionAsync(connection =>
        {
            foreach (var card in cardsToSave)
            {
                connection.Insert(card);
            }
        });
        return cardsToSave.Length;
    }

    public async Task<int> DeleteCardAsync(CustomerCard card)
    {
        await Init();
        return await _database!.DeleteAsync(card);
    }

    public async Task<int> UpdateLastUsedAsync(int cardId)
    {
        await Init();
        var card = await GetCardAsync(cardId);
        if (card != null)
        {
            card.LastUsed = DateTime.Now;
            return await _database!.UpdateAsync(card);
        }
        return 0;
    }

    private static IEnumerable<CustomerCard> SortCards(IEnumerable<CustomerCard> cards, CardSortMode sortMode) =>
        sortMode switch
        {
            CardSortMode.NameAscending => cards
                .OrderByDescending(c => c.IsFavorite)
                .ThenBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(c => c.Id),
            CardSortMode.NameDescending => cards
                .OrderByDescending(c => c.IsFavorite)
                .ThenByDescending(c => c.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(c => c.Id),
            _ => cards
                .OrderByDescending(c => c.IsFavorite)
                .ThenByDescending(c => c.LastUsed)
                .ThenBy(c => c.Id),
        };

    private static async Task TryAlterTableAsync(SQLiteAsyncConnection database, string sql)
    {
        try
        {
            await database.ExecuteAsync(sql);
        }
        catch { /* Column already exists — safe to ignore */ }
    }
}
