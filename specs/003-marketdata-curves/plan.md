# Implementation Plan: Market Data Curve & Quote Management

**Branch**: `003-marketdata-curves` | **Date**: 2025-12-25 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/003-marketdata-curves/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Build a standalone Java/Maven microservice for managing market data curves and quotes. The service provides REST APIs for defining curves with term structures (instruments + tenors), entering daily quotes (2 decimal precision), and rolling curves/quotes to new dates. Frontend is React/Vue SPA (marketdata team's choice) integrated via reverse proxy shell. Uses PostgreSQL (marketdata schema) for persistence, JQuantLib adapters for future valuation calculations. Hardcoded reference data (currencies, indexes, instrument types, tenors) designed for easy swap to external service later.

## Technical Context

**Language/Version**: Java 17, JavaScript/TypeScript (React 18)
**Primary Dependencies**: 
- Backend: Spring Boot 3.x, Spring Data JPA, PostgreSQL JDBC driver, JQuantLib (for future valuation adapters)
- Frontend: React 18, Vite, Axios (HTTP client), TypeScript
**Storage**: PostgreSQL (marketdata schema in existing instance) with JPA/Hibernate for ORM
**Testing**: JUnit 5 + Mockito (backend), Jest + React Testing Library + Vitest (frontend)
**Target Platform**: Linux server (Docker container), browser (modern Chrome/Firefox/Safari)
**Project Type**: Web application (micro-frontend: frontend SPA + backend REST API + database)
**Service Architecture**: Micro-frontends pattern - Marketdata team owns full stack (React 18 + TypeScript + Java/Spring Boot + PostgreSQL marketdata schema)
**Performance Goals**: 
- API response time: <500ms p95 for curve/quote CRUD operations
- UI render time: <1 second for quote entry screen with 50 instruments
- Roll operation: <2 seconds regardless of curve size (per SC-004)
**Constraints**:
- 2 decimal precision for all quote values (P5: Precision & Units)
- Quote values can be positive/negative (unlimited range, no validation bounds)
- Minimum 1 instrument per curve (FR-006)
- No duplicate tenors within curve version (FR-005)
- All instruments must have quotes when saving (FR-014, FR-017)
**Scale/Scope**: 
- Expected users: 5-20 market data operators (low concurrency)
- Curves: ~100 curve names × 250+ business dates = 25,000+ curve versions over time
- Instruments per curve: 5-50 instruments typical
- Quote entry: Daily workflow (1-2 hours/day)
- Last save wins (no conflict resolution due to low user count)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Constitution Version**: 5.0.0 (Last Amended: 2025-12-14)

**Note**: Spec mentions constitution P6 (Polyglot Service Architecture) but constitution file shows v5.0.0. Proceeding with v5.0.0 checks. P6 may need to be ratified separately.

### P1. Minimal, Accurate Trade Capture
- ✅ **COMPLIANT**: Feature captures curves with minimal, accurate attributes (name, currency, index, date, instruments)
- ✅ **COMPLIANT**: Validates all inputs (FR-005 no duplicate tenors, FR-006 minimum 1 instrument, FR-014 all quotes required, FR-026 unique name+date)
- ✅ **COMPLIANT**: Rejects incomplete/invalid inputs with clear error messages

### P2. Deterministic HTTP API for Pricing
- ✅ **COMPLIANT**: REST API with JSON I/O (13 endpoints defined in Technical Context section)
- ⚠️ **PARTIAL**: This feature provides market data (curves/quotes), not pricing directly. Future pricing features will consume this market data.
- ✅ **COMPLIANT**: Errors are human-readable with actionable messages (e.g., "All instruments must have quote values. Missing quotes for: [list]")
- ✅ **COMPLIANT**: UI (React/Vue) is separate frontend consuming the API

### P3. Explicit Market Data Dependencies
- ✅ **COMPLIANT**: This feature IS the market data source. Curves identified by name+date (5 PM Eastern Time), quotes tied to specific curve versions
- ✅ **COMPLIANT**: Timestamps are explicit (curve date includes 5 PM Eastern with EST/EDT handling per server clock)
- ✅ **COMPLIANT**: Future pricing services will call these APIs with explicit curve identifiers (name+date) to guarantee consistent results

### P4. Error Handling
- ✅ **COMPLIANT**: All validation errors return clear, actionable reasons (FR-001 through FR-029 specify validation rules)
- ✅ **COMPLIANT**: No silent failures - missing quotes, duplicate tenors, invalid data all trigger explicit errors
- ✅ **COMPLIANT**: Logging is optional (not specified in requirements)

### P5. Precision & Units
- ✅ **COMPLIANT**: Quote values stored with exactly 2 decimal places (FR-016)
- ✅ **COMPLIANT**: Currencies explicit (USD, EUR, GBP, JPY hardcoded, part of curve definition)
- ✅ **COMPLIANT**: No implicit unit conversions - quotes are raw decimal values with 2 decimal precision

### Additional Constraints
- ✅ **Auditability**: Curve versions and quotes are timestamped (curve date + 5 PM Eastern), preserving provenance
- ✅ **Performance**: Performance goals defined (<500ms API p95, <1s UI render, <2s roll operation)
- ✅ **Security**: Service defaults to internal deployment (Docker Compose), external exposure via reverse proxy with shared auth

**GATE RESULT**: ✅ **PASS** - No violations. Feature aligns with constitution v5.0.0.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
# Micro-frontends architecture: Separate services at root level

# Backend: Java/Maven microservice (root level)
marketdata-service/               # NEW - Java service
│   ├── pom.xml
│   ├── src/
│   │   ├── main/
│   │   │   ├── java/com/derivative/marketdata/
│   │   │   │   ├── MarketDataApplication.java
│   │   │   │   ├── controller/
│   │   │   │   │   ├── CurveController.java
│   │   │   │   │   ├── QuoteController.java
│   │   │   │   │   └── ReferenceDataController.java
│   │   │   │   ├── service/
│   │   │   │   │   ├── CurveService.java
│   │   │   │   │   ├── QuoteService.java
│   │   │   │   │   ├── RollService.java
│   │   │   │   │   └── JQuantLibAdapter.java
│   │   │   │   ├── repository/
│   │   │   │   │   ├── CurveRepository.java
│   │   │   │   │   └── QuoteRepository.java
│   │   │   │   ├── model/
│   │   │   │   │   ├── entity/
│   │   │   │   │   │   ├── Curve.java
│   │   │   │   │   │   ├── Instrument.java
│   │   │   │   │   │   └── Quote.java
│   │   │   │   │   └── dto/
│   │   │   │   │       ├── CurveRequest.java
│   │   │   │   │       ├── QuoteRequest.java
│   │   │   │   │       └── RollRequest.java
│   │   │   │   ├── config/
│   │   │   │   │   ├── DatabaseConfig.java
│   │   │   │   │   └── SecurityConfig.java
│   │   │   │   └── exception/
│   │   │   │       ├── CurveNotFoundException.java
│   │   │   │       ├── InvalidQuoteException.java
│   │   │   │       └── GlobalExceptionHandler.java
│   │   │   └── resources/
│   │   │       ├── application.yml
│   │   │       └── application-docker.yml
│   │   └── test/
│   │       └── java/com/derivative/marketdata/
│   │           ├── controller/
│   │           ├── service/
│   │           └── repository/
│   └── Dockerfile

# Frontend: React SPA (root level)
marketdata-ui/                    # NEW - React app
│   ├── package.json
│   ├── vite.config.js (or webpack.config.js)
│   ├── src/
│   │   ├── main.jsx (or main.js for Vue)
│   │   ├── App.jsx
│   │   ├── pages/
│   │   │   ├── DefineCurves.jsx
│   │   │   ├── EnterQuotes.jsx
│   │   │   └── RollQuotes.jsx
│   │   ├── components/
│   │   │   ├── CurveForm.jsx
│   │   │   ├── InstrumentBuilder.jsx
│   │   │   ├── QuoteGrid.jsx
│   │   │   └── RollPreview.jsx
│   │   ├── services/
│   │   │   ├── api.js
│   │   │   ├── curveService.js
│   │   │   └── quoteService.js
│   │   ├── utils/
│   │   │   ├── validation.js
│   │   │   └── dateUtils.js
│   │   └── styles/
│   │       └── main.css
│   ├── public/
│   └── Dockerfile
└── TraderUI/                     # EXISTING - Blazor app (Trade team)

# Infrastructure
infra/
├── docker/
│   └── compose.yml               # UPDATE - Add marketdata-service + marketdata-ui
└── nginx/
    └── nginx.conf                # NEW - Reverse proxy config (/marketdata/* → marketdata-ui)

# Database migrations
backend/marketdata-service/src/main/resources/db/migration/
├── V1__create_curves_table.sql
├── V2__create_instruments_table.sql
└── V3__create_quotes_table.sql
```

**Structure Decision**: Web application (Option 2) with micro-frontends pattern. Marketdata team owns full stack: React/Vue frontend + Java/Spring Boot backend + PostgreSQL marketdata schema. Separate deployment units integrated via Nginx reverse proxy. Existing trade module (Blazor + .NET) remains unchanged.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

**Status**: N/A - No constitution violations detected.

---

## Phase 0: Research (Complete)

**Status**: ✅ Complete
**Output**: [research.md](research.md)

**Resolved Clarifications**:
1. ✅ Frontend Framework: React 18 (ecosystem, TypeScript, financial UI libraries)
2. ✅ Backend Framework: Spring Boot 3.2.x + minimal dependencies
3. ✅ Frontend Testing: Jest + React Testing Library + Vitest
4. ✅ Database Schema: 3 tables with composite unique constraints, temporal versioning
5. ✅ JQuantLib Integration: Adapter pattern, separate from domain models
6. ✅ Eastern Time: `ZoneId.of("America/New_York")` for automatic DST
7. ✅ Reverse Proxy: Nginx path-based routing (/marketdata/*)
8. ✅ Docker Compose: 2 new services sharing PostgreSQL instance

---

## Phase 1: Design & Contracts (Complete)

**Status**: ✅ Complete

**Artifacts Generated**:

### 1. Data Model ([data-model.md](data-model.md))
- 3 entities: Curve, Instrument, Quote
- Temporal versioning via (name, date) unique constraint
- No cascade deletes (orphaned quotes supported)
- UUID primary keys, DECIMAL(10,2) for 2 decimal precision
- JPA entity mappings with validation

### 2. API Contract ([contracts/openapi.yaml](contracts/openapi.yaml))
- 13 REST endpoints (4 reference data, 5 curves, 2 quotes, 2 operations)
- Full request/response schemas with validation rules
- Error responses with machine-readable codes
- Examples for all endpoints

### 3. Quickstart Guide ([quickstart.md](quickstart.md))
- Docker Compose setup instructions
- Local development setup (backend + frontend)
- Database inspection queries
- API testing examples
- Common issues & solutions

### 4. Agent Context Update
- Updated `.github/agents/copilot-instructions.md`
- Added PostgreSQL (marketdata schema) + JPA/Hibernate
- Added web application pattern (micro-frontends)

---

## Constitution Check (Post-Design)

**Re-evaluation after Phase 1 design**:

### P1. Minimal, Accurate Trade Capture
- ✅ Data model enforces minimal, accurate attributes (3 entities, clear constraints)
- ✅ Database constraints enforce uniqueness, required fields
- ✅ No over-engineering (UUID PKs, simple relationships)

### P2. Deterministic HTTP API for Pricing
- ✅ REST API fully specified in OpenAPI contract
- ✅ JSON I/O with clear request/response schemas
- ✅ Error responses include machine-readable codes + human messages
- ✅ UI separate from API (React SPA consumes backend)

### P3. Explicit Market Data Dependencies
- ✅ Curves identified by name + date (5 PM Eastern timestamp)
- ✅ Quotes tied to specific curve version (instrument_id FK)
- ✅ Temporal versioning ensures reproducibility

### P4. Error Handling
- ✅ All validation errors specified in OpenAPI (DUPLICATE_TENOR, NO_INSTRUMENTS, INCOMPLETE_QUOTES, etc.)
- ✅ Error messages actionable (e.g., "Missing quotes for: [1Y, 5Y]")
- ✅ No silent failures in API design

### P5. Precision & Units
- ✅ DECIMAL(10, 2) enforces 2 decimal places
- ✅ Currencies explicit in curve definition
- ✅ No implicit conversions in quote values

**GATE RESULT**: ✅ **PASS** - Design aligns with constitution. Ready for implementation.

---

## Next Steps

**Phase 2 will be executed via `/speckit.tasks` command** (separate from this planning phase):
- Generate `tasks.md` with implementation checklist
- Break down user stories into concrete development tasks
- Provide testing strategy and acceptance criteria

**To proceed**:
```bash
# Run tasks generation command
/speckit.tasks
```

---

## Appendix: Technology Decisions Summary

| Component | Technology | Version | Rationale |
|-----------|-----------|---------|-----------|
| Backend Language | Java | 17 | Constitution alignment, JQuantLib compatibility |
| Backend Framework | Spring Boot | 3.2.x | Minimal dependencies, JPA/PostgreSQL, auto-configuration |
| Frontend Language | JavaScript/TypeScript | ES2022 | Type safety, React ecosystem |
| Frontend Framework | React | 18 | Financial UI libraries, ecosystem maturity, TypeScript support |
| Build Tool (Backend) | Maven | 3.8+ | Standard Java build tool, Spring Boot integration |
| Build Tool (Frontend) | Vite | 5.x | Fast HMR, better than CRA, Vitest integration |
| Database | PostgreSQL | 14+ | Existing instance, separate marketdata schema |
| ORM | JPA/Hibernate | (via Spring Boot) | Standard Java persistence, entity mapping |
| Testing (Backend) | JUnit 5 + Mockito | - | Spring Boot default, industry standard |
| Testing (Frontend) | Jest + RTL + Vitest | - | React best practices, user-centric testing |
| API Documentation | OpenAPI | 3.0.3 | Standard REST API specification |
| Containerization | Docker + Compose | - | Micro-services deployment |
| Reverse Proxy | Nginx | Alpine | Lightweight, path-based routing |
| Valuation Library | JQuantLib | 0.2.4 | Financial calculations (future use) |

---

**Plan Complete**: All clarifications resolved, design artifacts generated, constitution validated. Ready for implementation tasks generation.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
