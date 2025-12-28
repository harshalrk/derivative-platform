<!--
Sync Impact Report
- Version change: 5.0.0 → 5.1.0
- Principle updates: Added P6 (Code Maintainability & Single Responsibility)
- Added sections: None
- Removed sections: None
- Templates requiring updates:
  ✅ `.specify/templates/plan-template.md` — Constitution Check remains compatible
  ✅ `.specify/templates/spec-template.md` — Align stories with UI+API interaction
  ✅ `.specify/templates/tasks-template.md` — Emphasize frontend/backend tasks, consider file size in task breakdown
  ⚠ Pending: `.specify/templates/commands/*` — Directory not present; keep generic references
- Deferred TODOs:
  TODO(RATIFICATION_DATE): Original adoption date not found in repo context
-->
# Derivative Trade Capture & Pricing System Constitution

## Core Principles

### P1. Minimal, Accurate Trade Capture
The system MUST capture trades with essential, domain-valid attributes and
reject incomplete or invalid inputs (e.g., non-positive quantity, unsupported
currency codes). The exact data model is defined in feature specs and may
evolve; the constitution enforces accuracy and minimalism, not field lists.

Rationale: Keeps governance stable while allowing model evolution in specs.

### P2. Deterministic HTTP API for Pricing
Pricing MUST be reproducible and deterministic for the same inputs. Expose
pricing via an HTTP API with JSON I/O: inputs (trade/instrument params, market
data) → outputs (price, greeks if applicable). Errors MUST be human-readable and
include a machine-consumable code. The user interface is a separate frontend
consuming this API.

Rationale: Determinism simplifies testing, reconciliation, and operational use.

### P3. Explicit Market Data Dependencies
Pricing MUST declare required market data (e.g., curves, vols, FX rates) and
fail clearly if unavailable. Use explicit identifiers and timestamps for market
data snapshots to guarantee consistent results.

Rationale: Transparent dependencies prevent hidden variability in pricing.


### P4. Error Handling
Error messages MUST be actionable and never silently swallowed. Validation
errors MUST return clear reasons. Logging is OPTIONAL.

Rationale: Clear errors are sufficient for minimal operation; logging can be
added later if needed.

### P5. Precision & Units
All monetary outputs MUST specify currency; numerical precision MUST be defined
for prices and risk metrics. Avoid implicit unit conversions.

Rationale: Financial correctness requires explicit units and precision.

### P6. Code Maintainability & Single Responsibility
Code MUST follow the Single Responsibility Principle: each file, class, or
component MUST have one clear purpose. File complexity MUST remain readable for
an average engineer (typically <500 lines for components, <300 for classes).
When files exceed readability thresholds, they MUST be refactored into smaller,
focused units with clear interfaces.

Rationale: Maintainability and understandability are critical for long-term
system health. Code that is too complex or tries to do too many things becomes
difficult to test, debug, and extend. Enforcing SRP keeps the codebase
approachable for all team members.

## Additional Constraints

- Auditability: Record minimal provenance for pricing (snapshot timestamp,
	model id/version, market data ids) to enable result reproduction.
- Performance: Pricing requests SHOULD complete within acceptable latencies for
	desktop/server use; batch pricing may be processed asynchronously.
- Security: Default to localhost exposure; avoid unauthenticated external
	endpoints.

## Development Workflow

- Define user stories for capture, pricing, and reconciliation.
- Implement iteratively: deliver small, verifiable increments.
- Code review for domain validation, precision, and deterministic interfaces.
- Documentation: Update README/quickstart on interface or data changes.

## Governance

This constitution supersedes other practices for the trading system. Amendments
MUST include version bump rationale and migration notes (data/interface changes).
Compliance reviews MUST verify: capture schema adherence, deterministic pricing
interfaces, explicit market data dependencies, error clarity, precision/units,
code maintainability and single responsibility, and semantic versioning. Tests
and logging are optional for compliance.

**Version**: 5.1.0 | **Ratified**: TODO(RATIFICATION_DATE) | **Last Amended**: 2025-12-27
