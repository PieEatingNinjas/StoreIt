# Feature Specification: Card Sorting

**Feature Branch**: `[001-card-sorting]`

**Created**: 2026-07-12

**Status**: Draft

**Input**: User description: "allow the user to sort their 'cards'. By default they are sorted on last accessed and with favorites always on top. Maybe allow sorting on name (desc and asc) as well and allow the user to choose on the main page. No matter wheter the user chooses to sort on last accesed or on name (desc or asc) , always enforce that favorites are on top (following the same sorting criteria, I guess)"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - See the most relevant cards first (Priority: P1)

As a user viewing my card list, I want the app to show favorite cards at the top and the rest of my cards below them so I can reach my most important cards quickly.

**Why this priority**: The main card list is the core entry point of the app, and showing favorites first improves speed for the most common task without requiring any setup.

**Independent Test**: Can be fully tested by opening the main card list with a mix of favorite and non-favorite cards and confirming that favorite cards always appear above non-favorite cards.

**Acceptance Scenarios**:

1. **Given** a user has favorite and non-favorite cards, **When** the main card list is shown with the default sort mode, **Then** all favorite cards appear before any non-favorite card.
2. **Given** a user has multiple favorite cards and multiple non-favorite cards, **When** the list is ordered, **Then** cards within each group follow the active sort mode consistently.

---

### User Story 2 - Choose how cards are ordered (Priority: P1)

As a user on the main page, I want to choose the card sort mode so I can view my cards by recent use or alphabetically, depending on what helps me find them faster.

**Why this priority**: Users have different retrieval habits, and a selectable order makes the main list more useful without changing the underlying cards.

**Independent Test**: Can be fully tested by changing the sort mode on the main page and confirming that the card order updates immediately while favorites remain pinned above non-favorites.

**Acceptance Scenarios**:

1. **Given** the main card list is visible, **When** the user selects sort by last accessed, **Then** cards are grouped with favorites first and ordered from most recently accessed to least recently accessed within each group.
2. **Given** the main card list is visible, **When** the user selects sort by name ascending, **Then** cards are grouped with favorites first and ordered from A to Z within each group.
3. **Given** the main card list is visible, **When** the user selects sort by name descending, **Then** cards are grouped with favorites first and ordered from Z to A within each group.

---

### User Story 3 - Keep my chosen order preference (Priority: P2)

As a returning user, I want the app to remember my last selected sort mode so I do not need to set it every time I open the main page.

**Why this priority**: Remembering the user’s choice reduces repeated effort and makes the list behavior predictable across sessions.

**Independent Test**: Can be fully tested by changing the sort mode, leaving the main page or restarting the app, and confirming that the same sort mode is still active when the user returns.

**Acceptance Scenarios**:

1. **Given** a user selects a non-default sort mode, **When** they return to the main page later, **Then** the previously selected sort mode remains active.
2. **Given** a user has never selected a sort mode, **When** they first open the main page, **Then** the default sort mode is last accessed with favorites shown first.

### Edge Cases

- What happens when cards have never been accessed and the active sort mode is last accessed?
  Cards without a recorded access time appear after cards with recorded access times within their favorite or non-favorite group.
- How does the system handle cards with identical names or identical last accessed values?
  Cards with tied values keep a stable, predictable order so the list does not appear to shuffle unexpectedly between views. Name comparison is case-insensitive, so cards whose names differ only by case are treated as tied and resolved by stable ordering.
- What happens when all cards are favorites or no cards are favorites?
  The selected sort mode still applies, and the list remains usable without creating empty group labels or confusing gaps.
- What happens when the user has no cards?
  The empty state remains visible, and the available sort choice does not produce an error or misleading layout.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST display the main card list using last accessed as the default sort mode for users who have not chosen a different sort mode.
- **FR-002**: System MUST always place favorite cards before non-favorite cards, regardless of the selected sort mode.
- **FR-003**: System MUST allow users to choose the active sort mode via a toolbar or navigation bar button on the main page that opens a bottom sheet or action sheet listing the available sort modes.
- **FR-004**: System MUST provide at least these sort modes: last accessed, name ascending, and name descending.
- **FR-005**: System MUST apply the selected sort mode within both the favorite group and the non-favorite group using the same ordering rules. Name-based sort modes MUST use case-insensitive comparison so that cards named "apple" and "Apple" are treated as equivalent for ordering purposes.
- **FR-006**: System MUST update the visible card list after the user changes the sort mode without requiring the user to leave and reopen the page.
- **FR-007**: System MUST remember the user’s last selected sort mode and restore it when the user returns to the main page in a later session.
- **FR-008**: System MUST keep card ordering stable when two or more cards have the same value for the active sort field.
- **FR-009**: System MUST continue to show a valid list state when cards are added, removed, marked as favorite, unmarked as favorite, or accessed while a sort mode is active.

### Constitutional Constraints *(mandatory)*

- **CC-001 Local Data**: Feature MUST keep card payload data on-device and MUST NOT require cloud storage for primary usage.
- **CC-002 No Ads**: Feature MUST NOT introduce ad networks or ad-serving SDKs.
- **CC-003 MAUI Compatibility**: Feature MUST run on the repository MAUI baseline for iOS and Android.
- **CC-004 MVVM + DI**: New UI behavior MUST follow MVVM and route non-UI logic through DI services or ViewModels, not code-behind.
- **CC-005 Secret Safety**: Feature implementation and docs MUST NOT include live secrets or publish keys.

### Key Entities *(include if feature involves data)*

- **Card**: A user-saved card shown in the main list, including a display name, favorite state, and most recent access information.
- **Sort Preference**: The user’s selected ordering mode for the main card list.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In manual validation with a mixed list of favorite and non-favorite cards, 100% of list views show all favorite cards before any non-favorite card.
- **SC-002**: In manual validation, switching between available sort modes updates the visible order on the main page in 2 seconds or less.
- **SC-003**: In manual validation, the sort control is visible in the toolbar on the main page without scrolling on a standard viewport. Additionally, the app's WhatsNew flow MAY be used to introduce the sort control to existing users after the feature ships.
- **SC-004**: In manual validation across app restart scenarios, the previously selected sort mode is restored correctly in 100% of tested sessions.

## Clarifications

### Session 2026-07-12

- Q: How should the sort mode selector appear on the main page? → A: Toolbar/navigation bar button that opens a bottom sheet or action sheet with the available sort modes.
- Q: Should name-based sorting be case-insensitive? → A: Yes, case-insensitive ("apple" and "Apple" sort identically).
- Q: Should SC-003 (discoverability) be reformulated as a directly verifiable criterion? → A: Yes — replace with toolbar visibility criterion; additionally, the app's WhatsNew pages can be used to notify users about the new sort control.

## Assumptions

- The main page already has a clear place where users can access list-related actions without introducing a new navigation flow.
- Existing card data already includes the information needed to determine favorite status and recent access ordering.
- Remembering the user’s selected sort mode is within the intended scope because it supports predictable day-to-day use of the card list.
- The feature applies only to the main card list and does not change sorting behavior in other parts of the app unless separately specified.
- The app's existing WhatsNew pages are available as a mechanism to surface the new sort control to returning users after the feature ships; whether a WhatsNew entry is added is an implementation decision.