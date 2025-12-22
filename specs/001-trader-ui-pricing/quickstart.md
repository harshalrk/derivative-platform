# Quickstart: Trader UI + Pricing (No Auth)

## Prerequisites
- macOS with Docker Desktop
- .NET 8 SDK (for local builds if needed)

## Services
- backend (ASP.NET Core API)
- frontend (Blazor Server UI)
- postgres (for MartenDB)
- rabbitmq (for Rebus)

## Build & Run (Docker Compose)

```bash
# From repo root
docker compose -f infra/docker/compose.yml up --build
```

## Access
- UI: http://localhost:5002
- API: http://localhost:5000
- RabbitMQ mgmt (optional): http://localhost:15672
- Postgres: localhost:5432

## Flows
1. Open UI → enter your name
2. View Trades list (initially empty)
3. Create New Trade
4. Click "Price Trade" (seed provided by UI) → see indicative price

## Notes
- Pricing is deterministic per seed but not financially accurate
- No authentication; name used as `bookedBy` for filtering
- Logging optional; errors are returned as JSON with code+message
