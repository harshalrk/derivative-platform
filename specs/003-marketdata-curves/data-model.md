# Data Model: Market Data Curve & Quote Management

**Feature**: 003-marketdata-curves
**Date**: 2025-12-25
**Purpose**: Define entities, relationships, and constraints for curves, instruments, and quotes

## Entity Relationship Diagram

```
┌─────────────────────────┐
│       Curve             │
│─────────────────────────│
│ curve_id (PK, UUID)     │
│ name (VARCHAR)          │
│ curve_date (TIMESTAMP)  │ ← Always 5 PM Eastern (EST/EDT)
│ currency (VARCHAR)      │
│ index_name (VARCHAR)    │
│ created_at (TIMESTAMP)  │
└───────────┬─────────────┘
            │ 1
            │
            │ * (one-to-many)
            │
┌───────────▼─────────────┐
│     Instrument          │
│─────────────────────────│
│ instrument_id (PK, UUID)│
│ curve_id (FK)           │
│ instrument_type (ENUM)  │ ← MoneyMarket, Future, Swap
│ tenor (VARCHAR)         │ ← ON, 1M, 3M, 6M, 1Y, 2Y, 5Y, 10Y, 30Y
└───────────┬─────────────┘
            │ 1
            │
            │ 0..1 (zero-or-one-to-one)
            │
┌───────────▼─────────────┐
│        Quote            │
│─────────────────────────│
│ quote_id (PK, UUID)     │
│ instrument_id (FK)      │
│ value (DECIMAL(10,2))   │ ← 2 decimal places, can be negative
│ created_at (TIMESTAMP)  │
│ updated_at (TIMESTAMP)  │
└─────────────────────────┘

Notes:
- Relationship: Curve 1 → * Instruments → 0..1 Quote
- Orphaned quotes: Quotes remain if instrument deleted (no CASCADE)
- Unique constraints enforce data integrity (see schemas below)
```

## Entity Schemas

### 1. Curve

**Purpose**: Represents a market data curve at a specific point in time (temporal versioning)

**Attributes**:

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| `curve_id` | UUID | PRIMARY KEY | Unique identifier |
| `name` | VARCHAR(100) | NOT NULL | Curve identifier across dates (e.g., "USD-SOFR") |
| `curve_date` | TIMESTAMP | NOT NULL | Date + 5 PM Eastern time (EST/EDT), stored as UTC Instant |
| `currency` | VARCHAR(3) | NOT NULL | ISO currency code (USD, EUR, GBP, JPY) |
| `index_name` | VARCHAR(50) | NOT NULL | Interest rate index (SOFR, LIBOR, EURIBOR, SONIA) |
| `created_at` | TIMESTAMP | DEFAULT NOW | Record creation timestamp |

**Unique Constraints**:
- `UNIQUE (name, curve_date)` - Each curve name+date combination is unique

**Indexes**:
- `CREATE INDEX idx_curves_name ON curves(name);` - Fast lookup by name
- `CREATE INDEX idx_curves_date ON curves(curve_date);` - Fast lookup by date

**Validation Rules** (enforced in application layer):
- FR-001: Name, currency, index, date required
- FR-025: Cannot save without at least 1 instrument (checked via instruments relationship)
- FR-026: Unique name+date (enforced by database constraint)

**Example**:
```json
{
  "curve_id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "USD-SOFR",
  "curve_date": "2025-12-25T22:00:00Z",  // 5 PM EST = 10 PM UTC in winter
  "currency": "USD",
  "index_name": "SOFR",
  "created_at": "2025-12-25T14:30:00Z"
}
```

---

### 2. Instrument

**Purpose**: Represents a point on a curve's term structure (instrument type + tenor)

**Attributes**:

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| `instrument_id` | UUID | PRIMARY KEY | Unique identifier |
| `curve_id` | UUID | FOREIGN KEY | References curves(curve_id) |
| `instrument_type` | VARCHAR(20) | NOT NULL | MoneyMarket, Future, or Swap |
| `tenor` | VARCHAR(10) | NOT NULL | ON, 1M, 3M, 6M, 1Y, 2Y, 5Y, 10Y, 30Y |

**Unique Constraints**:
- `UNIQUE (curve_id, tenor)` - Each tenor appears only once per curve version

**Indexes**:
- `CREATE INDEX idx_instruments_curve ON instruments(curve_id);` - Fast lookup of instruments for a curve

**Foreign Keys**:
- `curve_id` references `curves(curve_id)` with **NO CASCADE DELETE** (allows orphaned quotes)

**Validation Rules** (enforced in application layer):
- FR-003: instrument_type must be in [MoneyMarket, Future, Swap]
- FR-004: tenor must be in [ON, 1M, 3M, 6M, 1Y, 2Y, 5Y, 10Y, 30Y]
- FR-005: No duplicate tenors within a curve version (enforced by database constraint)
- FR-006: At least 1 instrument required per curve (checked when saving curve)

**Example**:
```json
{
  "instrument_id": "660e9511-f30c-52e5-b827-557766551111",
  "curve_id": "550e8400-e29b-41d4-a716-446655440000",
  "instrument_type": "MoneyMarket",
  "tenor": "ON"
}
```

---

### 3. Quote

**Purpose**: Represents market data value for a specific instrument in a specific curve version

**Attributes**:

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| `quote_id` | UUID | PRIMARY KEY | Unique identifier |
| `instrument_id` | UUID | FOREIGN KEY | References instruments(instrument_id) |
| `value` | DECIMAL(10, 2) | NOT NULL | Quote value with 2 decimal places |
| `created_at` | TIMESTAMP | DEFAULT NOW | First save timestamp |
| `updated_at` | TIMESTAMP | DEFAULT NOW | Last update timestamp |

**Indexes**:
- `CREATE INDEX idx_quotes_instrument ON quotes(instrument_id);` - Fast lookup of quotes for instruments

**Foreign Keys**:
- `instrument_id` references `instruments(instrument_id)` with **NO CASCADE DELETE** (orphaned quotes allowed)

**Validation Rules** (enforced in application layer):
- FR-014, FR-017: All instruments must have quotes when saving (business logic check)
- FR-015: Value can be positive or negative (no sign constraint)
- FR-016: Exactly 2 decimal places (enforced by DECIMAL(10, 2) type)
- FR-020: Overwriting existing quotes replaces value and updates updated_at

**Example**:
```json
{
  "quote_id": "770f0622-041d-63f6-c938-668877662222",
  "instrument_id": "660e9511-f30c-52e5-b827-557766551111",
  "value": 5.25,
  "created_at": "2025-12-25T15:00:00Z",
  "updated_at": "2025-12-25T16:30:00Z"
}
```

---

## Reference Data (Hardcoded Enumerations)

### Currency
**Values**: `USD`, `EUR`, `GBP`, `JPY`
**Storage**: VARCHAR(3) in curves table
**Future**: Designed to be replaced by external reference data service (FR-028)

### Index
**Values**: `SOFR`, `LIBOR`, `EURIBOR`, `SONIA`
**Storage**: VARCHAR(50) in curves table
**Future**: Designed to be replaced by external reference data service (FR-028)

### InstrumentType
**Values**: `MoneyMarket`, `Future`, `Swap`
**Storage**: VARCHAR(20) in instruments table
**Future**: Designed to be replaced by external reference data service (FR-028)

### Tenor
**Values**: `ON`, `1M`, `3M`, `6M`, `1Y`, `2Y`, `5Y`, `10Y`, `30Y`
**Storage**: VARCHAR(10) in instruments table
**Future**: Designed to be replaced by external reference data service (FR-028)

---

## Data Integrity Rules

### Temporal Versioning
- Each `(name, curve_date)` combination is unique
- Multiple curve versions for same name across different dates
- Example: "USD-SOFR" can have versions for 2025-12-24, 2025-12-25, 2025-12-26

### Curve Structure Modification
- Users can edit curve structure (add/remove instruments) after creation
- FR-008: Editing allowed even if quotes exist
- Removing instrument leaves orphaned quotes (no CASCADE DELETE)
- Adding instrument requires user to enter new quotes (FR-014)

### Quote Completeness
- FR-013, FR-014, FR-017: All instruments in current curve structure must have quotes when saving
- Partial quotes not allowed
- Missing quotes trigger validation error with specific instrument names

### Orphaned Data Scenarios
1. **Instrument deleted from curve**: Associated quote remains in database (orphaned)
2. **Curve version deleted**: Instruments and quotes remain (orphaned)
3. **Rationale**: Preserves historical data, no accidental data loss

### Concurrent Editing
- Last save wins (no conflict resolution)
- FR-020: Overwriting existing quotes updates `updated_at` timestamp
- No optimistic locking or version tracking

---

## Database Migration Scripts

### V1: Create curves table
```sql
CREATE TABLE curves (
    curve_id UUID PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    curve_date TIMESTAMP NOT NULL,
    currency VARCHAR(3) NOT NULL,
    index_name VARCHAR(50) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_curve_name_date UNIQUE (name, curve_date),
    CONSTRAINT chk_currency CHECK (currency IN ('USD', 'EUR', 'GBP', 'JPY')),
    CONSTRAINT chk_index CHECK (index_name IN ('SOFR', 'LIBOR', 'EURIBOR', 'SONIA'))
);

CREATE INDEX idx_curves_name ON curves(name);
CREATE INDEX idx_curves_date ON curves(curve_date);
```

### V2: Create instruments table
```sql
CREATE TABLE instruments (
    instrument_id UUID PRIMARY KEY,
    curve_id UUID NOT NULL,
    instrument_type VARCHAR(20) NOT NULL,
    tenor VARCHAR(10) NOT NULL,
    CONSTRAINT fk_instruments_curve FOREIGN KEY (curve_id) 
        REFERENCES curves(curve_id),
    CONSTRAINT uq_curve_tenor UNIQUE (curve_id, tenor),
    CONSTRAINT chk_instrument_type CHECK (instrument_type IN ('MoneyMarket', 'Future', 'Swap')),
    CONSTRAINT chk_tenor CHECK (tenor IN ('ON', '1M', '3M', '6M', '1Y', '2Y', '5Y', '10Y', '30Y'))
);

CREATE INDEX idx_instruments_curve ON instruments(curve_id);
```

### V3: Create quotes table
```sql
CREATE TABLE quotes (
    quote_id UUID PRIMARY KEY,
    instrument_id UUID NOT NULL,
    value DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_quotes_instrument FOREIGN KEY (instrument_id) 
        REFERENCES instruments(instrument_id)
);

CREATE INDEX idx_quotes_instrument ON quotes(instrument_id);
```

---

## JPA Entity Mappings (Java)

### Curve.java
```java
@Entity
@Table(name = "curves", uniqueConstraints = {
    @UniqueConstraint(columnNames = {"name", "curve_date"})
})
public class Curve {
    @Id
    @GeneratedValue
    private UUID curveId;
    
    @Column(nullable = false, length = 100)
    private String name;
    
    @Column(nullable = false)
    private Instant curveDate;  // Store as UTC, convert to/from Eastern
    
    @Column(nullable = false, length = 3)
    private String currency;
    
    @Column(nullable = false, length = 50)
    private String indexName;
    
    @CreationTimestamp
    private Instant createdAt;
    
    @OneToMany(mappedBy = "curve", cascade = CascadeType.ALL, orphanRemoval = false)
    private List<Instrument> instruments = new ArrayList<>();
    
    // Validation method
    public void validate() {
        if (instruments.isEmpty()) {
            throw new ValidationException("At least one instrument required");
        }
    }
}
```

### Instrument.java
```java
@Entity
@Table(name = "instruments", uniqueConstraints = {
    @UniqueConstraint(columnNames = {"curve_id", "tenor"})
})
public class Instrument {
    @Id
    @GeneratedValue
    private UUID instrumentId;
    
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "curve_id", nullable = false)
    private Curve curve;
    
    @Column(nullable = false, length = 20)
    private String instrumentType;
    
    @Column(nullable = false, length = 10)
    private String tenor;
    
    @OneToOne(mappedBy = "instrument", cascade = CascadeType.ALL, orphanRemoval = false)
    private Quote quote;
}
```

### Quote.java
```java
@Entity
@Table(name = "quotes")
public class Quote {
    @Id
    @GeneratedValue
    private UUID quoteId;
    
    @OneToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "instrument_id", nullable = false)
    private Instrument instrument;
    
    @Column(nullable = false, precision = 10, scale = 2)
    private BigDecimal value;
    
    @CreationTimestamp
    private Instant createdAt;
    
    @UpdateTimestamp
    private Instant updatedAt;
}
```

---

## Query Patterns

### Get curve with instruments and quotes
```java
@Query("SELECT c FROM Curve c " +
       "LEFT JOIN FETCH c.instruments i " +
       "LEFT JOIN FETCH i.quote " +
       "WHERE c.name = :name AND c.curveDate = :date")
Curve findByNameAndDate(@Param("name") String name, @Param("date") Instant date);
```

### Find previous curve for rolling
```java
@Query("SELECT c FROM Curve c " +
       "WHERE c.name = :name AND c.curveDate < :targetDate " +
       "ORDER BY c.curveDate DESC")
List<Curve> findPreviousCurves(@Param("name") String name, 
                                @Param("targetDate") Instant targetDate);
```

### Check if all instruments have quotes
```java
public boolean hasCompleteQuotes(Curve curve) {
    long instrumentCount = curve.getInstruments().size();
    long quoteCount = curve.getInstruments().stream()
        .filter(i -> i.getQuote() != null)
        .count();
    return instrumentCount == quoteCount && instrumentCount > 0;
}
```

---

## Data Model Summary

| Entity | Primary Key | Unique Constraints | Foreign Keys | Cascade |
|--------|-------------|-------------------|--------------|---------|
| Curve | curve_id | (name, curve_date) | None | N/A |
| Instrument | instrument_id | (curve_id, tenor) | curve_id → curves | None |
| Quote | quote_id | None | instrument_id → instruments | None |

**Key Design Decisions**:
1. **UUID primary keys**: Distributed system friendly, no auto-increment issues
2. **No cascade deletes**: Preserves historical data, prevents accidental loss
3. **Temporal versioning**: (name, date) uniqueness allows curve evolution over time
4. **2 decimal precision**: DECIMAL(10, 2) enforces FR-016
5. **Server-side timestamps**: Instant (UTC) for consistency, convert to Eastern for display

---

## Reverse Proxy Configuration

**Purpose**: Nginx routes traffic to marketdata-ui and marketdata-service based on URL path

**Nginx Configuration** (`infra/nginx/nginx.conf`):
```nginx
server {
    listen 80;
    
    # Marketdata UI (React app with basename)
    location /marketdata {
        proxy_pass http://marketdata-ui:3000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
    
    # Marketdata API
    location /api/marketdata {
        # Rewrite /api/marketdata → /api
        rewrite ^/api/marketdata/(.*)$ /api/$1 break;
        proxy_pass http://marketdata-service:8080;
        proxy_set_header Host $host;
    }
    
    # Trade UI (existing)
    location /trade {
        proxy_pass http://ui:8080;
    }
    
    # Trade API (existing)
    location /api/trade {
        rewrite ^/api/trade/(.*)$ /api/$1 break;
        proxy_pass http://api:8080;
    }
}
```

**React Router Configuration** (`frontend/marketdata-ui/src/main.tsx`):
```typescript
import { BrowserRouter } from 'react-router-dom';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <BrowserRouter basename="/marketdata">
    <App />
  </BrowserRouter>
);
```

**Important**: React app must use `basename="/marketdata"` so routes like `/define` become `/marketdata/define` in browser.
