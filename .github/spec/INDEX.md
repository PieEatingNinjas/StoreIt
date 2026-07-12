# Spec Index — Portfolio Map

> **Navigation hub.** A curated map over the Spec Kit feature folders in `specs/`.
> It is **not** a copy of the specs — it links to them. The OrchestratorAgent keeps it current
> (see skill `orchestration/spec-ingest`).
>
> Authoring happens **upstream in Spec Kit** (`/speckit.specify` → `/speckit.clarify` →
> `/speckit.plan`). Each feature gets a `specs/<NNN-feature>/` folder on a matching git branch.
> The **constitution** lives at `.specify/memory/constitution.md`.

## Features

| # | Feature | Folder | Status | Summary |
|---|---------|--------|--------|---------|
| 001 | Card Sorting | `specs/001-card-sorting/` | active | User-selectable main-list sort (last accessed, name A→Z / Z→A) with favorites always pinned first; choice persisted via MAUI Preferences. |

<!-- Example row once you have a feature:
| 001 | User login | `specs/001-user-login/` | active | Email + password auth with JWT |
-->

## Active feature

**001 — Card Sorting** (`specs/001-card-sorting/`, branch `features/sorting`). Sort logic in
`MainViewModel`, new `CardSortMode` enum, `IDialogService` action-sheet overload, toolbar button on
`MainPage`, and preference persistence. The pipeline is building this feature.

## Open bugs / attention points

| # | Description | Feature | Status |
|---|-------------|---------|--------|
| _(empty)_ | | | |

---

### How this ties together

1. **Spec Kit** authors `specs/<NNN-feature>/spec.md` + `plan.md` (+ optional `data-model.md`,
   `contracts/`), and maintains the constitution.
2. **OrchestratorAgent** ingests the active feature, updates this INDEX, then dispatches the
   Developer / Tester / Security / Architecture / DevOps sub-agents.
3. This file stays a **thin pointer** — one row per feature, plus what's active and any open bugs.
