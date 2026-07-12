# Quickstart: Card Sorting Validation

**Feature**: 001-card-sorting  
**Date**: 2026-07-12

---

## Prerequisites

- Android emulator or iOS simulator running (or physical device connected).
- App built and deployed in Debug configuration.
- At least 3 cards seeded: minimum 1 favorite, 2 non-favorites, with varying names and `LastUsed` timestamps. If starting from an empty database, add cards manually first.

---

## Scenario 1 — Default sort (Last Accessed, favorites first)

**Goal**: Verify FR-001, FR-002, SC-001

1. Launch the app fresh (or clear preferences to reset sort mode).
2. Open the main "Mijn Items" tab.
3. Observe the card list.

**Expected**: All favorite cards appear before any non-favorite card. Within each group, cards are ordered from most recently accessed to least recently accessed. No sort picker interaction was required.

---

## Scenario 2 — Sort picker is accessible from toolbar

**Goal**: Verify FR-003, FR-006, SC-003

1. On the main page, locate the sort toolbar button in the navigation bar (top-right area).
2. Confirm the button is visible without scrolling the page.
3. Tap the sort button.

**Expected**: An action sheet or bottom sheet appears listing at minimum: "Last Accessed", "Name A→Z", "Name Z→A", and a cancel option.

---

## Scenario 3 — Switch to Name Ascending

**Goal**: Verify FR-004, FR-005, FR-006, SC-002

1. Tap the sort toolbar button.
2. Select "Name A→Z".
3. Observe the card list immediately.

**Expected**:
- Favorites remain at the top.
- Within the favorites group, cards are ordered A→Z by name (case-insensitive).
- Within the non-favorites group, cards are ordered A→Z by name (case-insensitive).
- The list updates without leaving and re-entering the page (FR-006).
- The update completes in ≤ 2 seconds (SC-002).

---

## Scenario 4 — Switch to Name Descending

**Goal**: Verify FR-004, FR-005

1. Tap the sort toolbar button.
2. Select "Name Z→A".
3. Observe the card list.

**Expected**: Same as Scenario 3 but reversed — Z→A within each group.

---

## Scenario 5 — Switch back to Last Accessed

**Goal**: Verify round-trip sort switching

1. Tap the sort toolbar button.
2. Select "Last Accessed".
3. Observe the card list.

**Expected**: List returns to the `LastUsed DESC` order within each group.

---

## Scenario 6 — Sort preference persists across sessions

**Goal**: Verify FR-007, SC-004

1. Switch to "Name A→Z" via the sort picker.
2. Close the app completely (background → swipe away on device / emulator).
3. Relaunch the app.
4. Open the main page.

**Expected**: The card list is sorted Name A→Z (or whatever was last selected). The sort preference was restored without any user interaction.

---

## Scenario 7 — Favorites always on top regardless of sort mode

**Goal**: Verify FR-002, SC-001 with all sort modes

1. With at least one favorite and one non-favorite card present, cycle through all three sort modes via the picker.
2. After each selection, verify the first card in the list is always a favorite (if any favorites exist).

**Expected**: 100% of list views show all favorite cards before any non-favorite card, regardless of the active sort mode.

---

## Scenario 8 — Edge cases

**Goal**: Verify edge case handling from spec

| Situation | Steps | Expected |
|-----------|-------|----------|
| All cards are favorites | Mark all cards as favorite, switch sort modes | List remains valid; all cards shown; no empty group labels |
| No cards are favorites | Unmark all favorites, switch sort modes | List remains valid; sorted by chosen mode |
| No cards at all | Delete all cards | Empty state ("Geen klantenkaarten gevonden") remains; sort button still accessible; no crash |
| Two cards with identical names | Add two cards named "Test" (same case), sort Name A→Z | Both appear in a stable, non-shuffling order |

---

## WhatsNew Validation (optional)

1. Set the `WhatsNewVersion` preference to a value lower than the new entry's `Id` (or clear it).
2. Launch the app.

**Expected**: The WhatsNew page for this feature appears, describing the new sort control.

---

## References

- Sort behaviour contract: [data-model.md](data-model.md#sort-behaviour-contract)
- Preference key: `"CardSortMode"` — see [data-model.md](data-model.md#preference-record)
- Acceptance scenarios: [spec.md](spec.md#user-scenarios--testing-mandatory)
