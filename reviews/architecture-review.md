# Architecture Review — 001-card-sorting

**Feature**: Card Sorting (`specs/001-card-sorting/`, branch `features/sorting`)
**Reviewer**: ArchitectureAgent (read-only) · **Date**: 2026-07-12
**State reviewed**: working tree vs. merge-base; unit tests green (26/26 passed, `dotnet test tests/StoreIt.Maui.Tests`)

## Verdict

**Solid.** The feature is well-structured, faithful to the spec/plan, and constitution-compliant. Sort logic was correctly extracted into a pure, MAUI-free function with thorough unit coverage. **0 PROBLEM-level findings.** A handful of low-severity smells/nitpicks are noted below, none blocking.

## What's right

- **Testability-driven design.** The plan called `ApplySort` a private `MainViewModel` method; the implementation instead extracted a pure `CardSorter.Sort` static class (`Sorting/CardSorter.cs`). This is a genuine improvement, not a deviation — it made the ordering logic unit-testable without the MAUI head, which is exactly why the 26-test suite exists. Judged against the plan's *binding* decision (in-memory LINQ sort, favorites-first, persisted mode) this fully conforms; the method-vs-class detail was non-binding.
- **Clean MVVM.** No business logic in code-behind. `MainPage.xaml.cs` only wires DI + an opacity animation; all sort logic lives in the VM/pure function. The new dependency (`IUserPreferencesService`) is an interface, DI-registered, and mockable.
- **Correct ordering contract.** `OrderByDescending(IsFavorite).ThenBy/ThenByDescending(...)` gives favorites-first with a stable secondary key (FR-002), `StringComparer.OrdinalIgnoreCase` for names (FR-005), and LINQ's stable sort satisfies FR-008. Null/empty inputs are guarded. Tests cover every FR and edge case, including case-only ties, unknown-enum fallback, and input non-mutation.
- **Consistency with existing patterns.** `WhatsNew120Page` mirrors `WhatsNew110Page` exactly (namespace, ctor injecting `WhatsNewViewModel`, XAML structure), and the registry entry (`Id = 3`, `1.2.0`) is correctly the new max picked by `WhatsNewService`'s `OrderByDescending(Id)`.

## Findings (most → least severe)

### SMELL — action-sheet selection is mapped back via display-string equality
`ViewModels/MainViewModel.cs:106-117` — `OpenSortPickerAsync` maps the returned action-sheet button **label** (`"Laatst gebruikt"`, `"Naam (A-Z)"`, …) back to a `CardSortMode` with a `switch` on those literals. This couples control flow to presentation strings: if a label is ever reworded or localized, the mapping silently falls through and the mode won't change (the `switch` has no matching case, so `SortMode` stays as-is). The constitution's "localization impacts MUST be considered" constraint makes this worth flagging even though the app is currently Dutch-only.
*Why it matters*: fragile selection logic; presentation strings drive branching in the VM.
*Fix*: drive the sheet from an ordered `(CardSortMode mode, string label)` list and resolve the selection by identity/index rather than by re-matching the label string. Low urgency — contained and consistent with the app's current Dutch-only reality.

### NITPICK — constructor assignment triggers `OnSortModeChanged` side effects
`ViewModels/MainViewModel.cs:44-54` — assigning `SortMode` in the constructor invokes the generated setter. For any **non-default** stored mode (backing field defaults to `LastAccessed = 0`), this fires `OnSortModeChanged`, which (a) writes the preference back redundantly and (b) sorts the still-empty `Cards`. Both are harmless (idempotent write, empty sort) but are unnecessary work during construction.
*Fix (optional)*: set the backing field directly in the ctor, or accept the no-op. Not worth churn on its own.

### NITPICK — new async command takes no `CancellationToken`
`ViewModels/MainViewModel.cs:97` — `OpenSortPickerAsync` (and the touched `LoadCardsAsync`) don't accept a `CancellationToken`, which brushes against constitution III's "cancellation support" wording. In practice the only awaited work here is a UI action sheet (not cancellable I/O), and this matches the surrounding code, so value is low. Noted for completeness, not as a required change (Ground Rule #4 — don't demand a repo-wide refactor).

### CORRECTED — toolbar binding must use the plain page-context form
`Views/MainPage.xaml:34` — the `ToolbarItem` binds via `Command="{Binding OpenSortPickerCommand}"`, resolving against the page `BindingContext` (`MainViewModel`).

> **Correction (post-review):** an earlier draft of this review suggested the explicit
> `Command="{Binding ViewModel.OpenSortPickerCommand, Source={x:Reference mainPage}, x:DataType=local:MainPage}"`
> form was "fully correct, no change needed." That was wrong. In practice the `x:Reference mainPage`
> source **did not work** for the `ToolbarItem`: a `ToolbarItem` is not part of the page's visual
> tree / XAML name scope in the same way the `CollectionView` item-template elements are, so
> resolving `{x:Reference mainPage}` from it is unreliable. The plain `{Binding OpenSortPickerCommand}`
> is the correct approach here — the `ToolbarItem` inherits the page `BindingContext` (the
> `MainViewModel`), and the page's `x:DataType="vm:MainViewModel"` keeps the binding compiled.
> The `Source={x:Reference mainPage}` form is only needed inside the `DataTemplate`, whose
> `x:DataType` is `CustomerCard`. Verified: build clean (0/0) with the plain form.

## Constitution check

| Principle | Verdict | Notes |
|---|---|---|
| I — Local-only data | ✅ | Sort mode stored via `IUserPreferencesService` → MAUI `Preferences` (on-device). No card data leaves device; no new I/O. |
| II — MAUI baseline (iOS/Android, .NET 10) | ✅ | Shared code only; no platform-specific code, no new packages. No obsolete APIs introduced by the diff (no `ListView`/renderers/`*AndExpand`; `CollectionView` stays inside a `Grid`, virtualization preserved). |
| III — MVVM + DI + testable + async (NON-NEGOTIABLE) | ✅ | No logic in code-behind; VM is DI-constructed; sort logic is a pure, stateless function (no hidden static state); async/await with no `.Result`/`.Wait()`. Minor cancellation nitpick above. |
| IV — Reuse over custom | ✅ | CommunityToolkit.Mvvm `[ObservableProperty]`/`[RelayCommand]`, existing `IUserPreferencesService`, and in-memory LINQ. No reinvented infrastructure. |
| V — Secret hygiene | ✅ | No secrets, keys, or tokens added. |

## Spec / plan conformance

- **FR-001..FR-009**: all satisfied. Default `LastAccessed` (FR-001, `MainViewModel.cs:44-47`); favorites-first every mode (FR-002); toolbar button → action sheet (FR-003/FR-004, `MainPage.xaml:32-37`, `MainViewModel.cs:99-104`); same rule per group + case-insensitive (FR-005); in-place re-sort without DB round-trip (FR-006, `OnSortModeChanged`); persistence + restore (FR-007); stable ties (FR-008); valid list after favorite/add/remove via `LoadCardsAsync` re-sort (FR-009).
- **CC-001..CC-005**: all satisfied (see constitution table).
- **SC-003 (discoverability)**: `ToolbarItem` present with `SemanticProperties.Description`, plus the WhatsNew 1.2.0 slide — matches the clarified criterion.
- **`ApplySort` → `CardSorter` deviation**: **acceptable improvement**, not a concern. It is a testability win and preserves the plan's binding sort contract exactly.

## Testability approach (linked-source test project)

**Sound and appropriately documented.** The MAUI head multi-targets `net10.0-android;net10.0-ios`, so a plain `net10.0` xUnit project cannot `ProjectReference` it. The test csproj instead `<Compile Include ... Link=>`s the three platform-independent files under test (`CardSortMode`, `CustomerCard`, `CardSorter`) and pulls `sqlite-net-pcl` only so the linked `CustomerCard` attributes compile. This is the standard, pragmatic pattern for testing shared MAUI logic, and the comments explain the rationale clearly. Because `CardSorter` is pure, no MAUI/UI mocking is needed. (Corollary: `MainViewModel` itself remains only partially unit-testable due to its pre-existing concrete `DatabaseService` dependency — *not* introduced by this change — which is precisely why extracting the pure sorter was the right call.)

## WhatsNew DI reasoning

**Verified — the developer's reasoning holds.** Neither `WhatsNew110Page` nor `WhatsNew120Page` is registered in `MauiProgram.cs`. WhatsNew pages are shown via `ShellNavigationService.Show` → `Routing.RegisterRoute("temp", pageType)` + `Shell.Current.GoToAsync`, and Shell instantiates the route type through the MAUI service provider (`ActivatorUtilities`), injecting the DI-registered `WhatsNewViewModel` (transient). Adding `WhatsNew120Page` to DI would be redundant, and omitting it correctly matches the established 110 pattern.

## Behavior & scope check

- **In scope.** Every changed file maps to the plan's file list. `.github/spec/INDEX.md` update is documentation bookkeeping. No unrelated churn.
- **Behavior preserved.** The only pre-existing behavior touched is `LoadCardsAsync`, which now sorts the loaded set instead of returning it raw — an intended feature change, not a regression. `SearchCardsAsync` sort behavior is untouched (no search changes in the diff). No tests were weakened.

## Suggested follow-up

1. Decouple action-sheet selection from display strings (the one SMELL) — resolve the tapped option by index/identity so the mapping survives future wording/localization changes.
2. (Optional, trivial) Set the `sortMode` backing field directly in the constructor to avoid the redundant startup preference write.

---
**PROBLEM-level findings: 0.**
