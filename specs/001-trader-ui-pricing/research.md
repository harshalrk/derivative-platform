# Research: Trader UI + Pricing (No Auth)

**Date**: 2025-12-14

## Decisions

- Decision: Blazor Server for UI
  - Rationale: Professional UX with server-side rendering; simpler containerization than WASM
  - Alternatives considered: Blazor WebAssembly (more complex hosting), React+ASP.NET (extra stack complexity)

- Decision: MartenDB for event sourcing on Postgres
  - Rationale: Native .NET integration, event store and document store on PostgreSQL
  - Alternatives considered: EventStoreDB (additional infra), raw SQL (loses event sourcing ergonomics)

- Decision: RabbitMQ + Rebus for messaging
  - Rationale: Mature broker; Rebus provides simple .NET abstractions
  - Alternatives considered: Azure Service Bus (cloud dependency), Kafka (overkill for MVP)

- Decision: Deterministic pricing via random seed
  - Rationale: Satisfies constitution determinism without requiring accurate models
  - Alternatives considered: Fixed mock price (less realistic variability), external market data (overkill now)

- Decision: Dockerized on macOS (Docker Desktop)
  - Rationale: Local consistency; easy orchestration via docker-compose
  - Alternatives considered: Local processes (less reproducible), Kubernetes (heavy for MVP)

## Open Questions (Resolved)

- Blazor Server vs WASM → Server selected for simplicity and professional UI
- Market data model → Not required; seed-driven random outputs suffice
- Auth → None; name entry page sets session context
- Precision → Display to two decimals; include currency
