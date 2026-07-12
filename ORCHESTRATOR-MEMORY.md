# Orchestrator Memory

> Durable, orchestrator-side lessons learned. Read at Phase 0; append (dated, terse) at the end of each run.
> Scope: how *runs* go — not feature requirements (those live in `specs/`).

## 2026-07-12 — 001-card-sorting

- **Env:** macOS, .NET SDK 10.0.301, workloads `android`/`ios`/`maccatalyst` installed. App TFMs are
  `net10.0-android;net10.0-ios` only (no plain `net10.0`, no Windows/MacCatalyst head). Android build is fast (~22s cold).
- **Testability pattern that worked:** a plain `net10.0` xUnit project **cannot** `ProjectReference` the multi-targeted
  MAUI head. Solution: keep the pure logic in a **MAUI-free static class** (`Sorting/CardSorter.cs`, depends only on
  `Models` + SQLite attributes) and have the test project **link the source files** (`<Compile Include="..\..\src\...\*.cs" Link="..." />`)
  plus add `sqlite-net-pcl` so `CustomerCard.cs` compiles. 26 tests, green. Use this pattern for all future ViewModel/service logic tests.
- **Contract-first parallel dispatch:** When dispatching Dev ∥ Test work, agree on exact public signatures up front
  (interfaces/classes that actually exist in the branch) and pin that contract in both prompts to avoid drift.
- **`IDialogService` lives in namespace `StoreIt.Services`** (not `StoreIt.Maui.Services`); `ShellDialogService` in
  `StoreIt.Navigation`. `IUserPreferencesService` already exposes generic `GetString`/`SetString` — reuse, don't add typed methods.
- **WhatsNew pages are NOT DI-registered** — they resolve via Shell routing + `ActivatorUtilities` (which injects the
  registered `WhatsNewViewModel`). Do not add `AddTransient<WhatsNewXxxPage>` — it's inconsistent with the existing pattern.
- **MainPage toolbar binding:** the `CollectionView` item template rebinds `x:DataType` to `CustomerCard`, so page-level
  commands must bind via `{Binding ViewModel.Cmd, Source={x:Reference mainPage}, x:DataType=local:MainPage}`.
- **Pre-existing CI gap (watch):** `.github/workflows/ci.yml` build jobs still pin `DOTNET_VERSION: 9.0.x` and build
  `net9.0-*` TFMs while the project is `net10.0-*`. Left untouched (out of scope). Added a separate `unit-tests` job on
  `10.0.x`. Flag alignment of the build jobs in a future run.
- **Result:** all gates passed first try, no retries. 0 CRITICAL/HIGH security, 0 PROBLEM architecture.
- **No `python3 yaml` module on this machine** — validate workflow YAML with `ruby -ryaml` instead.
