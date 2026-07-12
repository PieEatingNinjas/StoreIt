# Research: Card Sorting

**Feature**: 001-card-sorting  
**Date**: 2026-07-12  
**Status**: Complete — all NEEDS CLARIFICATION resolved

---

## 1. Sort Logic Placement

**Decision**: Sort in `MainViewModel` (in-memory, after loading from `DatabaseService`).

**Rationale**:
- Sort order is a presentation/UI concern; it does not belong in the persistence layer.
- The existing `DatabaseService.GetCardsAsync()` applies a default sort (`IsFavorite DESC, LastUsed DESC`). This default is preserved unchanged; `MainViewModel` re-applies its own deterministic sort on top of the returned list, so the database never needs to change its query signature.
- In-memory LINQ sort over a few hundred `CustomerCard` objects is effectively instantaneous — well within the SC-002 ≤ 2 s SLA.
- Keeping sort logic in the ViewModel makes it directly unit-testable without needing a database.

**Alternatives considered**:
- *Pass `CardSortMode` to `DatabaseService.GetCardsAsync()`*: Would centralise ordering but mixes a UI preference into a persistence service and requires changing every call site (`GetCardsAsync`, `SearchCardsAsync`). Rejected — unnecessary coupling.
- *New `ICardSortingService`*: Adds an indirection layer with no testability benefit over ViewModel-level sorting for this use case. Rejected — over-engineering.

---

## 2. Case-Insensitive Name Sorting

**Decision**: Use `StringComparer.OrdinalIgnoreCase` (or `StringComparison.OrdinalIgnoreCase` in `string.Compare`) for all name-based sort comparisons.

**Rationale**:
- Ordinal ignore-case is the correct choice for UI-visible alphabetical sort of user-supplied names on mobile (consistent with platform list sorting conventions).
- `CurrentCultureIgnoreCase` introduces locale-dependent ordering that can differ between devices — not desired for a deterministic, predictable sort.
- No SQLite `COLLATE NOCASE` change required because sorting is done in the ViewModel.

**Alternatives considered**:
- *`CurrentCultureIgnoreCase`*: Locale-sensitive; results may differ across user locales. Rejected for predictability.
- *SQLite `COLLATE NOCASE`*: Only works for ASCII; doesn't cover accented characters in card names. Rejected — sorting moved to ViewModel anyway.

---

## 3. Favorites-First Enforcement

**Decision**: Apply a two-level LINQ sort: primary key = `IsFavorite DESC` (favorites first), secondary key = the active `CardSortMode` criterion.

**Rationale**:
- This directly mirrors FR-002 and FR-005 and maps cleanly to LINQ's `.OrderByDescending(...).ThenBy(...)` / `.ThenByDescending(...)` pattern.
- Stable sort: LINQ's `OrderBy` is a stable sort in .NET — ties within the same group keep their relative order from the previous sort (which is `LastUsed DESC` from the database), satisfying FR-008.

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
| MVVM + DI | ✅ | `IDialogService` extended; sort logic in ViewModel |
| Reuse | ✅ | CommunityToolkit.Mvvm + existing services |
| Secret hygiene | ✅ | No secrets |
