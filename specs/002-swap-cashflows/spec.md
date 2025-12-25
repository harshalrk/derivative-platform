# Feature Specification: Swap Cashflow Generation

**Feature Branch**: `002-swap-cashflows`  
**Created**: 2025-12-25  
**Status**: Draft  
**Input**: User description: "Generate cashflows for financial instruments such as Swaps using JQuantLib. Trade screen should have new tab showing cashflows with key columns. Cashflows generated on demand via Generate button. Error handling with maximum context."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - View Swap Cashflows (Priority: P1)

A trader opens a new or existing swap trade and navigates to a new "Cashflows" tab to view the complete payment schedule. They click a "Generate" button to calculate and display all cashflows (both past and future) with key information like payment dates, notional amounts, fixed/floating rates, and payment amounts. Past cashflows (with payment dates that have already occurred) are visually distinguished from future cashflows.

**Why this priority**: This is the core value proposition - traders need visibility into the complete payment schedule to understand both historical payments and future obligations/receivables. Without this, the feature has no value.

**Independent Test**: Can be fully tested by creating a simple fixed-for-floating swap with some past payment dates, navigating to the Cashflows tab, clicking Generate, and verifying that a table appears with all payment dates and amounts, with past payments visually distinguished. Delivers immediate value by showing the complete payment history and schedule.

**Acceptance Scenarios**:

1. **Given** a trader has opened a new or existing swap trade detail page, **When** they click on the "Cashflows" tab, **Then** they see an empty cashflow table with a "Generate" button
2. **Given** the trader is viewing the Cashflows tab, **When** they click the "Generate" button, **Then** the system calculates all cashflows using JQuantLib and displays them in a table
3. **Given** cashflows have been generated, **When** the trader views the table, **Then** they see columns for: Payment Date, Leg Type (Fixed/Floating), Direction (Pay/Receive), Notional Amount, Rate, Payment Amount, and Currency
4. **Given** the table includes cashflows with past payment dates, **When** the trader views them, **Then** past cashflows are displayed in a visually distinct style (e.g., lighter color, different shading) from future cashflows
5. **Given** cashflows are displayed, **When** the trader switches to another tab and back, **Then** the previously generated cashflows remain visible (no automatic re-generation)

---

### User Story 2 - Comprehensive Error Handling (Priority: P2)

When cashflow generation fails due to invalid trade data, missing market data, or JQuantLib calculation errors, the trader sees a clear error message explaining what went wrong, which trade fields are problematic, and what action they should take.

**Why this priority**: Error handling is critical for usability but can be implemented after basic generation works. Traders must understand why generation failed to fix issues.

**Independent Test**: Can be tested by creating trades with invalid data (e.g., start date after end date, missing reference rate) and verifying that clicking Generate shows specific, actionable error messages rather than generic failures.

**Acceptance Scenarios**:

1. **Given** a swap trade has incomplete data (e.g., missing payment frequency), **When** the trader clicks "Generate", **Then** they see an error message stating "Cannot generate cashflows: Payment frequency is required for [Leg Name]"
2. **Given** a swap trade has invalid dates (e.g., effective date after maturity date), **When** the trader clicks "Generate", **Then** they see an error message: "Invalid trade dates: Effective date ([date]) must be before maturity date ([date])"
3. **Given** the JQuantLib service is unavailable, **When** the trader clicks "Generate", **Then** they see an error message: "Cashflow service unavailable. Please try again in a few moments or contact support if the issue persists."
4. **Given** JQuantLib encounters a calculation error, **When** the trader clicks "Generate", **Then** they see an error message including the JQuantLib error details and affected trade fields

---

### User Story 3 - Multi-Leg Swap Support (Priority: P3)

A trader working with complex swaps (e.g., basis swaps with two floating legs, or swaps with multiple payment frequencies) can generate and view cashflows for all legs in a single unified table, with clear differentiation between legs.

**Why this priority**: Most swaps have two legs (fixed-floating), but some complex instruments have multiple legs. This can be added after basic two-leg support works.

**Independent Test**: Can be tested by creating a swap with two floating legs (basis swap), generating cashflows, and verifying that both legs appear in the table with proper leg identification.

**Acceptance Scenarios**:

1. **Given** a swap has two legs (one fixed, one floating), **When** cashflows are generated, **Then** the table shows separate rows for each leg's payments, clearly labeled
2. **Given** a basis swap has two floating legs, **When** cashflows are generated, **Then** both legs appear with their respective reference rates and spreads
3. **Given** legs have different payment frequencies (e.g., quarterly vs semi-annual), **When** cashflows are generated, **Then** the table correctly shows payments on different schedules aligned to their respective calendars

---

### Edge Cases

- What happens when a swap has already matured (all payment dates in the past)? All cashflows should be shown with past payment styling.
- How does the system handle swaps with payment dates on non-business days?
- What if a swap has a very long tenor (e.g., 30 years) generating hundreds of cashflows?
- For multi-service features: What happens when the JQuantLib cashflow service is unavailable?
- How are service timeouts handled (e.g., complex swap taking too long to calculate)?
- What happens if market data (reference rates, curves) required for floating leg projections is missing?
- How does the system handle swaps with optional cashflows (e.g., callable swaps, caps/floors)?
- What happens if a trade is created with an effective date far in the past (e.g., 10 years ago)?
- How is "today's date" determined for distinguishing past vs. future cashflows (server time, user timezone, market close time)?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a new "Cashflows" tab on the swap trade detail screen for both new and existing trades
- **FR-002**: System MUST display a "Generate" button on the Cashflows tab that triggers cashflow calculation on demand
- **FR-003**: System MUST call a JQuantLib-based cashflow service via HTTP API to generate cashflows
- **FR-004**: System MUST display ALL generated cashflows in a tabular format with the following columns: Payment Date, Leg Type, Direction (Pay/Receive), Notional Amount, Rate, Payment Amount, Currency
- **FR-005**: System MUST visually differentiate past cashflows (payment date before current date) from future cashflows using distinct styling (e.g., lighter color, different background shade, or grayed-out appearance)
- **FR-006**: System MUST preserve generated cashflows when the user navigates away from the tab and returns (within the same session)
- **FR-007**: System MUST validate that all required swap trade data is present before calling the cashflow service (effective date, maturity date, notional, leg configurations, payment frequencies)
- **FR-008**: System MUST display specific, actionable error messages when cashflow generation fails, including details about invalid trade data, missing market data, or service errors
- **FR-009**: System MUST handle JQuantLib service unavailability gracefully with user-friendly error messages
- **FR-010**: System MUST support cashflow generation for swaps with multiple legs (2+ legs)
- **FR-011**: System MUST differentiate between fixed and floating leg cashflows in the display
- **FR-012**: System MUST apply business day conventions to payment dates as configured in the swap trade
- **FR-013**: System MUST include the payment calendar adjustments in calculated payment dates
- **FR-014**: System MUST show floating leg payment amounts as projected values based on forward curves (clearly marked as projections for future cashflows)
- **FR-015**: System MUST handle calculation timeouts with clear error messages (e.g., "Calculation taking too long, please try again")

### Key Entities

- **Cashflow**: Represents a single payment in a swap's schedule (past or future). Includes payment date, amount, currency, leg identifier, direction (pay/receive), rate used for calculation, notional amount for that period, and status indicator (past/future based on payment date vs. current date).
- **Cashflow Request**: Parameters sent to JQuantLib service including trade ID, effective date, maturity date, leg configurations (fixed/floating, rates, spreads, frequencies, day count conventions, calendars), and market data snapshot timestamp.
- **Cashflow Response**: Result from JQuantLib service containing array of Cashflow objects or error details (error code, message, problematic fields).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Traders can view the complete payment schedule (all past and future cashflows) for any swap trade within 5 seconds of clicking Generate
- **SC-002**: Error messages identify the specific problem (e.g., "Missing payment frequency on Floating Leg") in 95% of failure cases
- **SC-003**: System successfully generates cashflows for swaps with tenors up to 30 years without timeout
- **SC-004**: Traders understand why cashflow generation failed and know what action to take (measured by support ticket reduction)
- **SC-005**: Generated cashflows match expected payment schedules when compared to manual calculations (100% accuracy for date generation)
- **SC-006**: System handles JQuantLib service unavailability gracefully without crashing the UI
- **SC-007**: Traders can instantly distinguish past cashflows from future cashflows through visual differentiation (100% of users identify the distinction without explanation)

## Assumptions *(optional)*

- JQuantLib Java library will be integrated into a new Java/Maven microservice (per constitution P6)
- The cashflow service will use minimal dependencies and expose HTTP/JSON API endpoints
- Floating leg cashflows (both past and future) will use forward curve projections from the marketdata service
- Business day adjustments will follow conventions already configured in the swap trade (no new calendar data needed)
- Cashflows are calculated for display purposes only (not stored persistently unless user explicitly saves)
- The UI will use the existing Blazor Server architecture to add the new tab
- Market data (forward curves, discount curves) required for floating leg projections is available from the marketdata service
- Users understand that floating leg amounts are projections, not guaranteed payments (even for past dates, amounts shown are calculated values, not actual payment records)
- Past cashflows are determined by comparing payment date to the current server date/time
- The feature shows scheduled cashflows, not actual payment history (no integration with payment systems)

## Out of Scope *(optional)*

- Exporting cashflows to Excel/CSV (future enhancement)
- Editing or manually adjusting generated cashflows
- Marking cashflows as actually paid or recording actual payment amounts (this feature shows scheduled cashflows only)
- Cashflow generation for non-swap instruments (bonds, options, etc.) - swaps only for this feature
- Real-time updates to cashflows when market data changes
- Cashflow discounting or present value calculations (just nominal cashflows)
- Multiple market data scenarios (what-if analysis)
- Cashflow generation for cancelled or inactive trades

## Dependencies *(optional)*

- JQuantLib library (Java) for cashflow calculation engine
- New Java/Maven cashflow microservice must be created
- Marketdata service (Java/Maven) must provide forward curve and discount curve APIs
- Frontend (Blazor Server) must support tabbed navigation on trade detail pages
- .NET API must add HTTP client to call JQuantLib cashflow service
- Docker Compose must include the new JQuantLib cashflow service
- Existing swap trade data model must include all fields required by JQuantLib (frequencies, conventions, calendars, reference rates)
- **SC-004**: [Business metric, e.g., "Reduce support tickets related to [X] by 50%"]
