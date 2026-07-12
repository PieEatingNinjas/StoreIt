# Data Model: Card Sorting

**Feature**: 001-card-sorting  
**Date**: 2026-07-12

---

## Existing Entities (unchanged)

### `CustomerCard` — `src/StoreIt.Maui/Models/CustomerCard.cs`

No schema or model changes required. The fields relevant to this feature already exist:

| Field | Type | Role in sorting |
|-------|------|-----------------|
| `Name` | `string` | Used for name ascending / name descending sort via SQLite `COLLATE NOCASE` (case-insensitive). |
| `IsFavorite` | `bool` | Primary sort key — favorites always placed before non-favorites (FR-002). |
| `LastUsed` | `DateTime` | Secondary sort key for `LastAccessed` mode. Non-nullable; set to `DateAdded` on card creation, updated on each card view. |

**Edge case**: `LastUsed` is always set (never null). Cards created before being explicitly opened have `LastUsed == DateAdded`, so they sort after cards that have been explicitly opened — consistent with spec edge case behaviour.

---

## New Entities

### `CardSortMode` — `src/StoreIt.Maui/Models/CardSortMode.cs`

```
Enum: CardSortMode
Values:
  LastAccessed    — order by LastUsed descending (most recently used first)
  NameAscending   — order by Name ascending, OrdinalIgnoreCase
  NameDescending  — order by Name descending, OrdinalIgnoreCase

Default: LastAccessed
```

**Validation rules**: None — enum is exhaustive; any deserialized value not matching a known member falls back to `LastAccessed`.

**State transitions**: No lifecycle; value is set by user action and persisted as a preference.

---

## Preference Record

### Sort Mode Preference — MAUI `Preferences` (on-device key-value store)

| Key | Type | Default | Notes |
|-----|------|---------|-------|
| `"CardSortMode"` | `string` | `"LastAccessed"` | Stored as `CardSortMode` enum name. Deserialized with `Enum.TryParse<CardSortMode>` with fallback to `LastAccessed`. |

Accessed via `IUserPreferencesService.GetString` / `SetString`. No database table or SQLite column added.

---

## Sort Behaviour Contract

The following table defines the exact SQL ordering applied by `DatabaseService` when loading cards:

| Sort Mode | SQL ORDER BY Clause |
|-----------|---------------------|
| `LastAccessed` | `IsFavorite DESC, LastUsed DESC` |
| `NameAscending` | `IsFavorite DESC, Name COLLATE NOCASE ASC` |
| `NameDescending` | `IsFavorite DESC, Name COLLATE NOCASE DESC` |

**Collation**: SQLite `COLLATE NOCASE` provides case-insensitive ASCII collation for name comparisons, satisfying FR-005 and the clarification that name sorting is case-insensitive.

**Tie-breaking**: The `ORDER BY` clauses include `Id ASC` as a final key, ensuring a stable, deterministic order when two or more cards have the same value for the active sort field (FR-008).

**Empty-group handling**: No visual group headers or separators are rendered for this feature. Favorites and non-favorites appear as a single flat list; the sort simply ensures all favorites are at the top. When there are no favorites or all cards are favorites, the list remains valid (FR-009).

---

## Service / ViewModel Changes (summary)

### `IDialogService` extension

New method added to interface and `ShellDialogService`:

```
Task<string?> DisplayActionSheet(string title, string cancel, params string[] buttons)
```

Returns the label of the tapped button, or `null` / the cancel label if the user dismisses.

### `DatabaseService` modifications

| Modification | Signature | Purpose |
|--------------|-----------|----------|
| `GetCardsAsync(CardSortMode)` | `Task<List<CustomerCard>> GetCardsAsync(CardSortMode sortMode = CardSortMode.LastAccessed)` | Load all cards sorted by the specified mode and favorites-first |
| `SearchCardsAsync(string, CardSortMode)` | `Task<List<CustomerCard>> SearchCardsAsync(string searchTerm, CardSortMode sortMode = CardSortMode.LastAccessed)` | Search results sorted by the specified mode |
| Private sort builder | `string BuildSortOrderClause(CardSortMode)` | Constructs the SQL `ORDER BY` clause for the given mode |

**Sort SQL expressions**:
- `LastAccessed` mode: `ORDER BY IsFavorite DESC, LastUsed DESC`
- `NameAscending` mode: `ORDER BY IsFavorite DESC, Name COLLATE NOCASE ASC`
- `NameDescending` mode: `ORDER BY IsFavorite DESC, Name COLLATE NOCASE DESC`

**Note**: SQLite `COLLATE NOCASE` provides case-insensitive collation for text columns, satisfying the case-insensitive name sort requirement (FR-005).

### `MainViewModel` additions

| Addition | Type | Purpose |
|----------|------|----------|
| `SortMode` | `[ObservableProperty] CardSortMode` | Current active sort mode; changes trigger reload from database |
| `OpenSortPickerCommand` | `[RelayCommand]` | Opens action sheet, updates `SortMode`, persists preference |
| Preference key constant | `const string` | `"CardSortMode"` — preference storage key |

`LoadCardsAsync` is updated to pass `SortMode` to `DatabaseService.GetCardsAsync(SortMode)`.  
`SortMode` setter (or `OnSortModeChanged` partial) calls `LoadCardsAsync()` with the new sort mode, reloading the list from the database.
