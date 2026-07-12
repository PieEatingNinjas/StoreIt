# Research: Card Sorting

**Feature**: 001-card-sorting  
**Date**: 2026-07-12  
**Status**: Complete — all NEEDS CLARIFICATION resolved

---

## 1. Sort Logic Placement

**Decision**: Apply sort at the **database query level** in `DatabaseService`. All `GetCardsAsync()` and `SearchCardsAsync()` methods accept a `CardSortMode` parameter and apply the sort via SQLite `ORDER BY` clauses.

**Rationale**:
- **Correctness**: SQL-level sorting is deterministic and correctly handles large datasets, ties, and collation rules. It is the single source of truth for ordering.
- **Performance**: Sorting at the database layer avoids transferring unsorted data over the connection and avoids in-memory LINQ processing. For typical card counts (tens to low hundreds), the difference is negligible; for future scale (thousands), DB-level sort is more efficient.
- **Maintainability**: All sort logic lives in one place (`DatabaseService`). Changes to sort behaviour are localized to SQL queries, not scattered across ViewModel and database service.
- **Consistency**: Every call path that needs sorted cards (`GetCardsAsync`, `SearchCardsAsync`) uses the same sort logic — no risk of inconsistent ordering between views.
- **Testing**: Sort behaviour can be tested directly against the database fixture (integration test), or mocked in ViewModel unit tests.

**Alternatives considered**:
- *In-memory LINQ sort in ViewModel*: Simpler initially but creates a split ownership of sort logic — database has a default query, ViewModel has a re-sort. Risk of inconsistency and harder to maintain as features grow. Rejected.
- *New `ICardSortingService`*: Unnecessary indirection layer. Rejected.

---

## 2. Case-Insensitive Name Sorting

**Decision**: Use SQLite `COLLATE NOCASE` in the `ORDER BY` clause when sorting by the `Name` column.

**Rationale**:
- SQLite `COLLATE NOCASE` provides standard case-insensitive ASCII collation across all platforms.
- No additional LINQ code or configuration needed; the collation is specified directly in the SQL query.
- Consistent and predictable ordering across iOS, Android, and different SQL backends.
- Satisfies FR-005: name sort modes are case-insensitive.

**Example**:
```sql
SELECT * FROM CustomerCards 
ORDER BY IsFavorite DESC, Name COLLATE NOCASE ASC
```

---

## 3. Favorites-First Enforcement

**Decision**: Enforce favorites-first directly in the SQL `ORDER BY` clause in `DatabaseService` (primary key = `IsFavorite DESC`, secondary key = the active `CardSortMode` criterion, with `Id ASC` as a deterministic tie-breaker).

**Rationale**:
- Keeps ordering rules in one place (the database query), consistent with section 1.
- Explicit `Id ASC` tie-breakers prevent apparent shuffling when two cards have the same secondary sort value.

**Sort expressions per mode**:

| Mode | Primary | Secondary |
|------|---------|-----------|
| `LastAccessed` | `IsFavorite DESC` | `LastUsed DESC` |
| `NameAscending` | `IsFavorite DESC` | `Name ASC` (OrdinalIgnoreCase) |
| `NameDescending` | `IsFavorite DESC` | `Name DESC` (OrdinalIgnoreCase) |

**Null / default `LastUsed` handling**: `CustomerCard.LastUsed` is a non-nullable `DateTime`. Cards that were never explicitly accessed have `LastUsed == DateAdded` (set on `SaveCardAsync` insert). They will naturally sort after cards with later `LastUsed` values — consistent with the spec edge case ("cards without a recorded access time appear after cards with recorded access times").

---

## 4. Sort Preference Persistence

**Decision**: Store the selected sort mode as a string via the existing `IUserPreferencesService.GetString` / `SetString` methods using a new constant key `"CardSortMode"`. Default value: `"LastAccessed"` (maps to `CardSortMode.LastAccessed`).

**Rationale**:
- `IUserPreferencesService` already exposes generic `GetString` / `SetString`, backed by MAUI `Preferences` (platform key-value store — on-device, no cloud). No interface or implementation changes required.
- Storing as the enum name string (e.g., `"NameAscending"`) is human-readable and survives future enum reordering. `Enum.TryParse` gives safe deserialization with a fallback to the default.
- A typed `GetCardSortMode` / `SetCardSortMode` pair on the interface would be cleaner but is added complexity for a single use site — not warranted.

**Alternatives considered**:
- *Store as int*: Fragile — breaks if enum values are reordered. Rejected.
- *Add typed methods to `IUserPreferencesService`*: Clean but over-engineered for a single property accessed from one ViewModel. Rejected.

---

## 5. Action Sheet for Sort Mode Selection

**Decision**: Extend `IDialogService` with a new overload `Task<string?> DisplayActionSheet(string title, string cancel, params string[] buttons)` and implement it in `ShellDialogService` via `Shell.Current.DisplayActionSheetAsync`.

**Rationale**:
- `IDialogService` is already injected into `MainViewModel`, so extending it avoids adding a new dependency.
- `Shell.Current.DisplayActionSheetAsync` is the established MAUI pattern for bottom/action sheets (already used in `ViewCardPage.xaml.cs` — though there via code-behind, which this feature avoids in favour of MVVM).
- Returning `string?` lets the ViewModel null-check for "user cancelled" without needing a separate boolean.
- The ViewModel maps the returned label string back to the `CardSortMode` enum.

**Alternatives considered**:
- *Call `DisplayActionSheetAsync` in code-behind*: Used by existing `ViewCardPage.xaml.cs` but violates CC-004 MVVM boundary for new code. Rejected.
- *CommunityToolkit.Maui `BottomSheet`*: More complex, requires a separate XAML page; disproportionate for a three-option picker. Rejected — `DisplayActionSheetAsync` is sufficient.

---

## 6. Toolbar Button Placement

**Decision**: Add a `ToolbarItem` to `MainPage.xaml` bound to an `OpenSortPickerCommand` on `MainViewModel`. The item uses a sort icon (`sort` or equivalent from the app's icon resources) and/or a text label.

**Rationale**:
- `MainPage` is a `ContentPage` inside a MAUI Shell `TabBar`. Shell automatically places `ToolbarItem` entries in the navigation bar — visible without scrolling (satisfies SC-003 directly).
- No existing `ToolbarItem` patterns exist in the codebase, so this introduces the pattern cleanly.
- The XAML binding (`Command="{Binding OpenSortPickerCommand}"`) keeps all logic in the ViewModel.

---

## 7. WhatsNew Integration

**Decision**: Add a new `WhatsNewEntry` to `WhatsNewRegistry` with a new page describing the sort feature. The entry gets the next sequential `Id` (currently max is `2`; new entry will be `Id = 3`).

**Rationale**:
- `WhatsNewService` already handles showing the latest unseen entry on app launch.
- Adding a new page and registry entry is the established extension pattern for `WhatsNew`.
- Discoverability of the new sort button for existing users is addressed passively — the WhatsNew page surfaces on the next launch after update.

---

## 8. Post-Design Constitution Re-Check

All six constitution checks remain green after Phase 1 design:

| Check | Status | Notes |
|-------|--------|-------|
| Local-only data | ✅ | Preferences and SQLite, no network |
| No ads | ✅ | No new SDK |
| MAUI baseline | ✅ | No new packages |
| MVVM + DI | ✅ | `IDialogService` extended; sort logic in `DatabaseService`; `MainViewModel` manages preference |
| Reuse | ✅ | CommunityToolkit.Mvvm + existing services |
| Secret hygiene | ✅ | No secrets |
