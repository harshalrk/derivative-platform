# Phase 0: Research - Market Data Curve & Quote Management

**Date**: 2025-12-25
**Feature**: 003-marketdata-curves
**Purpose**: Resolve NEEDS CLARIFICATION items from Technical Context and establish technology choices

## Research Tasks

### 1. Frontend Framework Choice (React vs Vue)

**Question**: Should the marketdata UI use React 18 or Vue 3?

**Decision**: **React 18**

**Rationale**:
- **Ecosystem maturity**: React has larger ecosystem for financial/enterprise UIs (AG Grid, Recharts, Material-UI)
- **Team expertise**: More common in fintech; easier to hire React developers
- **TypeScript support**: Excellent TypeScript integration for type-safe API clients
- **Component libraries**: Better selection of data grid components for quote entry (AG Grid, React Table)
- **Testing**: React Testing Library + Jest is industry standard with extensive resources

**Alternatives considered**:
- **Vue 3**: Lighter weight, simpler syntax, good TypeScript support. Would work equally well but smaller fintech ecosystem.
- **Angular**: Too heavy for this use case, requires more boilerplate than necessary.

**Implementation notes**:
- Use Vite for build tooling (faster than Create React App)
- TypeScript for type safety
- Axios for HTTP client (type-safe API calls)
- Material-UI or Ant Design for component library (financial UI patterns)

---

### 2. Spring Boot Setup & Dependencies

**Question**: What Spring Boot version and minimal dependencies are needed?

**Decision**: **Spring Boot 3.2.x with minimal web + data + PostgreSQL**

**Rationale**:
- **Spring Boot 3.2.x**: Latest stable, Java 17+ required (aligns with Technical Context)
- **Minimal dependencies**: Per constitution and spec requirements for "minimal dependencies"
- **JPA/Hibernate**: Standard for relational data with PostgreSQL
- **No unnecessary frameworks**: Skip Spring Security (handled by reverse proxy), skip messaging (not needed yet)

**Dependencies (pom.xml)**:
```xml
<dependencies>
    <!-- Web REST API -->
    <dependency>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-starter-web</artifactId>
    </dependency>
    
    <!-- Data persistence -->
    <dependency>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-starter-data-jpa</artifactId>
    </dependency>
    
    <!-- PostgreSQL driver -->
    <dependency>
        <groupId>org.postgresql</groupId>
        <artifactId>postgresql</artifactId>
        <scope>runtime</scope>
    </dependency>
    
    <!-- JQuantLib (for future valuation adapters) -->
    <dependency>
        <groupId>org.jquantlib</groupId>
        <artifactId>jquantlib</artifactId>
        <version>0.2.4</version>
    </dependency>
    
    <!-- Validation -->
    <dependency>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-starter-validation</artifactId>
    </dependency>
    
    <!-- Testing -->
    <dependency>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-starter-test</artifactId>
        <scope>test</scope>
    </dependency>
</dependencies>
```

**Alternatives considered**:
- **Micronaut**: Lighter weight but less mature ecosystem, harder to find developers
- **Quarkus**: Native compilation not needed for this use case
- **Plain Servlet/JAX-RS**: Too much boilerplate compared to Spring Boot's auto-configuration

---

### 3. Frontend Testing Stack

**Question**: What testing tools for React frontend?

**Decision**: **Jest + React Testing Library + Vitest**

**Rationale**:
- **Jest**: Industry standard for React unit testing, excellent mocking capabilities
- **React Testing Library**: User-centric testing (test behavior, not implementation)
- **Vitest**: Faster than Jest for Vite-based projects, Jest-compatible API
- **Testing approach**: Component tests for forms/grids, integration tests for API calls

**Testing strategy**:
```javascript
// Component tests
test('CurveForm validates duplicate tenors', () => { ... });
test('QuoteGrid requires all instruments have values', () => { ... });

// Integration tests (mock API)
test('Saving quotes calls POST /api/quotes with correct payload', () => { ... });
test('Rolling quotes fetches previous date and displays preview', () => { ... });
```

**Alternatives considered**:
- **Cypress/Playwright**: Good for E2E but overkill for component testing
- **Enzyme**: Deprecated, React Testing Library is modern replacement

---

### 4. PostgreSQL Schema Design Patterns

**Question**: How to structure PostgreSQL schema for curves, instruments, quotes with temporal versioning?

**Decision**: **Three tables with composite unique constraints**

**Rationale**:
- **Curves table**: Primary key on curve_id (UUID), unique constraint on (name, date)
- **Instruments table**: Primary key on instrument_id (UUID), foreign key to curve_id, unique constraint on (curve_id, tenor)
- **Quotes table**: Primary key on quote_id (UUID), foreign key to instrument_id, one-to-one relationship

**Schema design**:
```sql
CREATE TABLE curves (
    curve_id UUID PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    curve_date TIMESTAMP NOT NULL,  -- Always 5 PM Eastern
    currency VARCHAR(3) NOT NULL,
    index_name VARCHAR(50) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_curve_name_date UNIQUE (name, curve_date)
);

CREATE TABLE instruments (
    instrument_id UUID PRIMARY KEY,
    curve_id UUID NOT NULL REFERENCES curves(curve_id),
    instrument_type VARCHAR(20) NOT NULL,  -- MoneyMarket, Future, Swap
    tenor VARCHAR(10) NOT NULL,  -- ON, 1M, 3M, etc.
    CONSTRAINT uq_curve_tenor UNIQUE (curve_id, tenor)
);

CREATE TABLE quotes (
    quote_id UUID PRIMARY KEY,
    instrument_id UUID NOT NULL REFERENCES instruments(instrument_id),
    value DECIMAL(10, 2) NOT NULL,  -- 2 decimal places per FR-016
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_curves_name ON curves(name);
CREATE INDEX idx_curves_date ON curves(curve_date);
CREATE INDEX idx_instruments_curve ON instruments(curve_id);
CREATE INDEX idx_quotes_instrument ON quotes(instrument_id);
```

**Orphaned quotes handling**: When curve structure changes (instrument removed), quotes remain (no CASCADE DELETE). This aligns with spec assumption "orphaned quotes may exist."

**Alternatives considered**:
- **JSON column for instruments**: Less relational, harder to query/validate
- **Separate tables per instrument type**: Over-normalization, violates DRY
- **Event sourcing**: Overkill for this feature (no audit trail requirement)

---

### 5. JQuantLib Integration Pattern

**Question**: How to structure adapters between domain models and JQuantLib classes?

**Decision**: **Separate adapter service, no JQuantLib in domain models**

**Rationale**:
- **Domain models** (Curve, Instrument, Quote entities) remain lightweight JPA entities for CRUD
- **JQuantLibAdapter** converts domain → JQuantLib classes only when valuation needed
- **Future-proof**: Valuation not in scope for this feature, but architecture supports it

**Adapter pattern**:
```java
// Domain model (JPA entity)
@Entity
public class Curve {
    @Id
    private UUID curveId;
    private String name;
    private LocalDate curveDate;
    private String currency;
    private String index;
    
    @OneToMany(mappedBy = "curve")
    private List<Instrument> instruments;
}

// Adapter (converts to JQuantLib for future valuation)
@Service
public class JQuantLibAdapter {
    public YieldTermStructure toYieldCurve(Curve curve, List<Quote> quotes) {
        // Map domain objects → JQuantLib RateHelper classes
        List<RateHelper> helpers = new ArrayList<>();
        for (Instrument inst : curve.getInstruments()) {
            Quote quote = findQuote(quotes, inst);
            helpers.add(createRateHelper(inst, quote));
        }
        return new PiecewiseYieldCurve<>(helpers, DayCounter, ...);
    }
    
    private RateHelper createRateHelper(Instrument inst, Quote quote) {
        // Convert based on instrument type
        switch (inst.getInstrumentType()) {
            case "MoneyMarket": return new DepositRateHelper(...);
            case "Future": return new FuturesRateHelper(...);
            case "Swap": return new SwapRateHelper(...);
        }
    }
}

// Valuation service (future feature)
@Service
public class ValuationService {
    @Autowired
    private JQuantLibAdapter adapter;
    
    public double calculateNPV(UUID curveId, Swap swap) {
        Curve curve = curveRepo.findById(curveId);
        List<Quote> quotes = quoteRepo.findByCurve(curve);
        YieldTermStructure yieldCurve = adapter.toYieldCurve(curve, quotes);
        return swap.NPV(yieldCurve);
    }
}
```

**Alternatives considered**:
- **Domain models extend JQuantLib classes**: Couples persistence to calculation library, violates separation of concerns
- **No adapter layer**: Direct JQuantLib usage in services is less testable, harder to mock

---

### 6. Eastern Time Handling (EST/EDT)

**Question**: How to handle "5 PM Eastern Time" that follows DST (EST in winter, EDT in summer)?

**Decision**: **Use `ZoneId.of("America/New_York")` for automatic DST handling**

**Rationale**:
- Java's `ZoneId` automatically handles DST transitions
- `America/New_York` is the canonical IANA timezone for Eastern Time
- Server-side conversion ensures consistency (per spec assumption)

**Implementation**:
```java
@Service
public class DateTimeService {
    private static final ZoneId EASTERN = ZoneId.of("America/New_York");
    
    public ZonedDateTime toCurveDateTime(LocalDate date) {
        // Convert date to 5 PM Eastern (EST or EDT depending on date)
        return date.atTime(17, 0).atZone(EASTERN);
    }
    
    public Instant toCurveInstant(LocalDate date) {
        // Store as UTC Instant in database
        return toCurveDateTime(date).toInstant();
    }
}
```

**Database storage**: Store as `TIMESTAMP WITH TIME ZONE` in UTC, convert to Eastern for display

**Alternatives considered**:
- **Hardcode UTC-5**: Breaks during DST (should be UTC-4 in summer)
- **Client-side timezone**: Inconsistent across users, violates spec requirement for server-side time

---

### 7. Reverse Proxy Configuration

**Question**: How to configure Nginx to route `/marketdata/*` to React app while preserving existing `/trade/*` routes?

**Decision**: **Path-based routing with separate upstream servers**

**Rationale**:
- Nginx excels at path-based routing
- Each team's app runs on different port (marketdata-ui:3000, trade-ui:5001)
- Shared authentication can be handled via proxy_set_header

**Nginx configuration**:
```nginx
upstream marketdata-ui {
    server marketdata-ui:3000;
}

upstream trade-ui {
    server trade-ui:5001;
}

server {
    listen 80;
    server_name localhost;
    
    # Marketdata module
    location /marketdata {
        proxy_pass http://marketdata-ui;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
    
    # Trade module (existing)
    location /trade {
        proxy_pass http://trade-ui;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
    }
    
    # API gateway (optional - direct backend access)
    location /api/marketdata {
        proxy_pass http://marketdata-service:8080/api;
    }
    
    # Default to trade module
    location / {
        proxy_pass http://trade-ui;
    }
}
```

**Alternatives considered**:
- **Subdomain routing** (marketdata.localhost): Requires DNS setup, more complex
- **API Gateway** (Kong, Traefik): Overkill for 2 services

---

### 8. Docker Compose Orchestration

**Question**: How to integrate marketdata service + UI into existing Docker Compose setup?

**Decision**: **Add 2 new services to compose.yml, share PostgreSQL instance**

**Docker Compose additions**:
```yaml
services:
  # Existing services (postgres, trade-api, trade-ui, rabbitmq, etc.)
  
  marketdata-service:
    build: ./backend/marketdata-service
    ports:
      - "8080:8080"
    environment:
      SPRING_DATASOURCE_URL: jdbc:postgresql://postgres:5432/marketdata
      SPRING_DATASOURCE_USERNAME: ${DB_USER}
      SPRING_DATASOURCE_PASSWORD: ${DB_PASSWORD}
    depends_on:
      - postgres
    networks:
      - derivative-network
  
  marketdata-ui:
    build: ./frontend/marketdata-ui
    ports:
      - "3000:3000"
    environment:
      VITE_API_BASE_URL: http://marketdata-service:8080
    depends_on:
      - marketdata-service
    networks:
      - derivative-network
  
  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
    volumes:
      - ./infra/nginx/nginx.conf:/etc/nginx/nginx.conf:ro
    depends_on:
      - marketdata-ui
      - trade-ui
    networks:
      - derivative-network

networks:
  derivative-network:
    driver: bridge
```

**Database setup**: Create `marketdata` database in existing PostgreSQL instance, separate schema from `trades` database

**Alternatives considered**:
- **Separate PostgreSQL container**: Adds complexity, no benefit for team autonomy
- **Kubernetes**: Overkill for local development/small deployment

---

## Summary of Decisions

| Question | Decision | Rationale |
|----------|----------|-----------|
| Frontend framework | React 18 | Larger ecosystem, better financial UI libraries, TypeScript support |
| Backend framework | Spring Boot 3.2.x | Minimal dependencies, JPA/PostgreSQL, Java 17 compatible |
| Frontend testing | Jest + React Testing Library + Vitest | Industry standard, user-centric testing |
| Database schema | 3 tables with composite unique constraints | Temporal versioning via (name, date), orphaned quotes supported |
| JQuantLib integration | Adapter pattern, separate from domain models | Clean separation, lightweight entities, testable |
| Eastern Time handling | `ZoneId.of("America/New_York")` | Automatic DST handling via Java ZoneId |
| Reverse proxy | Nginx path-based routing | Simple, standard, supports micro-frontends |
| Docker orchestration | 2 new services in compose.yml | Share PostgreSQL instance, minimal changes |

## Next Steps (Phase 1)

1. Generate `data-model.md` with detailed entity schemas
2. Generate `contracts/openapi.yaml` with full API specification
3. Generate `quickstart.md` with setup instructions
4. Update agent context file with new technologies (React, Spring Boot, JQuantLib)
