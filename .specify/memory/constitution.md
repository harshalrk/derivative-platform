<!--
Sync Impact Report
- Version change: 5.0.0 → 6.0.0
- Architecture change: Polyglot microservices - introducing Java/Maven marketdata service alongside existing .NET services
- Principle updates:
  * P3 (Explicit Market Data Dependencies) → Updated to reference external marketdata service
  * NEW P6 (Polyglot Service Architecture) → Added principle for multi-language service coordination
- Added sections: P6 (Polyglot Service Architecture)
- Removed sections: None
- Templates requiring updates:
  ⚠ `.specify/templates/plan-template.md` — Add Java/Maven to language/version context, update multi-service structure
  ⚠ `.specify/templates/spec-template.md` — Add inter-service communication patterns to user stories
  ⚠ `.specify/templates/tasks-template.md` — Add Java/Maven service setup tasks, marketdata service implementation tasks
  ⚠ All template updates pending: New service directory structure (marketdata-service/) needs to be reflected
- Follow-up TODOs:
  * Define marketdata service API contract (REST endpoints for curves, rates, vols, FX)
  * Update docker-compose.yml to include Java service
  * Establish service-to-service communication patterns (.NET API → Java marketdata)
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
fail clearly if unavailable. Market data MUST be sourced from a dedicated
marketdata service (Java/Maven) via HTTP API. Use explicit identifiers and
timestamps for market data snapshots to guarantee consistent results. Services
consuming market data MUST handle service unavailability gracefully with clear
error messages.

Rationale: Transparent dependencies prevent hidden variability in pricing.
Centralizing market data in a dedicated service ensures single source of truth
and allows independent scaling and technology choices.

### P4. Error Handling
Error messages MUST be actionable and never silently swallowed. Validation
errors MUST return clear reasons. Logging is OPTIONAL.

Rationale: Clear errors are sufficient for minimal operation; logging can be
added later if needed.

### P5. Precision & Units
All monetary outputs MUST specify currency; numerical precision MUST be defined
for prices and risk metrics. Avoid implicit unit conversions.

Rationale: Financial correctness requires explicit units and precision.

### P6. Polyglot Service Architecture
The system is composed of multiple services using appropriate technologies:
.NET services (API, UI, persistence, messaging) and Java/Maven services
(marketdata). Each service MUST expose HTTP APIs with JSON payloads for
inter-service communication. Services MUST be independently deployable and
SHOULD minimize dependencies. Service dependencies MUST be explicit (via API
contracts) and versioned. New services MUST use minimal dependencies and avoid
framework bloat.

Rationale: Polyglot architecture allows choosing the right tool for each domain
(e.g., Java's mature financial libraries for market data). HTTP/JSON ensures
technology-agnostic integration. Minimal dependencies keep services lightweight
and maintainable.

## Additional Constraints

- Auditability: Record minimal provenance for pricing (snapshot timestamp,
	model id/version, market data ids, marketdata service version) to enable
	result reproduction.
- Performance: Pricing requests SHOULD complete within acceptable latencies for
	desktop/server use; batch pricing may be processed asynchronously. Inter-
	service calls SHOULD use connection pooling and circuit breaker patterns.
- Security: Default to localhost exposure; avoid unauthenticated external
	endpoints. Inter-service communication SHOULD use internal network only.
- Service Discovery: Services MAY use hardcoded URLs for localhost development;
	production SHOULD use environment-based configuration.

## Development Workflow

- Define user stories for capture, pricing, reconciliation, and market data.
- Implement iteratively: deliver small, verifiable increments.
- Code review for domain validation, precision, deterministic interfaces, and
	service contracts.
- Documentation: Update README/quickstart on interface, data, or service changes.
- Contract Testing: Verify inter-service API contracts remain compatible.

## Governance

This constitution supersedes other practices for the trading system. Amendments
MUST include version bump rationale and migration notes (data/interface/service
changes). Compliance reviews MUST verify: capture schema adherence, deterministic
pricing interfaces, explicit market data dependencies, error clarity,
precision/units, service architecture adherence, and semantic versioning. Tests
and logging are optional for compliance.

**Version**: 6.0.0 | **Ratified**: TODO(RATIFICATION_DATE) | **Last Amended**: 2025-12-22
