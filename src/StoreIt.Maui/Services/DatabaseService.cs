using SQLite;
using StoreIt.Maui.Models;

namespace StoreIt.Maui.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task Init()
    {
        if (_database is not null)
            return;

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "CustomerCards.db3");
        _database = new SQLiteAsyncConnection(databasePath);
        await _database.CreateTableAsync<CustomerCard>();

        // Migrations: safely add columns introduced in later versions
        await TryAlterTableAsync("ALTER TABLE CustomerCards ADD COLUMN CustomCode TEXT");
        await TryAlterTableAsync("ALTER TABLE CustomerCards ADD COLUMN IsPrivate INTEGER DEFAULT 0");
    }

    public async Task<List<CustomerCard>> GetCardsAsync(CardSortMode sortMode = CardSortMode.LastAccessed)
    {
        await Init();
        var orderByClause = BuildSortOrderClause(sortMode);
        return await _database!.QueryAsync<CustomerCard>($"SELECT * FROM CustomerCards ORDER BY {orderByClause}");
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

    public async Task<List<CustomerCard>> SearchCardsAsync(string searchTerm, CardSortMode sortMode = CardSortMode.LastAccessed)
    {
        await Init();
        var orderByClause = BuildSortOrderClause(sortMode);
        return await _database!.QueryAsync<CustomerCard>(
            $"SELECT * FROM CustomerCards WHERE {nameof(CustomerCard.Name)} LIKE ? OR {nameof(CustomerCard.Description)} LIKE ? OR ({nameof(CustomerCard.CustomCode)} IS NOT NULL AND {nameof(CustomerCard.CustomCode)} LIKE ?) ORDER BY {orderByClause}",
            $"%{searchTerm}%",
            $"%{searchTerm}%",
            $"%{searchTerm}%");
    }

    private static string BuildSortOrderClause(CardSortMode sortMode) =>
        sortMode switch
        {
            CardSortMode.NameAscending => $"{nameof(CustomerCard.IsFavorite)} DESC, {nameof(CustomerCard.Name)} COLLATE NOCASE ASC, {nameof(CustomerCard.Id)} ASC",
            CardSortMode.NameDescending => $"{nameof(CustomerCard.IsFavorite)} DESC, {nameof(CustomerCard.Name)} COLLATE NOCASE DESC, {nameof(CustomerCard.Id)} ASC",
            _ => $"{nameof(CustomerCard.IsFavorite)} DESC, {nameof(CustomerCard.LastUsed)} DESC, {nameof(CustomerCard.Id)} ASC",
        };

    private async Task TryAlterTableAsync(string sql)
    {
        try
        {
            await _database!.ExecuteAsync(sql);
        }
        catch { /* Column already exists — safe to ignore */ }
    }
}
