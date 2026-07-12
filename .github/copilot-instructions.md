# Copilot Instructions — StoreIt

> Ground rules for EVERY AI agent (Copilot, Claude, …) working in this repo.
> Follow them unless a specific agent or skill instruction explicitly says otherwise.
> This repo contains **StoreIt**, a consumer-facing .NET MAUI mobile app (iOS/Android) with a spec-driven multi-agent SDLC setup.

## What this repo is

StoreIt is a cross-platform .NET MAUI app for storing and managing loyalty cards, membership cards,
and access codes on iOS and Android.
It is frontend-only and DOES NOT communicate with remote APIs.
Privacy-first: data remains on-device, with local persistence via SQLite.

## Key Conventions (this project's .NET bar)

> These are operational conventions agents follow. Architecture/product invariants live in the constitution.

- Target frameworks: use the TFMs defined in the app `.csproj`; C# `nullable enable`; `ImplicitUsings enable`.
- MVVM required: no business logic in code-behind; every new screen gets a ViewModel.
- New ViewModels must be DI-constructed and unit-testable; UI-independent logic belongs in services.
- New Services must be DI-constructed and unit-testable; no static state.
- Current baseline: there are no unit tests yet, and parts of MVVM are still naïve.
- For new implementations, follow surrounding code patterns, but build ViewModels/Services testable from the start.
- Bindable state and commands use the repo’s MVVM toolkit pattern used by existing screens.
- Prefer concise collection initialization (`[]` and target-typed `new()`) over verbose typed collection construction when the target type is already clear.
- `async`/`await` end-to-end for I/O, with `CancellationToken`; no `.Result`/`.Wait()`.
- DI registrations belong in `MauiProgram.cs`.
- One public type per file.

## Build & test

```bash
dotnet restore
dotnet build -f <android-target-framework-from-csproj>
dotnet test
```
