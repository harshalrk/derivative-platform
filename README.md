# Derivative Trading Platform

A comprehensive derivatives trading and risk management platform featuring:
- **Trade Management**: Real-time swap trading with event sourcing, CQRS, and async projections
- **Market Data**: Curve and quote management for derivatives pricing

## Quick Start

```bash
# Start all services
cd infra/docker
docker compose up -d 

# Access the applications
# Trade UI: http://localhost/trade
# Market Data UI: http://localhost/marketdata
# Trade API: http://localhost/api/trade
# Market Data API: http://localhost/api/marketdata
# API Docs: http://localhost/api/trade/swagger
```

## Modules

### Trade Management Module
Real-time swap trading platform with event sourcing, CQRS, and SignalR for instant updates across all connected clients.

### Market Data Module
Market data curves and daily quotes management system for derivatives pricing. Supports curve versioning by date, daily quote entry, and rolling curves/quotes to new dates.

## Architecture

### Design Patterns

**Event Sourcing + CQRS**
- All trades stored as immutable events in PostgreSQL (Marten)
- Separate read models (projections) optimized for queries
- Commands return immediate responses without waiting for async projections

**Async Projections**
- Background daemon processes events into read-optimized `TradeReadModel`
- Eliminates N event replays per query (massive performance gain)
- Eventually consistent reads with near-instant consistency (~250ms)

**Real-time Updates**
- SignalR hub broadcasts trade creation/updates to all connected clients
- RabbitMQ decouples event publishing from HTTP response
- Trades appear instantly across browser tabs

### Technology Stack

**Trade Management Backend**
- ASP.NET Core 8.0 Web API
- Marten 7.x (Event Store + Document DB)
- PostgreSQL 15 (Event storage + Projections)
- RabbitMQ 3.12 (Async messaging)
- SignalR (Real-time communication)

**Market Data Backend**
- Java 17 / Spring Boot 3.x
- Spring Data JPA / Hibernate
- PostgreSQL 15 (marketdata schema)
- Flyway (Database migrations)
- Jakarta Validation

**Frontends**
- Trade UI: Blazor Server (C# + Razor Components)
- Market Data UI: React 18 + TypeScript + Vite
- Real-time SignalR integration (Trade UI)

**Infrastructure**
- Docker Compose for local development
- Nginx reverse proxy for path-based routing
- Containerized services with health checks

## Project Structure

```
├── backend/src/               # Trade Management (ASP.NET Core)
│   ├── Api/                   # REST API controllers, SignalR hubs
│   ├── Models/                # Domain models, events, read models
│   │   ├── Events/           # Event sourcing events
│   │   ├── Aggregates/       # Domain aggregates for validation
│   │   └── ReadModels/       # Projection read models
│   ├── Persistence/          # Marten configuration, repositories, projections
│   │   └── Projections/      # Async projection definitions
│   ├── Pricing/              # Monte Carlo pricing engine
│   └── Messaging/            # RabbitMQ background services
│
├── frontend/src/TraderUI/    # Trade Management UI (Blazor)
│   ├── Components/
│   │   ├── Pages/            # Main pages (TradesList, NewTrade)
│   │   └── Shared/           # Reusable components (SwapForm, SwapEditModal)
│   └── Services/             # API client, session management
│
├── marketdata-service/       # Market Data Backend (Java/Spring Boot)
│   └── src/main/java/com/derivative/marketdata/
│       ├── controller/       # REST API endpoints
│       ├── service/          # Business logic (CurveService, QuoteService, RollService)
│       ├── repository/       # JPA repositories
│       ├── model/
│       │   ├── entity/       # JPA entities (Curve, Instrument, Quote)
│       │   ├── dto/          # Request/Response DTOs
│       │   └── enums/        # Reference data (Currency, Index, Tenor, InstrumentType)
│       └── exception/        # Exception handlers
│
├── marketdata-ui/            # Market Data UI (React + TypeScript)
│   └── src/
│       ├── pages/            # Main pages (DefineCurves, EnterQuotes, RollQuotes)
│       ├── components/       # Reusable components (CurveForm, QuoteGrid)
│       └── services/         # API clients (curveService, quoteService)
│
├── infra/
│   ├── docker/               # Docker Compose configuration
│   └── nginx/                # Reverse proxy configuration
│
└── specs/                    # Feature specifications
    ├── 001-trader-ui-pricing/
    └── 003-marketdata-curves/
```

## Key Features

### Trade Management
- **Create Swaps**: Define fixed-floating interest rate swaps with full leg configuration
- **Edit Trades**: Modify existing trades (counterparty, dates, legs)
- **Delete Trades**: Soft delete via cancellation events
- **Real-time Sync**: All changes broadcast instantly to connected clients
- **Monte Carlo Pricing**: Multi-threaded simulation with configurable paths
- **Bulk Pricing**: Price multiple trades efficiently in parallel

### Market Data Management
- **Define Curves**: Create market data curves with name, currency, index, date, and term structure (instruments)
- **Enter Quotes**: Daily quote entry with 2 decimal precision, support for positive/negative values
- **Roll Quotes**: Copy curves and quotes from previous dates to new dates
- **Curve Versioning**: Each curve identified by name + date (temporal versioning)
- **Complete Quotes Validation**: All instruments must have quotes before saving
- **CRUD Operations**: Full create, read, update, delete for curves and quotes

### Performance Optimizations
- **Zero Event Replay**: Direct queries on materialized projections (Trade Management)
- **Instant Commands**: Write operations return immediately with data from events
- **Async Processing**: Projection updates happen in background without blocking
- **Fast API Response**: <500ms p95 for curve/quote operations

## Data Flow

### Write Path (Create Trade)
```
1. HTTP POST → Controller
2. Create event → Append to event stream
3. Build response from event data (instant)
4. Return 201 Created
5. [Background] Publish to RabbitMQ → SignalR broadcast
6. [Background] Projection daemon processes event → Update read model
```

### Read Path (List Trades)
```
1. HTTP GET → Controller
2. Query TradeReadModel (single fast query)
3. Return results (no event replay needed)
```

## Development

### Prerequisites
- Docker Desktop
- .NET 8.0 SDK (for local Trade Management development)
- Java 17 + Maven (for local Market Data development)
- Node.js 18+ (for Market Data UI development)

### Build & Run

**All Services (Docker Compose)**
```bash
# Build all images
cd infra/docker
docker compose build

# Start all services
docker compose up -d

# View logs
docker compose logs -f api              # Trade API
docker compose logs -f marketdata-service # Market Data API
docker compose logs -f marketdata-ui     # Market Data UI
docker compose logs -f nginx            # Reverse proxy

# Stop services
docker compose down
```

**Trade Management (Local Development)**
```bash
# Backend
cd backend/src/Api
dotnet run

# Frontend
cd frontend/src/TraderUI
dotnet run
```

**Market Data (Local Development)**
```bash
# Backend
cd marketdata-service
mvn spring-boot:run

# Frontend
cd marketdata-ui
npm install
npm run dev
```

### Database Access

**Trade Management Database (traderdb schema)**
```bash
docker exec -it docker-postgres-1 psql -U trader -d traderdb

# Useful queries
SELECT * FROM mt_events ORDER BY seq_id DESC LIMIT 10;
SELECT * FROM mt_doc_tradereadmodel;
SELECT * FROM mt_event_progression;
```

**Market Data Database (marketdata schema)**
```bash
docker exec -it docker-postgres-1 psql -U trader -d traderdb

# Switch to marketdata schema
\c traderdb
SET search_path TO marketdata;

# Useful queries
SELECT * FROM curves ORDER BY created_at DESC;
SELECT * FROM instruments WHERE curve_id = '<uuid>';
SELECT * FROM quotes WHERE curve_id = '<uuid>';
```

## Market Data API Examples

### Create a Curve
```bash
curl -X POST http://localhost/api/marketdata/curves \
  -H "Content-Type: application/json" \
  -d '{
    "name": "USD-SOFR",
    "date": "2025-12-27",
    "currency": "USD",
    "index": "SOFR",
    "instruments": [
      {"type": "MONEY_MARKET", "tenor": "ON"},
      {"type": "MONEY_MARKET", "tenor": "1M"},
      {"type": "SWAP", "tenor": "1Y"},
      {"type": "SWAP", "tenor": "5Y"}
    ]
  }'
```

### Enter Quotes
```bash
curl -X POST http://localhost/api/marketdata/quotes \
  -H "Content-Type: application/json" \
  -d '{
    "curveName": "USD-SOFR",
    "curveDate": "2025-12-27",
    "quotes": [
      {"instrumentType": "MONEY_MARKET", "tenor": "ON", "value": 5.25},
      {"instrumentType": "MONEY_MARKET", "tenor": "1M", "value": 5.30},
      {"instrumentType": "SWAP", "tenor": "1Y", "value": 5.50},
      {"instrumentType": "SWAP", "tenor": "5Y", "value": 5.85}
    ]
  }'
```

### Roll Curve and Quotes
```bash
curl -X POST http://localhost/api/marketdata/quotes/roll \
  -H "Content-Type: application/json" \
  -d '{
    "curveName": "USD-SOFR",
    "targetDate": "2025-12-30",
    "overwrite": false
  }'
```

### Query Curves
```bash
# Get curve by name and date
curl http://localhost/api/marketdata/curves/query?name=USD-SOFR&date=2025-12-27

# List all curve names
curl http://localhost/api/marketdata/curves

# List all dates for a curve
curl http://localhost/api/marketdata/curves/dates?name=USD-SOFR

# Get quotes for a curve
curl http://localhost/api/marketdata/quotes?curveName=USD-SOFR&curveDate=2025-12-27
```

### Update/Delete Curves
```bash
# Update curve instruments
curl -X PUT http://localhost/api/marketdata/curves/<curve-id> \
  -H "Content-Type: application/json" \
  -d '{
    "instruments": [
      {"type": "MONEY_MARKET", "tenor": "ON"},
      {"type": "SWAP", "tenor": "1Y"}
    ]
  }'

# Delete curve version
curl -X DELETE "http://localhost/api/marketdata/curves?name=USD-SOFR&date=2025-12-27"
```

## Architecture Decisions

### Trade Management

**Why Async Projections?**
- **Before**: Each query replayed N events (slow, CPU intensive)
- **After**: Query materialized view (fast, constant time)
- **Trade-off**: Eventual consistency (~250ms) vs immediate consistency

**Why Build Response from Events?**
- Avoids race condition waiting for async projection
- Immediate response (0ms vs 750ms average)
- Proper CQRS: commands don't wait for read models

**Why Consolidated RabbitMQ Publisher?**
- Single method handles all event types (`PublishTradeEvent(aggregate, "created")`)
- Eliminates duplicate connection/queue setup code
- Easier to maintain and extend

### Market Data Management

**Why Curve Versioning by Date?**
- Market data changes daily (new quotes for same curve name)
- Temporal versioning allows historical analysis and audit trail
- Supports rolling operations (copy previous date to new date)

**Why Complete Quotes Validation?**
- Prevents partial data entry errors
- Ensures all instruments have values before pricing
- Clear error messages show exactly which instruments are missing quotes

**Why Separate Java Microservice?**
- Independent scaling and deployment from Trade Management
- Java/Spring Boot expertise in market data team
- Microservices architecture enables polyglot technology choices

**Why React for Market Data UI?**
- Different user base (market data operators vs traders)
- React ecosystem better suited for data-heavy CRUD operations
- Micro-frontends pattern allows independent UI development

## Reverse Proxy Routing

Nginx routes requests by path:
- `/trade` → Trade Management UI (Blazor)
- `/marketdata` → Market Data UI (React)
- `/api/trade` → Trade Management API (.NET)
- `/api/marketdata` → Market Data API (Java)

This enables:
- Single entry point for all services
- Path-based service discovery
- Shared authentication/authorization layer (future)
- SSL termination at proxy level (future)

## License

Internal trading system - Not for public distribution
