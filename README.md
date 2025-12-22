# Trader Pricing System

A real-time swap trading platform built with event sourcing, CQRS, and async projections for high-performance trade management.

## Quick Start

```bash
# Start all services
cd infra/docker
docker compose up -d

# Access the application
# UI: http://localhost:7051
# API: http://localhost:7050
# API Docs: http://localhost:7050/swagger
```

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

**Backend**
- ASP.NET Core 8.0 Web API
- Marten 7.x (Event Store + Document DB)
- PostgreSQL 15 (Event storage + Projections)
- RabbitMQ 3.12 (Async messaging)
- SignalR (Real-time communication)

**Frontend**
- Blazor Server (C# + Razor Components)
- Real-time SignalR client integration
- Component-based UI architecture

**Infrastructure**
- Docker Compose for local development
- Containerized services with health checks

## Project Structure

```
├── backend/src/
│   ├── Api/                    # REST API controllers, SignalR hubs
│   ├── Models/                 # Domain models, events, read models
│   │   ├── Events/            # Event sourcing events
│   │   ├── Aggregates/        # Domain aggregates for validation
│   │   └── ReadModels/        # Projection read models
│   ├── Persistence/           # Marten configuration, repositories, projections
│   │   └── Projections/       # Async projection definitions
│   ├── Pricing/               # Monte Carlo pricing engine
│   └── Messaging/             # RabbitMQ background services
│
├── frontend/src/TraderUI/
│   ├── Components/
│   │   ├── Pages/             # Main pages (TradesList, NewTrade)
│   │   └── Shared/            # Reusable components (SwapForm, SwapEditModal)
│   └── Services/              # API client, session management
│
└── infra/docker/              # Docker Compose configuration
```

## Key Features

### Trade Management
- **Create Swaps**: Define fixed-floating interest rate swaps with full leg configuration
- **Edit Trades**: Modify existing trades (counterparty, dates, legs)
- **Delete Trades**: Soft delete via cancellation events
- **Real-time Sync**: All changes broadcast instantly to connected clients

### Pricing
- **Monte Carlo Simulation**: Multi-threaded pricing with configurable paths
- **Bulk Pricing**: Price multiple trades efficiently in parallel
- **NPV Calculation**: Present value computation with market data

### Performance Optimizations
- **Zero Event Replay**: Direct queries on materialized projections
- **Instant Commands**: Write operations return immediately with data from events
- **Async Processing**: Projection updates happen in background without blocking

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
- .NET 8.0 SDK (for local development)

### Build & Run
```bash
# Build images
docker compose build

# Start services
docker compose up -d

# View logs
docker compose logs -f api
docker compose logs -f ui

# Stop services
docker compose down
```

### Database Access
```bash
# PostgreSQL connection
docker exec -it docker-postgres-1 psql -U trader -d traderdb

# Useful queries
SELECT * FROM mt_events ORDER BY seq_id DESC LIMIT 10;
SELECT * FROM mt_doc_tradereadmodel;
SELECT * FROM mt_event_progression;
```

## Architecture Decisions

### Why Async Projections?
- **Before**: Each query replayed N events (slow, CPU intensive)
- **After**: Query materialized view (fast, constant time)
- **Trade-off**: Eventual consistency (~250ms) vs immediate consistency

### Why Build Response from Events?
- Avoids race condition waiting for async projection
- Immediate response (0ms vs 750ms average)
- Proper CQRS: commands don't wait for read models

### Why Consolidated RabbitMQ Publisher?
- Single method handles all event types (`PublishTradeEvent(aggregate, "created")`)
- Eliminates duplicate connection/queue setup code
- Easier to maintain and extend

## License

Internal trading system - Not for public distribution
