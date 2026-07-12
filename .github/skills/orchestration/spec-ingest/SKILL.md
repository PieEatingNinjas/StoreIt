---
name: spec-ingest
description: How the Orchestrator ingests Spec Kit output (specs/<feature>/) and keeps the INDEX.md portfolio map current. Use in phase 1 before dispatching sub-agents.
---

# Spec Ingest

> **Related skills:** `orchestration/pipeline-flow`, `orchestration/sub-agent-dispatch`
>
> Specs are authored **upstream by Spec Kit**. You do not write or re-plan them — you read them,
> validate they're ready, and point the rest of the pipeline at them.

## Where specs come from

Spec Kit writes one folder per feature, on a matching git branch:

```
specs/
  001-<feature>/
    spec.md            # WHAT + WHY + acceptance criteria (from /speckit.specify + /speckit.clarify)
    plan.md            # HOW: tech approach, architecture (from /speckit.plan)
    data-model.md      # entities / value objects        (optional)
    contracts/         # API contracts                    (optional)
    research.md        # decisions & rationale            (optional)
    checklists/        # quality checklists               (optional)
```

The **active feature** is the one whose `NNN-feature` branch is checked out. The
**constitution** (`.specify/memory/constitution.md`) applies to every feature.

## Reading Spec Kit output — intent, not a task list

Read the feature as **intent to satisfy**, not a script to run verbatim. This is exactly why the
template stops before `/speckit.tasks` + `/speckit.implement`: decomposition and sequencing are
**yours**, not Spec Kit's.

| File | Read it as | Authoritative on |
|------|-----------|------------------|
| `spec.md` | **Functional intent** — WHAT & WHY, acceptance criteria | Behaviour. Non-negotiable. |
| `plan.md` | **Technical intent** — decisions already made (stack, architecture, data model, contracts) | The *decisions*, not the *order*. |
| `.specify/memory/constitution.md` | **Invariants** | Everything. Trumps both. |

**Inside `plan.md`, separate two things:**

- **Binding decisions** — concrete choices ("EF Core InMemory", "endpoint returns a DTO", "field X
  stays internal"). **Honour these** — don't re-litigate or "improve" them. A clarified technical
  decision is a *result of the process*, not noise to route around.
- **Illustrative guidance** — suggested steps, ordering, scaffolding, sample code snippets.
  **Non-binding.** The orchestrator owns *which agent does what, in which order*; the developer owns
  *implementation order within a task*. A snippet illustrates a decision; it is not a mandate to
  paste verbatim unless it encodes a real contract.

**If `plan.md` reads like a checklist** (T1, T2, step-by-step), extract the **decisions** embedded in
it and **discard the sequencing** — do not execute it the way `/speckit.implement` would.

**Conflict ladder** — resolve upstream, don't paper over in code:

1. **Constitution beats everything.** A plan that violates it is not ready → flag.
2. **`plan.md` contradicts `spec.md`** (the *how* doesn't serve the *what*) → flag; fix via `/speckit.plan`.
3. **`plan.md` contradicts repo conventions** (`copilot-instructions.md`): honour a *deliberate,
   spec-serving* technical decision; flag a drive-by violation.
4. **`plan.md` is silent** → the developer decides within conventions + constitution. Expected, not a gap.

## Ingest checklist (phase 1)

1. **Locate** the active feature folder from the current branch.
2. **Read for intent** — `spec.md` (functional) + `plan.md` (technical decisions) + `data-model.md`/`contracts/`, and the constitution. Interpret them per *Reading Spec Kit output* above.
3. **Validate readiness** — this is the phase-1 gate:
   - acceptance criteria are present and testable;
   - no unresolved `[NEEDS CLARIFICATION]` markers left in the spec;
   - the plan contradicts neither the constitution nor `spec.md`;
   - the scope is a single coherent feature.
4. **Index it** — update `.github/spec/INDEX.md` (below).
5. If anything fails validation → **stop and report**. Fixes happen upstream in Spec Kit
   (`/speckit.clarify`, `/speckit.plan`, `/speckit.analyze`), not in code.

## INDEX.md — the portfolio map

`.github/spec/INDEX.md` is a **curated hub** that links to the Spec Kit feature folders. It is not
a copy of the specs. Keep it small:

1. **Features table** — one row per feature: id, `specs/<feature>/`, status, one-line summary.
2. **Active feature** — which one the pipeline is currently building.
3. **Open bugs / attention points** — a short table.

Update it whenever a feature is added, becomes active, or completes.

## Rules

- Never edit files under `specs/**` or the constitution — those are upstream inputs.
- Pass sub-agents the **exact** spec paths + the constitution path; they have no other context.
- Verify uncertain .NET/Azure APIs via Microsoft Learn MCP before relying on them.
