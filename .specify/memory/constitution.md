<!--
Sync Impact Report
- Version change: template-placeholder -> 1.0.0
- Modified principles:
	- Template Principle 1 -> I. Privacy-First, Local-Only Data
	- Template Principle 2 -> II. MAUI Platform Baseline and Architecture
	- Template Principle 3 -> III. MVVM + DI + Testable Code (NON-NEGOTIABLE)
	- Template Principle 4 -> IV. Reuse Proven Libraries Before Custom Code
	- Template Principle 5 -> V. Open Source and Release Security Hygiene
- Added sections:
	- Product and Technical Constraints
	- Development Workflow and Quality Gates
- Removed sections:
	- None
- Templates requiring updates:
	- ✅ updated: .specify/templates/plan-template.md
	- ✅ updated: .specify/templates/spec-template.md
	- ✅ updated: .specify/templates/tasks-template.md
	- ✅ checked: .specify/templates/commands/*.md (directory not present)
- Follow-up TODOs:
	- None
-->

# StoreIt Constitution

## Core Principles

### I. Privacy-First, Local-Only Data
StoreIt MUST keep user card data fully on-device. The app MUST NOT depend on cloud
storage for core functionality, MUST NOT include ad SDKs, and MUST NOT require
account creation to use the product. Any new feature that introduces data transfer off
device requires an explicit constitutional amendment.

Rationale: StoreIt exists to provide private storage of loyalty and access credentials,
and user trust depends on strict local-data guarantees.

### II. MAUI Platform Baseline and Architecture
The app MUST target iOS and Android on the latest supported .NET MAUI baseline for
this repository (currently .NET 10). Features MUST be implemented in shared MAUI
code first; platform-specific code is allowed only when required by platform
capabilities, APIs, or performance constraints and MUST be isolated.

Rationale: A consistent MAUI baseline maximizes maintainability and keeps release
quality aligned across both app stores.

### III. MVVM + DI + Testable Code (NON-NEGOTIABLE)
UI behavior MUST follow MVVM. Business or workflow logic MUST NOT live in code-
behind. New ViewModels and Services MUST be dependency-injected, unit-testable, and
free of hidden static state. New asynchronous or I/O flows MUST use async/await end-
to-end with cancellation support.

Rationale: Testable MVVM and DI reduce regressions, keep UI code clean, and enable
safe evolution by contributors.

### IV. Reuse Proven Libraries Before Custom Code
Teams MUST prefer mature, actively maintained libraries over custom implementations
when solving common concerns. CommunityToolkit.Maui, CommunityToolkit.Mvvm, SQLite,
and equivalent proven dependencies SHOULD be the default path unless a clear
technical reason is documented in the plan.

Rationale: Reuse lowers defect risk, accelerates delivery, and avoids reinventing
well-solved infrastructure.

### V. Open Source and Release Security Hygiene
All repository contributions MUST be safe for public source release. Secrets,
certificates, signing keys, and publish credentials MUST NEVER be committed. Release
secrets MUST be managed through secure CI/CD secret stores and least-privilege access.
Contributions MUST include checks for accidental secret leakage.

Rationale: Public GitHub collaboration and store publishing require strict secret
handling and transparent security practices.

## Product and Technical Constraints

- Primary storage MUST be local SQLite.
- Core app functionality MUST work offline.
- Telemetry, analytics, and diagnostics MAY be added only if they are privacy-
	preserving, opt-in where legally required, and do not transmit card payload data.
- New dependencies MUST be reviewed for license compatibility with MIT distribution,
	maintenance health, and mobile footprint.
- Accessibility and localization impacts MUST be considered for user-facing features.

## Development Workflow and Quality Gates

- Every spec and implementation plan MUST include an explicit Constitution Check.
- Every pull request MUST confirm:
	- local-only data and no-ad compliance;
	- MVVM + DI boundaries are respected;
	- no secrets are added;
	- iOS and Android build viability is maintained.
- For new logic in ViewModels/Services, contributors SHOULD add or update automated
	tests when feasible, and MUST document manual validation when tests are not yet
	practical.
- CI and release automation MUST fail on detected secrets and MUST enforce required
	build checks before publish workflows.

## Governance

This constitution overrides conflicting process notes in this repository.

Amendment process:
- Propose changes in a pull request that explains motivation, migration impact, and
	affected templates/docs.
- Obtain approval from at least one maintainer.
- Update dependent templates and guidance files in the same change when required.

Versioning policy:
- MAJOR: Backward-incompatible governance changes or principle removals/redefinitions.
- MINOR: New principle/section or materially expanded mandatory guidance.
- PATCH: Clarifications, wording updates, and non-semantic refinements.

Compliance review expectations:
- Constitution compliance MUST be checked during spec planning, implementation review,
	and pre-release validation.
- Non-compliant changes MUST be blocked until either fixed or covered by an approved
	constitutional amendment.

**Version**: 1.0.0 | **Ratified**: 2026-07-12 | **Last Amended**: 2026-07-12
