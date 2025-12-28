# Quickstart: Market Data Curves & Quotes

**Feature**: 003-marketdata-curves
**Purpose**: Get the marketdata service and UI running locally
**Target**: Developers setting up the marketdata module for the first time

## Prerequisites

- **Java 17+**: `java -version`
- **Maven 3.8+**: `mvn -version`
- **Node.js 18+**: `node -version`
- **Docker & Docker Compose**: `docker --version` and `docker-compose --version`
- **PostgreSQL client** (optional): `psql --version` for database inspection

## Project Structure

```
derivative-platform/
├── backend/                      # Existing .NET services
├── frontend/                     # Existing Blazor UI
├── marketdata-service/           # NEW Java/Spring Boot service
│   ├── pom.xml
│   └── src/
├── marketdata-ui/                # NEW React 18 SPA
│   ├── package.json
│   └── src/
└── infra/
    ├── docker/
    │   └── compose.yml           # Docker Compose orchestration
    └── nginx/
        └── nginx.conf            # Reverse proxy config
```

---

## Quick Start (Docker Compose)

### 1. Start all services

```bash
# From project root
cd /Users/harshal/workspace/dotnet/derivative-platform

# Start all services (including nginx, marketdata-service, marketdata-ui)
docker compose -f infra/docker/compose.yml up -d

# Or start only marketdata services with reverse proxy
docker compose -f infra/docker/compose.yml up -d postgres nginx marketdata-service marketdata-ui
```

### 2. Verify services

```bash
docker compose -f infra/docker/compose.yml ps

# Expected output:
# NAME                   STATUS              PORTS
# nginx                  Up                  0.0.0.0:80->80/tcp
# postgres               Up                  5432/tcp
# marketdata-service     Up (healthy)        0.0.0.0:8080->8080/tcp
# marketdata-ui          Up                  0.0.0.0:3000->3000/tcp
```

### 3. Access the application

**Via Reverse Proxy (Recommended)**:
- **Main Shell**: http://localhost (redirects to /trade or landing page)
- **Marketdata UI**: http://localhost/marketdata
- **Marketdata API Docs**: http://localhost/api/marketdata/swagger-ui.html
- **Trade UI** (existing): http://localhost/trade

**Direct Access** (for debugging):
- **Marketdata UI**: http://localhost:3000 (bypasses nginx)
- **Marketdata API**: http://localhost:8080/api (bypasses nginx)

```bash
# Check service status
docker compose -f infra/docker/compose.yml ps

# Expected output:
# NAME                       STATUS    PORTS
# postgres                   Up        5432
# rabbitmq                   Up        5672, 15672
# api                        Up        7050
# ui                         Up        7051
# marketdata-service         Up        8080
# marketdata-ui              Up        3000
```

### 3. Access the application

- **Marketdata UI**: http://localhost:3000
- **API Docs**: http://localhost:8080/swagger-ui.html
- **API Base**: http://localhost:8080/api
- **Trade UI** (existing): http://localhost:7051
- **Trade API** (existing): http://localhost:7050

### 4. Initialize marketdata database

```bash
# Connect to PostgreSQL
docker compose -f infra/docker/compose.yml exec postgres psql -U trader -d traderdb

# Create marketdata database
CREATE DATABASE marketdata;
\q

# Flyway migrations will run automatically on marketdata-service startup
```

### 5. Test with sample data

```bash
# Create a curve
curl -X POST http://localhost:8080/api/curves \
  -H "Content-Type: application/json" \
  -d '{
    "name": "USD-SOFR",
    "date": "2025-12-25",
    "currency": "USD",
    "index": "SOFR",
    "instruments": [
      {"type": "MoneyMarket", "tenor": "ON"},
      {"type": "MoneyMarket", "tenor": "1M"},
      {"type": "Swap", "tenor": "1Y"}
    ]
  }'

# Response: 201 Created with curveId

# Enter quotes
curl -X POST http://localhost:8080/api/quotes \
  -H "Content-Type: application/json" \
  -d '{
    "curveName": "USD-SOFR",
    "curveDate": "2025-12-25",
    "quotes": [
      {"instrumentType": "MoneyMarket", "tenor": "ON", "value": 5.25},
      {"instrumentType": "MoneyMarket", "tenor": "1M", "value": 5.30},
      {"instrumentType": "Swap", "tenor": "1Y", "value": 5.50}
    ]
  }'

# Response: 200 OK with saved quotes
```

### 6. View logs

```bash
# All services
docker compose -f infra/docker/compose.yml logs -f

# Marketdata service only
docker compose -f infra/docker/compose.yml logs -f marketdata-service

# Marketdata UI only
docker compose -f infra/docker/compose.yml logs -f marketdata-ui
```

### 7. Stop services

```bash
# Stop all services
docker compose -f infra/docker/compose.yml down

# Stop only marketdata services
docker compose -f infra/docker/compose.yml stop marketdata-service marketdata-ui
```

---

## Development Setup (Local)

### Backend (Java/Spring Boot)

#### 1. Setup PostgreSQL database

**For Docker Compose** (recommended):
```bash
# Database is created automatically via init script or manual creation
# Connect to running PostgreSQL container
docker compose -f infra/docker/compose.yml exec postgres psql -U trader -d traderdb

# Create marketdata database
CREATE DATABASE marketdata;
\q
```

**For local PostgreSQL**:
```bash
# Connect to PostgreSQL
psql -U postgres -h localhost

# Create database
CREATE DATABASE marketdata;

# Grant permissions
GRANT ALL PRIVILEGES ON DATABASE marketdata TO postgres;

# Exit psql
\q
```

#### 2. Configure application

**For Docker Compose**: Configuration is managed via environment variables in `compose.yml`

**For local development**, edit `marketdata-service/src/main/resources/application.yml`:

```yaml
spring:
  datasource:
    url: jdbc:postgresql://localhost:5432/marketdata
    username: trader
    password: trader123
  jpa:
    hibernate:
      ddl-auto: validate  # Use Flyway for migrations
    show-sql: true
  flyway:
    enabled: true
    locations: classpath:db/migration

server:
  port: 8080
```

#### 3. Run migrations

```bash
cd marketdata-service

# Flyway migrations run automatically on startup
# Or run manually:
mvn flyway:migrate
```

#### 4. Build and run

```bash
# Build
mvn clean package

# Run
mvn spring-boot:run

# Or run JAR directly
java -jar target/marketdata-service-1.0.0.jar
```

#### 5. Verify backend

```bash
# Health check
curl http://localhost:8080/actuator/health

# Get reference data
curl http://localhost:8080/api/reference/currencies
# Expected: ["USD","EUR","GBP","JPY"]
```

---

### Frontend (React 18)

#### 1. Install dependencies

```bash
cd marketdata-ui

npm install
```

#### 2. Configure API endpoint

Edit `marketdata-ui/.env`:

```bash
VITE_API_BASE_URL=http://localhost:8080
```

#### 3. Run development server

```bash
npm run dev

# Vite will start on http://localhost:3000
```

#### 4. Access UI

Open http://localhost:3000 in browser

**Navigation**:
- `/` - Home (redirects to Define Curves)
- `/define` - Define Curves screen
- `/quotes` - Enter Quotes screen
- `/roll` - Roll Quotes screen

#### 5. Build for production

```bash
npm run build

# Output in dist/ folder
# Serve with nginx or any static file server
```

---

## Database Schema Inspection

### View tables

```bash
psql -U postgres -d marketdata

# List tables
\dt

# Expected tables:
# - curves
# - instruments
# - quotes
# - flyway_schema_history

# Describe curve table
\d curves

# View data
SELECT * FROM curves;
SELECT * FROM instruments;
SELECT * FROM quotes;
```

### Sample queries

```sql
-- Get all curve names
SELECT DISTINCT name FROM curves ORDER BY name;

-- Get curve with instruments
SELECT 
    c.name, c.curve_date, c.currency, c.index_name,
    i.instrument_type, i.tenor
FROM curves c
JOIN instruments i ON c.curve_id = i.curve_id
WHERE c.name = 'USD-SOFR'
ORDER BY c.curve_date DESC;

-- Get quotes for curve
SELECT 
    c.name, c.curve_date,
    i.instrument_type, i.tenor,
    q.value
FROM curves c
JOIN instruments i ON c.curve_id = i.curve_id
JOIN quotes q ON i.instrument_id = q.instrument_id
WHERE c.name = 'USD-SOFR' AND c.curve_date = '2025-12-25 22:00:00';
```

---

## Testing

### Backend tests

```bash
cd marketdata-service

# Run all tests
mvn test

# Run specific test
mvn test -Dtest=CurveServiceTest

# Run integration tests only
mvn test -Dtest=**/*IntegrationTest
```

### Frontend tests

```bash
cd marketdata-ui

# Run all tests
npm test

# Run specific test
npm test CurveForm

# Run with coverage
npm test -- --coverage
```

---

## Common Issues & Solutions

### Issue: Port 8080 already in use

**Solution**: Kill process or change port in `application.yml`

```bash
# Find process
lsof -i :8080

# Kill process
kill -9 <PID>

# Or change port in application.yml
server.port: 8081
```

### Issue: PostgreSQL connection refused

**Solution**: Verify PostgreSQL is running

```bash
# Check PostgreSQL status
pg_isready -h localhost -p 5432

# Start PostgreSQL (macOS with Homebrew)
brew services start postgresql

# Start PostgreSQL (Linux systemd)
sudo systemctl start postgresql
```

### Issue: Flyway migration fails

**Solution**: Reset database or repair migration

```bash
# Drop and recreate database
psql -U postgres -c "DROP DATABASE marketdata;"
psql -U postgres -c "CREATE DATABASE marketdata;"

# Or repair Flyway
mvn flyway:repair
```

### Issue: CORS errors in frontend

**Solution**: Configure CORS in Spring Boot

```java
// marketdata-service/src/main/java/com/derivative/marketdata/config/WebConfig.java
@Configuration
public class WebConfig implements WebMvcConfigurer {
    @Override
    public void addCorsMappings(CorsRegistry registry) {
        registry.addMapping("/api/**")
                .allowedOrigins("http://localhost:3000")
                .allowedMethods("GET", "POST", "PUT", "DELETE");
    }
}
```

### Issue: Npm install fails

**Solution**: Clear cache and reinstall

```bash
cd marketdata-ui

# Clear cache
npm cache clean --force

# Delete node_modules and package-lock.json
rm -rf node_modules package-lock.json

# Reinstall
npm install
```

---

## API Testing Examples

### Using curl

```bash
# 1. Get reference data
curl http://localhost:8080/api/reference/tenors

# 2. Create curve
CURVE_RESPONSE=$(curl -s -X POST http://localhost:8080/api/curves \
  -H "Content-Type: application/json" \
  -d '{
    "name": "EUR-EURIBOR",
    "date": "2025-12-25",
    "currency": "EUR",
    "index": "EURIBOR",
    "instruments": [
      {"type": "MoneyMarket", "tenor": "ON"},
      {"type": "MoneyMarket", "tenor": "3M"},
      {"type": "Swap", "tenor": "5Y"}
    ]
  }')

echo $CURVE_RESPONSE

# 3. Query curve
curl "http://localhost:8080/api/curves/query?name=EUR-EURIBOR&date=2025-12-25"

# 4. Enter quotes
curl -X POST http://localhost:8080/api/quotes \
  -H "Content-Type: application/json" \
  -d '{
    "curveName": "EUR-EURIBOR",
    "curveDate": "2025-12-25",
    "quotes": [
      {"instrumentType": "MoneyMarket", "tenor": "ON", "value": 3.85},
      {"instrumentType": "MoneyMarket", "tenor": "3M", "value": 3.90},
      {"instrumentType": "Swap", "tenor": "5Y", "value": 4.15}
    ]
  }'

# 5. Retrieve quotes
curl "http://localhost:8080/api/quotes?curveName=EUR-EURIBOR&curveDate=2025-12-25"

# 6. Roll to next day
curl -X POST http://localhost:8080/api/quotes/roll \
  -H "Content-Type: application/json" \
  -d '{
    "curveName": "EUR-EURIBOR",
    "targetDate": "2025-12-26"
  }'
```

### Using Postman

1. Import OpenAPI spec: `specs/003-marketdata-curves/contracts/openapi.yaml`
2. Set base URL: `http://localhost:8080`
3. Execute requests from collection

---

## Development Workflow

### 1. Make changes

```bash
# Backend changes
cd marketdata-service
# Edit Java files
mvn spring-boot:run  # Hot reload with spring-boot-devtools

# Frontend changes
cd marketdata-ui
# Edit React components
# Vite hot reload automatic
```

### 2. Run tests

```bash
# Backend
mvn test

# Frontend
npm test
```

### 3. Create database migration

```bash
cd marketdata-service/src/main/resources/db/migration

# Create new migration file: V4__description.sql
# Example: V4__add_curve_description_column.sql

# Flyway will auto-apply on next startup
```

### 4. Build for deployment

```bash
# Backend
cd marketdata-service
mvn clean package
# Output: target/marketdata-service-1.0.0.jar

# Frontend
cd marketdata-ui
npm run build
# Output: dist/
```

---

## Next Steps

1. **Implement User Stories**: Start with US1 (Define Curves)
2. **Add Validation**: Implement business rules from spec
3. **UI Components**: Build CurveForm, QuoteGrid, RollPreview
4. **Integration Tests**: Test full workflows (create curve → enter quotes → roll)
5. **Error Handling**: Implement clear error messages per constitution P4
6. **Performance Testing**: Verify <500ms API response, <1s UI render goals

---

## Resources

- **Spec**: `specs/003-marketdata-curves/spec.md`
- **Data Model**: `specs/003-marketdata-curves/data-model.md`
- **API Contract**: `specs/003-marketdata-curves/contracts/openapi.yaml`
- **Research**: `specs/003-marketdata-curves/research.md`
- **Spring Boot Docs**: https://spring.io/projects/spring-boot
- **React Docs**: https://react.dev
- **JQuantLib**: http://www.jquantlib.org
