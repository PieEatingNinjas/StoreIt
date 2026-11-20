# Architecture Review — Sharing feature

Date: 2026-11-20

## Scope

Reviewed the implementation against issue #48, repository MAUI/MVVM conventions, direct import behavior, platform deep-link lifecycle, DI/testability, UI refresh, and privacy messaging.

## Findings and remediation

### Blocking findings

1. Database initialization race during cold-start imports.
   - Remediated by serializing initialization and publishing the SQLite connection only after table creation and migrations complete.

2. Incoming URLs could be handled before the MAUI window was ready or lost while `Application.Current` was unavailable.
   - Remediated by retrying platform forwarding briefly, queueing links in `App`, draining after startup and window activation, and serializing handlers.

### Other findings

1. Future schema versions could be classified as corrupt payloads.
   - Remediated by reading and checking the envelope version before strict version-1 deserialization.

2. Foreground card list was stale after warm-start imports.
   - Remediated with an import-completed notification and MainPage lifecycle subscription.

3. The StoreIt link existed only in the share request URI field.
   - Remediated by also including it in share text.

4. Legacy cards with a null color could not be shared.
   - Remediated by accepting null colors and applying the default color during import.

## Result

All identified architecture and correctness issues were remediated. Strict Android/iOS builds and unit tests pass.

## Residual operational check

Physical-device smoke tests remain necessary for WhatsApp, AirDrop, Nearby Share, and mail link delivery.
