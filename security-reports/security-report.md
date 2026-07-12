# Security Report — 001 Card Sorting — 2026-07-12

## Executive Summary
Risk level: **Low**. The `001-card-sorting` change set is a pure, on-device UI/feature
addition (an in-memory sort of already-loaded cards plus a persisted string preference). It
introduces **no CRITICAL or HIGH severity security issues**. There are **no network calls,
telemetry, or off-device data transfer** (local-only guarantee CC-001 preserved), **no
hardcoded secrets** (CC-005 preserved), and no injection, deserialization, or reflection
sinks. The stored preference is parsed defensively with `Enum.TryParse` and a safe fallback.
A few low/informational hardening notes are listed below.

## Findings

| Severity | Location | Description | Recommendation |
|----------|----------|-------------|----------------|
| LOW | `src/StoreIt.Maui/ViewModels/MainViewModel.cs:47-51` | `Enum.TryParse<CardSortMode>(storedMode, out parsedMode)` accepts any numeric string (e.g. a tampered/corrupted `"CardSortMode"` preference value of `"999"`) and returns `true` with an undefined enum value `(CardSortMode)999`. There is **no security impact**: `CardSorter.Sort` uses a `switch` with a safe `default` (LastAccessed), so an out-of-range value degrades gracefully to the default sort rather than crashing or misbehaving. Preferences are per-app sandboxed storage, so the value is not attacker-controlled in practice. | Optional hardening: add `&& Enum.IsDefined(typeof(CardSortMode), parsedMode)` to reject undefined values, keeping the same `LastAccessed` fallback. |
| INFO | `src/StoreIt.Maui/ViewModels/MainViewModel.cs:52-56` (`OnSortModeChanged`) | Persisting the preference and re-sorting happens in a property-changed partial method with no try/catch. `IPreferences.Set` and the in-memory LINQ sort are not expected to throw for these inputs, so this is acceptable. Note only. | No action required. Optionally wrap preference persistence in defensive handling consistent with the app's error-handling conventions. |
| INFO | `src/StoreIt.Maui/Sorting/CardSorter.cs:20-31` | User-controlled `CustomerCard.Name` is used only as an ordering key via `StringComparer.OrdinalIgnoreCase` — pure comparison, no format string, no string interpolation into a sink, no `eval`/reflection. Null input collection is handled (returns empty). No format-string or injection risk. | No action required — confirmed safe. |
| INFO | `src/StoreIt.Maui/Navigation/ShellDialogService.cs:13-14` | New `DisplayActionSheet` overload wraps `Shell.Current.DisplayActionSheetAsync` with a fixed, developer-supplied list of button labels (constants in `MainViewModel`). No user/network-controlled data reaches the dialog. | No action required. |

No CRITICAL or HIGH severity issues were identified in this change set.

## Vulnerable dependencies

No new dependencies are added to the `src/StoreIt.Maui` production project by this feature.

The new **test-only** project `tests/StoreIt.Maui.Tests/StoreIt.Maui.Tests.csproj` adds the
following packages. These ship only with the test assembly, never with the shipped app, so
they carry no runtime exposure for end users.

| Package | Current version | Safe version | Severity | Note |
|---------|-----------------|--------------|----------|------|
| xunit | 2.9.2 | n/a (test-only) | Info | No known advisories. Test framework. |
| xunit.runner.visualstudio | 2.8.2 | n/a (test-only) | Info | Test runner, `PrivateAssets=all`. |
| Microsoft.NET.Test.Sdk | 17.12.0 | n/a (test-only) | Info | Test host. |
| FluentAssertions | 6.12.2 | n/a (test-only) | Info | v6.x is the last MIT-licensed line; no security advisory. Test-only. |
| sqlite-net-pcl | 1.11.284-**beta** | 1.9.172 (stable) | Low (test-only) | Referenced only so the linked `CustomerCard.cs` (SQLite attributes) compiles for tests. It is a **pre-release/beta** and is not part of the shipped app, but pinning test tooling to a beta is a minor supply-chain hygiene concern. Consider a stable release. No known exploit relevant to this attribute-only usage. |

> A `dotnet list package --vulnerable --include-transitive` run is recommended in CI for
> authoritative advisory data; it was not executed here to keep the review read-only/offline.

## False positives
- **Preference "deserialization"** — reading `"CardSortMode"` via `IPreferences.GetString`
  and `Enum.TryParse` is **not** insecure deserialization: no `BinaryFormatter`, no JSON
  object graph, no type/assembly loading from the stored string. Not a finding.
- **`CustomerCard.Name` in sort** — flagged and confirmed safe (INFO row above); it is used
  purely as an `OrdinalIgnoreCase` comparison key, not rendered into any executable/format
  sink. Not a vulnerability.
- **Action sheet button labels** — static developer constants, not user/network input. Not
  an injection vector.
