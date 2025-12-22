---

description: "Tasks for Trader UI + Pricing (No Auth)"
---

# Tasks: Trader UI + Pricing (No Auth)

**Input**: Design documents from `/specs/001-trader-ui-pricing/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Tests are OPTIONAL per constitution and user direction.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Create solution and projects: `backend/src/Api`, `backend/src/Pricing`, `backend/src/Messaging`, `backend/src/Persistence`, `frontend/src` (Blazor Server)
- [ ] T002 Initialize .NET 8 projects with required dependencies (ASP.NET Core, Blazor, MartenDB, Rebus, RabbitMQ client)
- [ ] T003 [P] Create Dockerfiles for `backend/src/Api` and `frontend/src`
- [ ] T004 Create `infra/docker/compose.yml` with services: api (5000), ui (5002), postgres (5432), rabbitmq (5672, 15672)
- [ ] T005 [P] Configure MartenDB connection to Postgres in `backend/src/Persistence/MartenConfig.cs`
- [ ] T006 [P] Configure Rebus with RabbitMQ in `backend/src/Messaging/RebusConfig.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

- [ ] T007 Setup API routing and base controllers in `backend/src/Api`
- [ ] T008 Define core models in `backend/src/Models/Trade.cs`, `PricingRequest.cs`, `PricingResult.cs`
- [ ] T009 Implement persistence repositories (Marten) in `backend/src/Persistence/TradeRepository.cs`
- [ ] T010 Implement deterministic pricing service using seed in `backend/src/Pricing/PricingService.cs`
- [ ] T011 [P] Create Blazor layout and theme in `frontend/src/Shared/MainLayout.razor` and styles in `frontend/src/wwwroot/css/site.css`
- [ ] T012 [P] Implement API client service in `frontend/src/Services/ApiClient.cs`

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - Enter Name to Access (Priority: P1) 🎯 MVP

**Goal**: Provide entry page to capture display name and set session context.

**Independent Test**: Enter a name and navigate to trades list with name visible.

### Implementation for User Story 1

- [ ] T013 [US1] Add session endpoint `POST /session` in `backend/src/Api/SessionController.cs`
- [ ] T014 [US1] Add Blazor page `frontend/src/Pages/NameEntry.razor` with validation and submit
- [ ] T015 [US1] Persist session name client-side (Blazor) and include in API calls
- [ ] T016 [US1] Display session name in header `frontend/src/Shared/MainLayout.razor`

**Checkpoint**: User can enter name and proceed

---

## Phase 4: User Story 2 - View Booked Trades (Priority: P2)

**Goal**: Show a list of trades booked by current session user.

**Independent Test**: Book sample trades and confirm list filters by `bookedBy`.

### Implementation for User Story 2

- [ ] T017 [US2] Implement `GET /trades` in `backend/src/Api/TradesController.cs` (filter by session name)
- [ ] T018 [P] [US2] Add trades table component `frontend/src/Components/TradesTable.razor`
- [ ] T019 [US2] Add Blazor page `frontend/src/Pages/TradesList.razor` with sorting
- [ ] T020 [US2] Wire API client for fetching trades in `frontend/src/Services/ApiClient.cs`

**Checkpoint**: User sees only own trades

---

## Phase 5: User Story 3 - Create and Price Trade (Priority: P3)

**Goal**: Create a trade and request a price using deterministic random market data.

**Independent Test**: Submit trade, receive price response with seed.

### Implementation for User Story 3

- [ ] T021 [US3] Implement `POST /trades` in `backend/src/Api/TradesController.cs`
- [ ] T022 [US3] Validate inputs server-side in `backend/src/Api/TradesController.cs`
- [ ] T023 [US3] Add Blazor page `frontend/src/Pages/NewTrade.razor` with form validation
- [ ] T024 [US3] Implement `POST /pricing/{tradeId}` in `backend/src/Api/PricingController.cs`
- [ ] T025 [US3] Add pricing UI (modal/component) `frontend/src/Components/PriceModal.razor`
- [ ] T026 [US3] Pass seed to backend; display price and currency in UI

**Checkpoint**: User can create and price a trade

---

## Phase N: Polish & Cross-Cutting Concerns

- [ ] T027 [P] Update `frontend/src/wwwroot/css/site.css` with professional trader theme
- [ ] T028 Add error display component `frontend/src/Components/ErrorBanner.razor`
- [ ] T029 Add graceful handling for backend unavailability in `frontend/src/Services/ApiClient.cs`
- [ ] T030 [P] Add README and update `specs/001-trader-ui-pricing/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies
- Setup (Phase 1): No dependencies
- Foundational (Phase 2): Depends on Setup completion - BLOCKS all user stories
- User Stories (Phase 3+): Depend on Foundational completion; can proceed sequentially P1 → P2 → P3

### User Story Dependencies
- US1: None (entry context)
- US2: Depends on user session context (US1)
- US3: Depends on session context and trade persistence

### Within Each User Story
- Models/services ready before endpoints/UI
- Core implementation before integration

### Parallel Opportunities
- [P] tasks across Setup and Foundational
- UI components [P] can be developed alongside API endpoints

---

## Implementation Strategy

### MVP First (User Story 1 Only)
1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: US1
4. Validate and demo name entry → trades list routing

### Incremental Delivery
1. Add US2 → List trades (filtered)
2. Add US3 → Create and price trade (deterministic)
3. Polish → Styling, error handling
