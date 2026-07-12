# Data Model: Card Sorting

**Feature**: 001-card-sorting  
**Date**: 2026-07-12

---

## Existing Entities (unchanged)

### `CustomerCard` — `src/StoreIt.Maui/Models/CustomerCard.cs`

No schema or model changes required. The fields relevant to this feature already exist:

| Field | Type | Role in sorting |
|-------|------|-----------------|
| `Name` | `string` | Used for name ascending / name descending sort. Case-insensitive comparison (`OrdinalIgnoreCase`). |
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

The following table defines the exact ordering applied by `MainViewModel` to the card list after loading from `DatabaseService`:

| Sort Mode | Group 1 (Favorites) | Group 2 (Non-Favorites) |
|-----------|---------------------|------------------------|
| `LastAccessed` | Favorites, `LastUsed DESC` | Non-favorites, `LastUsed DESC` |
| `NameAscending` | Favorites, `Name ASC` (OrdinalIgnoreCase) | Non-favorites, `Name ASC` (OrdinalIgnoreCase) |
| `NameDescending` | Favorites, `Name DESC` (OrdinalIgnoreCase) | Non-favorites, `Name DESC` (OrdinalIgnoreCase) |

**Tie-breaking**: LINQ `OrderBy` / `ThenBy` in .NET is a stable sort. Tied values within a group keep the relative order they had after the database load (which is `IsFavorite DESC, LastUsed DESC`). This satisfies FR-008 — no apparent shuffling between views.

**Empty-group handling**: No visual group headers or separators are rendered for this feature. Favorites and non-favorites appear as a single flat list; the sort simply ensures all favorites are at the top. When there are no favorites or all cards are favorites, the list remains valid (FR-009).

---

## Service / ViewModel Changes (summary)

### `IDialogService` extension

New method added to interface and `ShellDialogService`:

```
Task<string?> DisplayActionSheet(string title, string cancel, params string[] buttons)
```

Returns the label of the tapped button, or `null` / the cancel label if the user dismisses.

### `MainViewModel` additions

| Addition | Type | Purpose |
|----------|------|---------|
| `SortMode` | `[ObservableProperty] CardSortMode` | Current active sort mode; triggers re-sort on change |
| `OpenSortPickerCommand` | `[RelayCommand]` | Opens action sheet, updates `SortMode`, persists preference |
| `ApplySort(IEnumerable<CustomerCard>)` | private method | Pure sort function; returns sorted list; unit-testable |
| Preference key constant | `const string` | `"CardSortMode"` — preference storage key |

`LoadCardsAsync` is updated to call `ApplySort` after loading from the database.  
`SortMode` setter (or `OnSortModeChanged` partial) re-applies sort to the existing `Cards` collection without a database round-trip, satisfying FR-006.
