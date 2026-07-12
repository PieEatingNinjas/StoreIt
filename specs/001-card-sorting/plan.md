# Implementation Plan: Card Sorting

**Branch**: `001-card-sorting` | **Date**: 2026-07-12 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-card-sorting/spec.md`

## Summary

Allow the user to choose how cards are ordered on the main page (last accessed, name A→Z, name Z→A), with favorites always pinned first within the chosen order. The selected sort mode persists across sessions via MAUI Preferences. Sort logic is implemented at the **database level** in `DatabaseService` via SQLite `ORDER BY` clauses; a new `CardSortMode` enum drives the query construction; `MainViewModel` passes the sort mode to `DatabaseService`; `IDialogService` is extended to support an action sheet for mode selection; a toolbar button in `MainPage.xaml` exposes the control.

## Technical Context

**Language/Version**: C# 13 / .NET 10

**Primary Dependencies**: CommunityToolkit.Mvvm 8.x (`ObservableObject`, `[ObservableProperty]`, `[RelayCommand]`), CommunityToolkit.Maui, sqlite-net-pcl (async), Plugin.Maui.Biometric (existing, unrelated)

**Storage**: SQLite via `DatabaseService` for card data (no schema change required); MAUI `Preferences` via `IUserPreferencesService` for sort mode preference

**Testing**: No automated test suite yet; manual validation per `quickstart.md`

**Target Platform**: iOS and Android — .NET MAUI .NET 10

**Project Type**: mobile-app

**Performance Goals**: Sort mode change updates visible list in ≤ 2 seconds (SC-002); in-memory sort of typical card set (tens to low hundreds) is well within that

**Constraints**: Local-only data (CC-001); no new third-party dependencies; MVVM + DI boundaries (CC-004); must not break existing sort behaviour in `SearchCardsAsync`

**Scale/Scope**: Single user, one device; expected card count: low tens to ~200

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Local-only data** ✅ — Sort preference is stored via MAUI `Preferences` (on-device). Card data remains in local SQLite. No off-device transfer.
- **No ads / no hidden trackers** ✅ — Feature introduces no ad SDK and no telemetry.
- **MAUI baseline** ✅ — Uses .NET 10 MAUI and existing CommunityToolkit.Mvvm/Maui packages. No new dependencies required.
- **MVVM + DI boundaries** ✅ — Sort logic lives in `MainViewModel`; `IDialogService` is extended for action sheet support; no business logic in code-behind.
- **Reuse check** ✅ — CommunityToolkit.Mvvm for observable properties/commands; existing `IUserPreferencesService` for persistence; in-memory LINQ for sorting. No custom infrastructure invented.
- **Secret hygiene** ✅ — No keys, certificates, or tokens involved.

**Gate result**: PASS — all six checks green. No violations to justify.

## Project Structure

### Documentation (this feature)

```text
specs/001-card-sorting/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit.tasks — NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
src/StoreIt.Maui/
├── Models/
│   └── CardSortMode.cs                  [NEW — enum: LastAccessed, NameAscending, NameDescending]
├── Services/
│   ├── IDialogService.cs                [MODIFIED — add DisplayActionSheet overload]
│   ├── DatabaseService.cs               [MODIFIED — add CardSortMode parameter to GetCardsAsync, SearchCardsAsync; add BuildSortOrderClause helper]
│   └── ShellDialogService.cs            [MODIFIED — implement DisplayActionSheet]
├── Navigation/
├── ViewModels/
│   └── MainViewModel.cs                 [MODIFIED — add SortMode property, OpenSortPickerCommand; pass sort mode to DatabaseService]
├── Views/
│   └── MainPage.xaml                    [MODIFIED — add ToolbarItem bound to OpenSortPickerCommand]
└── WhatsNew/
    ├── WhatsNewRegistry.cs              [MODIFIED — add new entry for this version]
    └── Pages/
        └── WhatsNew[ver]Page.xaml[.cs]  [NEW — optional WhatsNew slide for the sort feature]
**Structure Decision**: Single-project mobile app (production code in `src/StoreIt.Maui`), plus a separate `net10.0` xUnit test project under `tests/StoreIt.Maui.Tests` for non-MAUI logic/testing.
## Complexity Tracking

No constitution violations. Section not applicable.
