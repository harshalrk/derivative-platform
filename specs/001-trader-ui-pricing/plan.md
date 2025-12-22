# Implementation Plan: Trader UI + Pricing (No Auth)

**Branch**: `001-trader-ui-pricing` | **Date**: 2025-12-14 | **Spec**: `specs/001-trader-ui-pricing/spec.md`
**Input**: Feature specification from `/specs/001-trader-ui-pricing/spec.md`

**Note**: Generated from template and tailored to the updated constitution.

## Summary

A professional trader UI (Blazor) backed by a deterministic HTTP API (ASP.NET Core on .NET 8). Users enter a display name (no auth), view their booked trades, create new trades, and request a price using random market data with a provided seed. Data persists in Postgres using MartenDB (event sourcing). Async messaging via RabbitMQ with Rebus. Containerized to run on macOS Docker.

## Technical Context

**Language/Version**: .NET 8 (ASP.NET Core), Blazor Server (UI)  
**Primary Dependencies**: Blazor, MartenDB, PostgreSQL, RabbitMQ, Rebus  
**Storage**: Postgres (Marten event store + document store)  
**Testing**: OPTIONAL (not a compliance gate per constitution)  
**Target Platform**: macOS via Docker (containers: backend, frontend, Postgres, RabbitMQ)  
**Project Type**: web (frontend + backend)  
**Performance Goals**: Single-trade pricing response within 2 seconds (SC-003)  
**Constraints**: Deterministic pricing via random seed; no authentication; UI professional look and feel; containerized on mac Docker  
**Scale/Scope**: Initial MVP for single-user session context; multi-user via name entry without auth

## Constitution Check

GATE evaluation against Derivative Trade Capture & Pricing System Constitution:
- Deterministic HTTP API for pricing: PASS (random seed ensures reproducibility)
- Explicit market data dependencies: PASS (document random-data seed as dependency; snapshot concept minimal)
- Error handling clarity: PASS (human-readable messages + simple error code)
- Precision & units: PASS (price displayed with currency; two-decimal display precision)
- UI frontend consuming API: PASS (Blazor Server)
- Logging/tests optional: PASS (not required for compliance)

## Project Structure

### Documentation (this feature)

```text
specs/001-trader-ui-pricing/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
└── spec.md              # Feature specification
```

### Source Code (repository root)

```text
backend/
├── src/
│   ├── Api/             # ASP.NET Core Web API (trade + pricing endpoints)
│   ├── Pricing/         # Pricing service (random-seed deterministic)
│   ├── Messaging/       # Rebus integration with RabbitMQ
│   ├── Persistence/     # MartenDB configurations + repositories
│   └── Models/          # Trade, PricingRequest/Result
└── tests/               # OPTIONAL per constitution

frontend/
├── src/
│   ├── Pages/           # Blazor pages: NameEntry, TradesList, NewTrade, PriceModal
│   ├── Components/      # Reusable components (tables, forms)
│   └── Services/        # API client (HTTP JSON)
└── tests/               # OPTIONAL per constitution

infra/
├── docker/              # Dockerfiles, compose.yml for mac
└── migrations/          # DB setup (if needed beyond Marten auto)
```

**Structure Decision**: Web application with separate frontend (Blazor Server) and backend (ASP.NET Core API). RabbitMQ and Postgres run as containers. MartenDB for event sourcing aligned with constitution.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Event sourcing (Marten) | Auditability + replay of trades | Flat CRUD loses provenance and snapshot reproduction |
| Messaging (RabbitMQ + Rebus) | Async workflow and future extensibility | Direct synchronous calls limit scalability and decoupling |

## Implementation Strategy

- MVP First: Name entry → Trades list → New trade → Price trade (seeded random)
- Deterministic pricing: Frontend passes seed; backend uses seed to produce stable outputs
- Persistence: Marten documents/event store in Postgres; minimal schemas
- Messaging: Rebus configured with RabbitMQ for future async flows (e.g., price requests events)
- Containerization: Dockerfiles for backend and frontend; docker-compose for Postgres and RabbitMQ

## Notes

- No authentication in MVP; session name used for filtering trades (`bookedBy`)
- Professional UI: Blazor components, responsive table, form validation, consistent styling
- Logging optional; focus on clear error responses
