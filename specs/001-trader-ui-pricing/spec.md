# Feature Specification: Trader UI + Pricing (No Auth)

**Feature Branch**: `001-trader-ui-pricing`  
**Created**: 2025-12-14  
**Status**: Draft  
**Input**: Users enter name (no authentication). Professional trader UI. Pages: name entry, trades list (booked trades), create new trade, and price trade using random market data (deterministic per seed). Accuracy not required.

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

### User Story 1 - Enter Name to Access (Priority: P1)

Users land on an entry page and provide their name to enter the system. No authentication; name establishes session identity for UI filtering.

**Why this priority**: Unblocks all subsequent flows and establishes user context without auth.

**Independent Test**: Enter a name and confirm navigation to trades list with session name displayed.

**Acceptance Scenarios**:

1. **Given** the entry page, **When** the user inputs a non-empty name and continues, **Then** the system sets session display name and routes to trades list.
2. **Given** the entry page, **When** the user submits an empty name, **Then** the UI prevents submission and displays a clear validation message.

---

### User Story 2 - View Booked Trades (Priority: P2)

Users can view a list of trades they have booked (filtered by the current session name). List shows key fields and supports basic sorting.

**Why this priority**: Core visibility for traders; establishes baseline UI professionalism.

**Independent Test**: Book sample trades and confirm the list renders only the user’s trades.

**Acceptance Scenarios**:

1. **Given** existing trades, **When** the user opens the Trades page, **Then** the list shows only trades with `bookedBy` matching the session name.
2. **Given** many trades, **When** the user clicks on the `Trade Date` header, **Then** trades sort ascending/descending.

---

### User Story 3 - Create and Price Trade (Priority: P3)

Users can create a new trade (minimal fields) and request a price using random market data. Accuracy is not required; pricing must be deterministic for a given random seed.

**Why this priority**: Enables the core workflow of trade capture and indicative pricing.

**Independent Test**: Submit a trade form and receive a price response from the backend.

**Acceptance Scenarios**:

1. **Given** the New Trade page, **When** the user inputs required fields and submits, **Then** the system persists the trade associated with `bookedBy` = session name.
2. **Given** a newly created trade, **When** the user clicks “Price Trade”, **Then** the backend returns a JSON price using random market data with a fixed seed provided by the frontend.
3. **Given** invalid inputs (e.g., non-positive quantity), **When** submitting, **Then** the UI displays clear validation messages and prevents submission.

---

[Add more user stories as needed, each with an assigned priority]

### Edge Cases

- Name entry empty or whitespace-only: block and prompt for a valid name.
- Trades list with zero trades: show an empty state with guidance.
- Pricing request when backend is unavailable: display non-blocking error with retry option.
- Large quantities or extreme prices: accept but display with proper formatting; no precision guarantees beyond two decimals for display.

## Requirements *(mandatory)*

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right functional requirements.
-->

### Functional Requirements

- **FR-001**: System MUST present a name entry page and store the display name for the session.
- **FR-002**: System MUST render a Trades list filtered by the session name (`bookedBy`).
- **FR-003**: System MUST provide a New Trade form with required fields: `instrument`, `side` (Buy/Sell), `quantity`, `price`, `currency`, `tradeDate`.
- **FR-004**: System MUST validate inputs client-side and server-side (non-empty, supported currency, positive quantity).
- **FR-005**: System MUST persist trades and associate them with the session `bookedBy` name.
- **FR-006**: System MUST provide a Pricing endpoint that returns a price using random market data with a provided seed to ensure deterministic output per request.
- **FR-007**: System MUST display pricing results clearly (price and currency), with two-decimal display precision.
- **FR-008**: System MUST expose deterministic HTTP JSON APIs consumed by the UI for trade capture and pricing (no authentication).
- **FR-009**: System MUST handle errors clearly in HTTP responses with human-readable message and a simple error code.

### Key Entities

- **Trade**: Represents a booked trade with attributes for instrument, side, quantity, price, currency, tradeDate, bookedBy.
- **PricingRequest**: Represents a request to price a trade with a random seed and optional market data flags.
- **PricingResult**: Represents indicative pricing output with price and currency.

## Success Criteria *(mandatory)*

<!--
  ACTION REQUIRED: Define measurable success criteria.
  These must be technology-agnostic and measurable.
-->

### Measurable Outcomes

- **SC-001**: Users can enter their name and reach the trades list in under 5 seconds.
- **SC-002**: Users can create and see a new trade in the list in under 10 seconds.
- **SC-003**: A pricing response returns within 2 seconds for single-trade requests.
- **SC-004**: 90% of users can complete the create-and-price flow on first attempt without errors.
