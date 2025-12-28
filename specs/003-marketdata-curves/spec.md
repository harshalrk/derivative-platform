# Feature Specification: Market Data Curve & Quote Management

**Feature Branch**: `003-marketdata-curves`  
**Created**: 2025-12-25  
**Status**: Draft  
**Input**: User description: "Marketdata service for defining curves with term structures, managing quotes by valuation date, and rolling quotes to new dates"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Define Market Data Curves (Priority: P1)

A market data manager navigates to the marketdata module (via shell navigation), accesses the curve definition screen (React/Vue UI), and creates a new curve by providing a name (e.g., "USD-SOFR"), currency (USD), index (SOFR), and date in ISO format (YYYY-MM-DD, automatically set to 5 PM Eastern Time). They build the term structure by adding instruments - selecting instrument type (MoneyMarket, Future, or Swap) and tenor (ON, 1M, 3M, 6M, 1Y, 2Y, 5Y, 10Y, 30Y) for each point on the curve. Each tenor can only appear once in the curve. At least one instrument must be added. Once defined, they save the curve configuration for that date. Quotes can be entered later as a separate operation.

**Why this priority**: Curve definitions are foundational - without curves, there's nothing to quote. This must work first before any quote management functionality.

**Independent Test**: Can be fully tested by creating a curve with 3-5 instruments of different types and tenors for a specific date, saving it without quotes, and verifying it can be retrieved. Delivers immediate value by establishing the curve catalog. Tests the full stack owned by marketdata team.

**Acceptance Scenarios**:

1. **Given** a user is on the curve definition screen, **When** they enter curve name "USD-SOFR", currency "USD", index "SOFR", and date "2025-12-25", **Then** the system accepts these core curve attributes and automatically sets the time to 5 PM Eastern Time (EST/EDT depending on season)
2. **Given** the user is building a term structure, **When** they add an instrument by selecting type "MoneyMarket" and tenor "ON", **Then** the instrument is added to the curve's term structure
3. **Given** the user has added multiple instruments to the term structure, **When** they click Save, **Then** the curve is persisted with all its instruments for that date (5 PM Eastern Time) and can be retrieved later by name and date
4. **Given** the user tries to save a curve without adding any instruments, **When** they click Save, **Then** the system shows an error: "At least one instrument is required to create a curve"
5. **Given** the user has added an instrument with tenor "3M", **When** they try to add another instrument with tenor "3M", **Then** the system shows an error: "Duplicate tenor not allowed. Tenor 3M already exists in this curve."
6. **Given** the user is adding instruments, **When** they select from available instrument types and tenors, **Then** they see all standard options from hardcoded lists (MoneyMarket, Future, Swap for types; ON, 1M, 3M, 6M, 1Y, 2Y, 5Y, 10Y, 30Y for tenors)
7. **Given** a curve already exists with the same name and date, **When** the user tries to save, **Then** the system shows an error requiring unique curve name+date combinations

---

### User Story 2 - Manage Daily Quotes (Priority: P1)

A market data operator selects an existing curve by name and date. The system displays all instruments in that curve version's term structure. The operator enters quote values (with 2 decimal place precision) for ALL instruments (all quotes required). If quotes already exist for that curve version, the existing values are prepopulated. When the operator saves, the quotes are stored for that curve version (overwriting existing quotes if present).

**Why this priority**: Quote entry is the core operational workflow - equally critical as curve definition. Market data is useless without actual quote values.

**Independent Test**: Can be tested by selecting an existing curve by name+date, entering quotes (e.g., 5.25, -0.50) for all instruments, saving them, then reopening the same curve to verify quotes are prepopulated. Delivers immediate value by capturing daily market data.

**Acceptance Scenarios**:

1. **Given** a user selects curve "USD-SOFR" with date "2025-12-25", **When** the system loads the quote entry screen, **Then** it displays all instruments from that curve version's term structure with empty quote fields (if no existing quotes)
2. **Given** the user is viewing the quote entry screen, **When** they enter quote values for each instrument (e.g., "5.25" for the ON instrument), **Then** the system accepts numeric quote values with 2 decimal places (including negative values like "-0.50")
3. **Given** the user has entered quotes for all instruments, **When** they click Save, **Then** the quotes are persisted for that curve version (name + date at 5 PM Eastern Time) with 2 decimal precision
4. **Given** the user tries to save with missing quote values, **When** they click Save, **Then** the system shows an error: "All instruments must have quote values. Missing quotes for: [list of instruments]"
5. **Given** quotes already exist for the selected curve version, **When** the user opens the quote entry screen, **Then** existing quote values are prepopulated in the input fields
6. **Given** the user modifies prepopulated quotes and saves, **When** the save completes, **Then** the new values overwrite the existing quotes for that curve version

---

### User Story 3 - Roll Curves and Quotes to New Dates (Priority: P2)

A market data operator needs to copy both a curve definition and its quotes from the most recent prior date to a new date (e.g., rolling Friday's curve and quotes to Monday when markets are closed). They select a curve name and the target date. The system automatically finds the most recent previous date that has both a curve definition and complete quotes for that curve. It then copies both the curve structure (with all instruments) and all quote values forward to the target date. If the previous date is missing quotes for any instruments, the system notifies the user and prevents the roll operation.

**Why this priority**: Rolling is a convenience feature that saves time but isn't required for basic operations. Can be implemented after core curve and quote management works.

**Independent Test**: Can be tested by creating a curve with complete quotes on one date, using the roll function with a future date, and verifying both the curve structure and quote values are copied. Delivers value by reducing manual re-entry when adding/removing instruments.

**Acceptance Scenarios**:

1. **Given** curve "USD-SOFR" with date "2025-12-22" has complete quotes, **When** user selects the curve name and target date "2025-12-25" and clicks Roll, **Then** the system copies both the curve definition and all quote values from "2025-12-22" to "2025-12-25"
2. **Given** the previous curve date has quotes but is missing values for some instruments, **When** the user tries to roll, **Then** the system shows an error: "Cannot roll: Curve date (2025-12-22) is missing quotes for instruments: [list of instruments]. All instruments must have quotes to roll."
3. **Given** there is no previous curve date with quotes for the selected curve, **When** the user tries to roll, **Then** the system shows an error: "No previous curve version with quotes found to roll"
4. **Given** the target date already has a curve version with quotes, **When** the user executes a roll operation, **Then** the system warns about overwriting and requires confirmation before proceeding
5. **Given** a successful roll operation, **When** the user selects the curve and target date, **Then** both the curve structure and all instrument quote values match the previous instance
6. **Given** the user rolled a curve and then adds a new instrument to the target date curve, **When** they save, **Then** the new curve structure is saved independently for that date

---

### Edge Cases

- What happens when a user selects a curve name+date combination that doesn't exist?
- What happens when a user tries to enter quotes for a curve version that has no instruments defined?
- What happens when a user tries to enter quotes for a curve version that was just created but hasn't been saved yet?
- What if there are multiple prior curve dates with quotes - which one is "previous"? (Answer: most recent date before target date)
- What happens when rolling to the same date as the previous instance (should this be prevented)?
- For multi-service features: What happens when the marketdata service is unavailable?
- How are service timeouts handled for large curves with many instruments?
- What happens if a user tries to save quotes without selecting a curve?
- How does the system handle invalid date selections (e.g., invalid date format)?
- How does the system handle concurrent edits to the same curve version or quotes by multiple users? (Answer: last save wins)
- What happens if a user deletes a curve version that has quotes associated with it? (Answer: quotes remain as orphaned data)
- What happens if a user edits a curve version's structure (adds/removes instruments) after quotes exist? (Answer: allowed - user must add quotes for new instruments; removed instruments leave orphaned quotes)
- What if a user enters more than 2 decimal places for a quote value (e.g., 5.2525)?
- What happens if the system clock is adjusted during 5 PM Eastern Time processing?
- What if a user tries to roll a curve on the first date ever for that curve (no previous instance exists)?

## Technical Context

**Architecture Pattern**: Micro-frontends with team autonomy. Each team owns their full stack (frontend + backend + database).

**Marketdata Team Stack**:
- Frontend: React 18 SPA with TypeScript
- Backend: Java 17 + Spring Boot + Maven
- Database: PostgreSQL (marketdata schema in existing instance)
- Integration: JQuantLib for valuation/calculation (not for domain models)

**Trade Team Stack** (existing):
- Frontend: Blazor Server
- Backend: .NET 8 API + Marten (event sourcing)
- Database: PostgreSQL (trades schema)

**Shell Integration**:
- Reverse proxy (Nginx or API Gateway) routes `/marketdata/*` to marketdata module, `/trade/*` to trade module
- Shared navigation component (web component or HTML)
- Shared authentication/authorization service (JWT tokens)

**Inter-Service Communication**: HTTP/JSON APIs with circuit breakers. Trade services may call marketdata APIs for curve/quote lookups during pricing operations.

**JQuantLib Integration Pattern**:
```java
// Domain Model (lightweight, for persistence/API)
@Entity
public class Curve {
    private String name;
    private LocalDate date;
    private String currency;
    private String index;
    private List<Instrument> instruments;
}

// Adapter (converts domain to JQuantLib for calculations)
public class JQuantLibAdapter {
    public YieldTermStructure toYieldCurve(Curve curve, List<Quote> quotes) {
        // Map domain objects → JQuantLib classes
        // Use PiecewiseYieldCurve with RateHelpers
    }
}

// Valuation Service (uses JQuantLib)
public class ValuationService {
    public double calculateNPV(Curve curve, List<Quote> quotes, Swap swap) {
        YieldTermStructure yieldCurve = adapter.toYieldCurve(curve, quotes);
        return swap.NPV(yieldCurve);
    }
}
```

### API Endpoints (Java Service)

**Reference Data**:
- `GET /api/reference/currencies` → `["USD", "EUR", "GBP", "JPY"]`
- `GET /api/reference/indexes` → `["SOFR", "LIBOR", "EURIBOR", "SONIA"]`
- `GET /api/reference/instrument-types` → `["MoneyMarket", "Future", "Swap"]`
- `GET /api/reference/tenors` → `["ON", "1M", "3M", "6M", "1Y", "2Y", "5Y", "10Y", "30Y"]`

**Curve Management**:
- `POST /api/curves` - Create curve
  - Request: `{ name, currency, index, date, instruments: [{ type, tenor }] }`
  - Response: `201 Created` with curve ID
- `GET /api/curves?name={name}&date={date}` - Get specific curve version
- `GET /api/curves?name={name}` - Get all dates for curve name
- `GET /api/curves` - List all curve names (distinct)
- `PUT /api/curves/{id}` - Update curve structure
  - Request: `{ instruments: [{ type, tenor }] }`
- `DELETE /api/curves?name={name}&date={date}` - Delete curve version

**Quote Management**:
- `POST /api/quotes` - Save/update quotes
  - Request: `{ curveName, curveDate, quotes: [{ instrumentType, tenor, value }] }`
  - Validates all instruments have values (2 decimals)
- `GET /api/quotes?curveName={name}&curveDate={date}` - Get quotes
- `POST /api/quotes/roll` - Roll from previous date
  - Request: `{ curveName, targetDate }`
  - Response: Previous date + rolled curve + quotes
  - Validates previous has complete quotes

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow users to create curves with name, currency, index, and date (ISO format YYYY-MM-DD) as core attributes
- **FR-002**: System MUST allow users to build term structures by adding instruments to curves
- **FR-003**: System MUST support instrument types from hardcoded list: MoneyMarket, Future, Swap
- **FR-004**: System MUST support tenors from hardcoded list: ON, 1M, 3M, 6M, 1Y, 2Y, 5Y, 10Y, 30Y
- **FR-005**: System MUST prevent duplicate tenors within a single curve version (each tenor can appear only once)
- **FR-006**: System MUST require at least one instrument to create a curve
- **FR-007**: System MUST persist saved curves with their date and make them retrievable by name and date
- **FR-008**: System MUST allow users to edit an existing curve structure by selecting curve name and date (adding or removing instruments)
- **FR-009**: System MUST allow users to delete a specific curve instance by curve name and date without deleting associated quotes
- **FR-010**: System MUST allow users to select a curve by name and date to view or edit its term structure
- **FR-011**: System MUST allow users to select a curve by name and date to enter quotes for that curve version
- **FR-012**: System MUST automatically set the time to 5 PM Eastern Time (EST in winter, EDT in summer) for all curve dates
- **FR-013**: System MUST display all instruments from the selected curve version's term structure in the quote entry interface
- **FR-014**: System MUST require numeric quote values for ALL instruments in the current curve structure (no partial quote entry allowed)
- **FR-015**: System MUST accept positive and negative quote values
- **FR-016**: System MUST store and display quote values with exactly 2 decimal places
- **FR-017**: System MUST prevent saving quotes if any instrument in the current curve structure is missing a value
- **FR-018**: System MUST persist quotes with curve name, curve date (5 PM Eastern Time), and quote values per instrument
- **FR-019**: System MUST prepopulate existing quote values when a user selects a curve version that already has quotes
- **FR-020**: System MUST overwrite existing quote values when saving to a curve version that already has quotes
- **FR-021**: System MUST provide a roll function that automatically finds the most recent previous curve date with complete quotes and copies both the curve structure and quotes to the target date
- **FR-022**: System MUST validate that the previous curve date has quotes for ALL instruments before allowing roll operation
- **FR-023**: System MUST notify users with specific instrument names if the previous date is missing quotes for any instruments
- **FR-024**: System MUST warn users when rolling to a target date that already has a curve version and quotes
- **FR-025**: System MUST prevent saving curves without a name, currency, index, date, and at least one instrument
- **FR-026**: System MUST enforce unique curve name+date combinations (no duplicates)
- **FR-027**: System MUST use hardcoded lists for currencies (USD, EUR, GBP, JPY), indexes (SOFR, LIBOR, EURIBOR, SONIA), instrument types, and tenors
- **FR-028**: System MUST be designed to allow future replacement of hardcoded reference data with external service calls
- **FR-029**: System MUST use ISO date format (YYYY-MM-DD) for all curve dates in UI display and input

### Key Entities

- **Curve**: Represents a market data curve at a specific point in time. Contains name (identifier across dates), date in ISO format (YYYY-MM-DD with time hardcoded to 5 PM Eastern Time - EST/EDT), currency (e.g., USD, EUR), index (e.g., SOFR, LIBOR), and a term structure (collection of instruments, minimum 1 required). Each curve name+date combination must be unique. Can exist without quotes.
- **Instrument**: Represents a point on a curve's term structure. Contains instrument type (MoneyMarket, Future, Swap), tenor (ON, 1M, 3M, etc.), and belongs to a specific curve version. Each tenor can appear only once within a curve version.
- **Quote**: Represents market data for a specific instrument in a specific curve version. Contains curve name, curve date (5 PM Eastern Time), instrument identifier, and numeric quote value stored with 2 decimal places (positive or negative). All instruments in a curve version's current structure must have quotes when saving. Quotes can be orphaned if their instrument is removed from curve structure.
- **InstrumentType**: Hardcoded enumeration: MoneyMarket, Future, Swap. Designed to be replaceable with external service.
- **Tenor**: Hardcoded enumeration: ON, 1M, 3M, 6M, 1Y, 2Y, 5Y, 10Y, 30Y. Designed to be replaceable with external service.
- **Currency**: Hardcoded enumeration: USD, EUR, GBP, JPY. Designed to be replaceable with external service.
- **Index**: Hardcoded enumeration: SOFR, LIBOR, EURIBOR, SONIA. Designed to be replaceable with external service.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can define a complete curve with 10+ instruments in under 3 minutes
- **SC-002**: Users can enter and save complete quotes for all instruments on a curve within 2 minutes
- **SC-003**: Quote prepopulation occurs in under 1 second when opening an existing curve version
- **SC-004**: Rolling both curve structure and quotes automatically finds the previous instance and completes in under 2 seconds regardless of curve size
- **SC-005**: System correctly validates no duplicate tenors within a curve (100% detection rate)
- **SC-006**: System correctly identifies and reports missing instrument quotes (100% accuracy in validation)
- **SC-007**: System supports curves with up to 50 instruments without performance degradation
- **SC-008**: 100% of saved curves and quotes are accurately retrieved and prepopulated
- **SC-009**: Zero data loss when overwriting existing curve versions or quotes (new values fully replace old values)
- **SC-010**: Time is consistently set to 5 PM EST for all quotes regardless of user timezone or DST transitions
- **SC-011**: Users can successfully edit and delete specific curve instances by name+date

## Assumptions *(optional)*

- **Architecture**: Micro-frontends pattern with reverse proxy (Nginx/API Gateway) routing to separate team modules
- **Marketdata Team Ownership**: Full stack - React 18 frontend + Java/Maven backend + PostgreSQL database (marketdata schema)
- **Trade Team Ownership**: Full stack - Blazor frontend + .NET API backend + PostgreSQL database (trades schema)
- **Shell Layer**: Shared navigation component, authentication/authorization, unified routing via reverse proxy
- The marketdata service will be built as a Java/Maven microservice with minimal dependencies and HTTP/JSON API endpoints
- The service will expose HTTP/JSON API endpoints
- **JQuantLib Usage**: JQuantLib classes used for valuation/calculation logic, NOT as domain models or JPA entities. Separate lightweight domain models for API/persistence with adapters to convert to JQuantLib types when needed for calculations
- The marketdata module UI will be accessible via route prefix (e.g., /marketdata/*) through reverse proxy
- Curves and quotes are separate entities - curves can exist without quotes, quotes reference curve versions
- Curves are versioned by date - each curve name+date combination represents a unique curve version
- Curve date always includes time of 5 PM Eastern Time (EST in winter, EDT in summer - follows local clock)
- Date format in UI is ISO (YYYY-MM-DD) for consistency
- Users can modify curve structures over time by creating new dated versions
- Users can edit existing curve structures (add/remove instruments) even after quotes exist
- Quote values are decimal numbers with 2 decimal place precision
- Quote values support unlimited positive and negative values (no validation ranges)
- Curve dates are business dates (no validation against calendars in this feature)
- Each tenor can appear only once within a curve version (no duplicate tenors)
- At least one instrument is required to create a curve
- Quotes must be complete for current curve structure - all instruments must have quote values before saving
- Quotes are tied directly to curve version (no separate valuation date concept)
- The system uses server-side time for determining "5 PM Eastern Time" (no client-side time adjustments)
- Concurrent editing is expected to be rare (low user count) - last save wins, no conflict resolution
- Historical curve versions and quote data must be preserved (no automatic deletion/archiving)
- Deleting a curve version does NOT cascade delete its quotes (orphaned quotes may exist)
- Deleting quotes does NOT affect curve version (curve remains)
- When instruments are removed from a curve structure, their quotes become orphaned (not automatically deleted)
- When instruments are added to a curve structure, user must enter quotes for new instruments
- **Reference data (currencies, indexes, instrument types, tenors) will be hardcoded initially but the code should be designed to easily swap in calls to a future reference data service**
- Rolling quotes always uses the most recent previous curve date before the target date (chronologically closest)
- The "previous instance" for rolling includes both the curve structure and complete quotes for all instruments in that version

## Out of Scope *(optional)*

- Real-time quote streaming or feeds
- Quote validation against market bounds (e.g., rejecting unrealistic values)
- Audit trail of who entered/modified curves or quotes
- Quote approval workflow (all quotes are immediately effective)
- Interpolation between quoted instruments
- Curve construction from quotes (building discount/forward curves)
- Integration with external market data vendors (Bloomberg, Reuters)
- Quote versioning within the same valuation date (tracking intraday changes)
- Bulk import of curves or quotes from CSV/Excel
- Export functionality for curves or quotes
- User authentication and authorization (covered by separate feature)
- Calendar integration (business day validation)
- Multi-currency conversion
- Administrative interface for adding new instrument types, tenors, currencies, or indexes (hardcoded for now, future service will provide)
- Cascade delete rules (if curve version deleted, what happens to quotes)
- Conflict resolution for concurrent edits (last save wins)

## Dependencies *(optional)*

- New Java/Maven marketdata service must be created
- Docker Compose must include the marketdata service
- .NET API must add HTTP client to call marketdata service endpoints
- Frontend (Blazor Server) must add curve management and quote entry UI screens
- Database/storage for persisting curves and quotes (could use PostgreSQL, or embedded H2 for Java service)
- Existing trade capture system should eventually consume curves from this service for pricing workflows
- Future reference data service will provide currencies, indexes, instrument types, and tenors (hardcoded for now, code must be designed for easy integration later)

## User Scenarios & Testing *(mandatory)*

<!--
  IMPORTANT: User stories should be PRIORITIZED as user journeys ordered by importance.
  Each user story/journey must be INDEPENDENTLY TESTABLE - meaning if you implement just ONE of them,
  you should still have a viable MVP (Minimum Viable Product) that delivers value.
  
  Assign priorities (P1, P2, P3, etc.) to each story, where P1 is the most critical.
  Think of each story as a standalone slice of functionality that can be:
  - Developed independently
  - Tested independently
  - Deployed independently
  - Demonstrated to users independently
-->

### User Story 1 - [Brief Title] (Priority: P1)

[Describe this user journey in plain language]

**Why this priority**: [Explain the value and why it has this priority level]

**Independent Test**: [Describe how this can be tested independently - e.g., "Can be fully tested by [specific action] and delivers [specific value]"]

**Acceptance Scenarios**:

1. **Given** [initial state], **When** [action], **Then** [expected outcome]
2. **Given** [initial state], **When** [action], **Then** [expected outcome]

---

### User Story 2 - [Brief Title] (Priority: P2)

[Describe this user journey in plain language]

**Why this priority**: [Explain the value and why it has this priority level]

**Independent Test**: [Describe how this can be tested independently]

**Acceptance Scenarios**:

1. **Given** [initial state], **When** [action], **Then** [expected outcome]

---

### User Story 3 - [Brief Title] (Priority: P3)

[Describe this user journey in plain language]

**Why this priority**: [Explain the value and why it has this priority level]

**Independent Test**: [Describe how this can be tested independently]

**Acceptance Scenarios**:

1. **Given** [initial state], **When** [action], **Then** [expected outcome]

---

[Add more user stories as needed, each with an assigned priority]

### Edge Cases

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right edge cases.
-->

- What happens when [boundary condition]?
- How does system handle [error scenario]?

## Requirements *(mandatory)*

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right functional requirements.
-->

### Functional Requirements

- **FR-001**: System MUST [specific capability, e.g., "allow users to create accounts"]
- **FR-002**: System MUST [specific capability, e.g., "validate email addresses"]  
- **FR-003**: Users MUST be able to [key interaction, e.g., "reset their password"]
- **FR-004**: System MUST [data requirement, e.g., "persist user preferences"]
- **FR-005**: System MUST [behavior, e.g., "log all security events"]

*Example of marking unclear requirements:*

- **FR-006**: System MUST authenticate users via [NEEDS CLARIFICATION: auth method not specified - email/password, SSO, OAuth?]
- **FR-007**: System MUST retain user data for [NEEDS CLARIFICATION: retention period not specified]

### Key Entities *(include if feature involves data)*

- **[Entity 1]**: [What it represents, key attributes without implementation]
- **[Entity 2]**: [What it represents, relationships to other entities]

## Success Criteria *(mandatory)*

<!--
  ACTION REQUIRED: Define measurable success criteria.
  These must be technology-agnostic and measurable.
-->

### Measurable Outcomes

- **SC-001**: [Measurable metric, e.g., "Users can complete account creation in under 2 minutes"]
- **SC-002**: [Measurable metric, e.g., "System handles 1000 concurrent users without degradation"]
- **SC-003**: [User satisfaction metric, e.g., "90% of users successfully complete primary task on first attempt"]
- **SC-004**: [Business metric, e.g., "Reduce support tickets related to [X] by 50%"]
