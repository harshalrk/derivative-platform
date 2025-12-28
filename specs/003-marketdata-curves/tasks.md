# Tasks: Market Data Curve & Quote Management

**Feature**: 003-marketdata-curves  
**Branch**: `003-marketdata-curves`  
**Input**: Design documents from `/specs/003-marketdata-curves/`

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `- [ ] [ID] [P?] [Story?] Description with file path`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- Exact file paths included in descriptions

---

## Phase 1: Setup (Shared Infrastructure) ✅ COMPLETE

**Purpose**: Project initialization and basic structure

- [X] T001 Create Java/Spring Boot project structure in marketdata-service/ with Maven pom.xml
- [X] T002 Create React 18 project structure in marketdata-ui/ with Vite and TypeScript
- [X] T003 [P] Add Docker files: marketdata-service/Dockerfile and marketdata-ui/Dockerfile
- [X] T004 [P] Add Spring Boot dependencies to marketdata-service/pom.xml (web, data-jpa, postgresql, flyway, validation, jquantlib)
- [X] T005 [P] Add React dependencies to marketdata-ui/package.json (axios, react-router-dom, mui or antd)
- [X] T006 Configure application.yml in marketdata-service/src/main/resources/ with PostgreSQL connection
- [X] T007 Update infra/docker/compose.yml to include marketdata-service and marketdata-ui services (already done)
- [X] T008 [P] Setup ESLint and Prettier for marketdata-ui/
- [X] T009 [P] Configure CORS in marketdata-service/src/main/java/config/WebConfig.java
- [X] T010 Create nginx configuration in infra/nginx/nginx.conf with path-based routing (/trade → ui, /marketdata → marketdata-ui, /api/trade → api, /api/marketdata → marketdata-service)
- [X] T011 Add nginx service to infra/docker/compose.yml (nginx:alpine, port 80, mount nginx.conf, depends on ui + marketdata-ui)
- [X] T012 Update quickstart.md to document reverse proxy URLs (http://localhost/marketdata, http://localhost/trade)
- [X] T012a Add navigation link to marketdata module in frontend/src/TraderUI/Components/Layout/NavMenu.razor (external link to http://localhost/marketdata)

---

## Phase 2: Foundational (Blocking Prerequisites) ✅ COMPLETE

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Database Schema

- [X] T013 Create Flyway migration V1__create_curves_table.sql in marketdata-service/src/main/resources/db/migration/
- [X] T014 Create Flyway migration V2__create_instruments_table.sql in marketdata-service/src/main/resources/db/migration/
- [X] T015 Create Flyway migration V3__create_quotes_table.sql in marketdata-service/src/main/resources/db/migration/

### Domain Entities (JPA)

- [X] T016 [P] Create Curve entity in marketdata-service/src/main/java/com/derivative/marketdata/model/entity/Curve.java
- [X] T017 [P] Create Instrument entity in marketdata-service/src/main/java/com/derivative/marketdata/model/entity/Instrument.java
- [X] T018 [P] Create Quote entity in marketdata-service/src/main/java/com/derivative/marketdata/model/entity/Quote.java

### Repositories

- [X] T019 [P] Create CurveRepository interface in marketdata-service/src/main/java/com/derivative/marketdata/repository/CurveRepository.java
- [X] T020 [P] Create InstrumentRepository interface in marketdata-service/src/main/java/com/derivative/marketdata/repository/InstrumentRepository.java
- [X] T021 [P] Create QuoteRepository interface in marketdata-service/src/main/java/com/derivative/marketdata/repository/QuoteRepository.java

### DTOs and Common Services

- [X] T022 [P] Create DateTimeService in marketdata-service/src/main/java/com/derivative/marketdata/service/DateTimeService.java (Eastern Time handling)
- [X] T023 [P] Create GlobalExceptionHandler in marketdata-service/src/main/java/com/derivative/marketdata/exception/GlobalExceptionHandler.java
- [X] T024 [P] Create reference data enums (Currency, Index, InstrumentType, Tenor) in marketdata-service/src/main/java/com/derivative/marketdata/model/enums/

### Frontend Base Setup

- [X] T025 [P] Create API client service in marketdata-ui/src/services/api.ts with Axios configuration
- [X] T026 [P] Setup React Router with routes in marketdata-ui/src/App.tsx (/define, /quotes, /roll)
- [X] T027 [P] Create shared validation utilities in marketdata-ui/src/utils/validation.ts

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Define Market Data Curves (Priority: P1) 🎯 MVP ✅ COMPLETE

**Goal**: Users can create curves with name, currency, index, date, and term structure (instruments). Each curve is a temporal version identified by name+date.

**Independent Test**: Create a curve "USD-SOFR" for date "2025-12-25" with 3 instruments (MoneyMarket/ON, MoneyMarket/1M, Swap/1Y), save it, then retrieve by name+date to verify all attributes and instruments are persisted correctly.

### Backend - Reference Data API

- [X] T028 [P] [US1] Create ReferenceDataController in marketdata-service/src/main/java/com/derivative/marketdata/controller/ReferenceDataController.java with GET endpoints for currencies, indexes, instrument-types, tenors
- [ ] T029 [P] [US1] Write unit tests for ReferenceDataController in marketdata-service/src/test/java/com/derivative/marketdata/controller/ReferenceDataControllerTest.java

### Backend - Curve DTOs

- [X] T030 [P] [US1] Create CurveRequest DTO in marketdata-service/src/main/java/com/derivative/marketdata/model/dto/CurveRequest.java (name, date, currency, index, instruments[])
- [X] T031 [P] [US1] Create InstrumentInput DTO in marketdata-service/src/main/java/com/derivative/marketdata/model/dto/InstrumentInput.java (type, tenor)
- [X] T032 [P] [US1] Create CurveResponse DTO in marketdata-service/src/main/java/com/derivative/marketdata/model/dto/CurveResponse.java
- [X] T033 [P] [US1] Create CurveSummary DTO in marketdata-service/src/main/java/com/derivative/marketdata/model/dto/CurveSummary.java

### Backend - Curve Service

- [X] T034 [US1] Implement CurveService.createCurve() in marketdata-service/src/main/java/com/derivative/marketdata/service/CurveService.java with validation (min 1 instrument, no duplicate tenors, unique name+date)
- [X] T035 [US1] Implement CurveService.getCurveByNameAndDate() in marketdata-service/src/main/java/com/derivative/marketdata/service/CurveService.java
- [X] T036 [US1] Implement CurveService.getCurveById() in marketdata-service/src/main/java/com/derivative/marketdata/service/CurveService.java
- [X] T037 [US1] Implement CurveService.listCurveNames() and listCurvesByName() in marketdata-service/src/main/java/com/derivative/marketdata/service/CurveService.java
- [X] T038 [US1] Add validation: at least 1 instrument (FR-006), no duplicate tenors (FR-005), unique name+date (FR-026)
- [ ] T039 [P] [US1] Write unit tests for CurveService in marketdata-service/src/test/java/com/derivative/marketdata/service/CurveServiceTest.java

### Backend - Curve Controller

- [X] T040 [US1] Implement POST /api/curves endpoint in marketdata-service/src/main/java/com/derivative/marketdata/controller/CurveController.java
- [X] T041 [P] [US1] Implement GET /api/curves/query?name&date endpoint in marketdata-service/src/main/java/com/derivative/marketdata/controller/CurveController.java
- [X] T042 [P] [US1] Implement GET /api/curves?name endpoint (list all dates for curve) in marketdata-service/src/main/java/com/derivative/marketdata/controller/CurveController.java
- [X] T043 [P] [US1] Implement GET /api/curves (list all curve names) in marketdata-service/src/main/java/com/derivative/marketdata/controller/CurveController.java
- [ ] T044 [P] [US1] Write integration tests for CurveController in marketdata-service/src/test/java/com/derivative/marketdata/controller/CurveControllerTest.java

### Frontend - Curve Service

- [X] T045 [P] [US1] Create curveService.ts in marketdata-ui/src/services/curveService.ts with API calls (createCurve, getCurve, listCurves, getReferenceData)

### Frontend - Define Curves Screen

- [X] T046 [US1] Create DefineCurves page component in marketdata-ui/src/pages/DefineCurves.tsx
- [X] T047 [US1] Create CurveForm component in marketdata-ui/src/components/CurveForm.tsx (name, currency, index, date inputs)
- [X] T048 [US1] Create InstrumentBuilder component in marketdata-ui/src/components/InstrumentBuilder.tsx (add/remove instruments, dropdowns for type/tenor)
- [X] T049 [US1] Add validation in CurveForm: min 1 instrument, no duplicate tenors, all fields required
- [X] T050 [US1] Display error messages for validation failures (duplicate tenor, unique name+date conflict)
- [ ] T051 [P] [US1] Write component tests for CurveForm in marketdata-ui/src/components/CurveForm.test.tsx

**Checkpoint**: At this point, User Story 1 should be fully functional - users can create and retrieve curves with instruments

---

## Phase 4: User Story 2 - Manage Daily Quotes (Priority: P1)

**Goal**: Users can enter quotes for all instruments in a curve version. Quotes must be complete (all instruments), support 2 decimal precision, and allow positive/negative values. Existing quotes are prepopulated for editing.

**Independent Test**: Select curve "USD-SOFR" for date "2025-12-25" (from US1), enter quotes (5.25, 5.30, 5.50) for all 3 instruments, save. Reopen same curve to verify quotes are prepopulated. Modify one quote and save again to verify overwrite.

### Backend - Quote DTOs

- [X] T052 [P] [US2] Create QuoteRequest DTO in marketdata-service/src/main/java/com/derivative/marketdata/model/dto/QuoteRequest.java (curveName, curveDate, quotes[])
- [X] T053 [P] [US2] Create QuoteInput DTO in marketdata-service/src/main/java/com/derivative/marketdata/model/dto/QuoteInput.java (instrumentType, tenor, value with 2 decimals)
- [X] T054 [P] [US2] Create QuoteResponse DTO in marketdata-service/src/main/java/com/derivative/marketdata/model/dto/QuoteResponse.java
- [X] T055 [P] [US2] Create QuoteOutput DTO in marketdata-service/src/main/java/com/derivative/marketdata/model/dto/QuoteOutput.java

### Backend - Quote Service

- [X] T056 [US2] Implement QuoteService.saveQuotes() in marketdata-service/src/main/java/com/derivative/marketdata/service/QuoteService.java with validation (all instruments have quotes, 2 decimal precision)
- [X] T057 [US2] Implement QuoteService.getQuotesByCurve() in marketdata-service/src/main/java/com/derivative/marketdata/service/QuoteService.java (retrieve quotes for curve name+date)
- [X] T058 [US2] Add validation: all instruments must have quotes (FR-014, FR-017), 2 decimal places (FR-016), accept positive/negative (FR-015)
- [X] T059 [US2] Implement overwrite logic: update existing quotes or create new ones (FR-020)
- [ ] T060 [P] [US2] Write unit tests for QuoteService in marketdata-service/src/test/java/com/derivative/marketdata/service/QuoteServiceTest.java

### Backend - Quote Controller

- [X] T061 [US2] Implement POST /api/quotes endpoint in marketdata-service/src/main/java/com/derivative/marketdata/controller/QuoteController.java
- [X] T062 [P] [US2] Implement GET /api/quotes?curveName&curveDate endpoint in marketdata-service/src/main/java/com/derivative/marketdata/controller/QuoteController.java
- [ ] T063 [P] [US2] Write integration tests for QuoteController in marketdata-service/src/test/java/com/derivative/marketdata/controller/QuoteControllerTest.java

### Frontend - Quote Service

- [X] T064 [P] [US2] Create quoteService.ts in marketdata-ui/src/services/quoteService.ts with API calls (saveQuotes, getQuotes)

### Frontend - Enter Quotes Screen

- [X] T065 [US2] Create EnterQuotes page component in marketdata-ui/src/pages/EnterQuotes.tsx
- [X] T066 [US2] Create curve selector inputs (name dropdown, date picker) in EnterQuotes component
- [X] T067 [US2] Create QuoteGrid component in marketdata-ui/src/components/QuoteGrid.tsx displaying all instruments from selected curve with input fields
- [X] T068 [US2] Implement quote value validation: 2 decimal places, allow negative values
- [X] T069 [US2] Implement prepopulation: load existing quotes when curve selected and display in input fields
- [X] T070 [US2] Add validation: all instruments must have values before save (display error with missing instrument list)
- [ ] T071 [P] [US2] Write component tests for QuoteGrid in marketdata-ui/src/components/QuoteGrid.test.tsx

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently - users can create curves and enter/edit quotes

---

## Phase 5: User Story 3 - Roll Curves and Quotes to New Dates (Priority: P2)

**Goal**: Users can copy both curve structure and quotes from the most recent previous date to a new target date. System validates previous date has complete quotes.

**Independent Test**: Create curve "EUR-EURIBOR" for "2025-12-24" with 5 instruments and complete quotes. Roll to "2025-12-25" and verify both curve structure and all quote values are copied. Verify error if previous date has incomplete quotes.

### Backend - Roll DTOs

- [X] T072 [P] [US3] Create RollRequest DTO in marketdata-service/src/main/java/com/derivative/marketdata/model/dto/RollRequest.java (curveName, targetDate, overwrite flag)
- [X] T073 [P] [US3] Create RollResponse DTO in marketdata-service/src/main/java/com/derivative/marketdata/model/dto/RollResponse.java (sourceDate, targetDate, instrumentCount, quotes)

### Backend - Roll Service

- [X] T074 [US3] Implement RollService.findPreviousCurveWithQuotes() in marketdata-service/src/main/java/com/derivative/marketdata/service/RollService.java (find most recent date before target with complete quotes)
- [X] T075 [US3] Implement RollService.validateCompleteQuotes() in marketdata-service/src/main/java/com/derivative/marketdata/service/RollService.java (check all instruments have quotes, return missing list if not)
- [X] T076 [US3] Implement RollService.rollCurveAndQuotes() in marketdata-service/src/main/java/com/derivative/marketdata/service/RollService.java (copy curve structure + copy all quotes to new date)
- [X] T077 [US3] Add validation: no previous curve → error, incomplete quotes → error with missing instruments list, target exists → warning
- [ ] T078 [P] [US3] Write unit tests for RollService in marketdata-service/src/test/java/com/derivative/marketdata/service/RollServiceTest.java

### Backend - Roll Controller

- [X] T079 [US3] Implement POST /api/quotes/roll endpoint in marketdata-service/src/main/java/com/derivative/marketdata/controller/QuoteController.java (or separate RollController)
- [ ] T080 [P] [US3] Write integration tests for roll endpoint in marketdata-service/src/test/java/com/derivative/marketdata/controller/RollControllerTest.java

### Frontend - Roll Service

- [X] T081 [P] [US3] Add rollQuotes() method to quoteService.ts in marketdata-ui/src/services/quoteService.ts

### Frontend - Roll Quotes Screen

- [X] T082 [US3] Create RollQuotes page component in marketdata-ui/src/pages/RollQuotes.tsx
- [X] T083 [US3] Add curve name selector and target date picker in RollQuotes component
- [X] T084 [US3] Add "Find Previous" button that calls API and displays previous curve date found
- [X] T085 [US3] Create RollPreview component in marketdata-ui/src/components/RollPreview.tsx showing source date, instrument count, quote values
- [X] T086 [US3] Add "Roll to Target Date" button with confirmation dialog if target already exists
- [X] T087 [US3] Display error messages: no previous found, incomplete quotes with specific instrument list
- [ ] T088 [P] [US3] Write component tests for RollPreview in marketdata-ui/src/components/RollPreview.test.tsx

**Checkpoint**: All 3 user stories should now be independently functional - users can define curves, enter quotes, and roll to new dates

---

## Phase 6: Enhanced Features (Priority: P2) ✅ COMPLETE

**Purpose**: Additional CRUD operations for curves

### Backend - Curve Management

- [X] T089 [P] Implement PUT /api/curves/{id} endpoint in marketdata-service/src/main/java/com/derivative/marketdata/controller/CurveController.java (update curve structure - add/remove instruments)
- [X] T090 [P] Implement DELETE /api/curves?name&date endpoint in marketdata-service/src/main/java/com/derivative/marketdata/controller/CurveController.java (delete curve version without cascading to quotes)
- [X] T091 [P] Implement GET /api/curves/{id} endpoint in marketdata-service/src/main/java/com/derivative/marketdata/controller/CurveController.java

### Frontend - Curve Management

- [X] T092 [P] Add "Edit Curve" functionality in DefineCurves page to modify existing curve structures
- [X] T093 [P] Add "Delete Curve" button with confirmation dialog in DefineCurves page

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

### Documentation

- [ ] T094 [P] Add Javadoc comments to all service classes in marketdata-service/src/main/java/com/derivative/marketdata/service/ (SKIPPED)
- [ ] T095 [P] Add JSDoc comments to all service files in marketdata-ui/src/services/ (SKIPPED)
- [X] T096 [P] Update README.md in project root with marketdata module setup instructions

### Testing & Validation ✅ COMPLETE

- [X] T097 Validate all acceptance scenarios from spec.md for US1 (7 scenarios)
- [X] T098 Validate all acceptance scenarios from spec.md for US2 (6 scenarios)
- [X] T099 Validate all acceptance scenarios from spec.md for US3 (6 scenarios)
- [X] T100 [P] Run quickstart.md validation: docker compose up, test sample data, verify APIs

### Performance & Optimization ✅ COMPLETE

- [X] T101 [P] Add database indexes per data-model.md (curves name/date, instruments curve, quotes instrument)
- [X] T102 [P] Measure API response times, verify <500ms p95 goal
- [X] T103 [P] Measure UI render time for quote grid with 50 instruments, verify <1s goal

### Error Handling & UX ✅ COMPLETE

- [X] T104 [P] Review all error messages for clarity and actionability (per constitution P4)
- [X] T105 [P] Add loading spinners and error toasts in all React components
- [X] T106 [P] Add success notifications for create/update/delete operations

### Security & Configuration

- [ ] T107 [P] Review CORS configuration for production deployment
- [ ] T108 [P] Add Spring Boot Actuator health endpoint configuration
- [ ] T109 [P] Configure production environment variables in application-prod.yml

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup (Phase 1) - BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational (Phase 2) - MVP baseline
- **User Story 2 (Phase 4)**: Depends on Foundational (Phase 2) - Can start in parallel with US1 but needs curves to test
- **User Story 3 (Phase 5)**: Depends on Foundational (Phase 2) and US1+US2 for testing - Needs curves with quotes
- **Enhanced Features (Phase 6)**: Depends on US1 completion
- **Polish (Phase 7)**: Depends on all desired user stories being complete

### User Story Dependencies

- **US1 (Define Curves)**: MUST complete first - provides curves for US2 and US3 testing
- **US2 (Enter Quotes)**: Can develop in parallel with US1, but testing requires US1 curves
- **US3 (Roll Quotes)**: Requires US1 (curves) + US2 (quotes) for meaningful testing

### Recommended Execution Order

**Sprint 1 - MVP (US1 + US2)**:
1. Phase 1: Setup (T001-T009) - 1 day
2. Phase 2: Foundational (T010-T024) - 2-3 days
3. Phase 3: US1 Backend (T025-T041) - 3-4 days
4. Phase 3: US1 Frontend (T042-T048) - 2-3 days
5. Phase 4: US2 Backend (T049-T060) - 2-3 days
6. Phase 4: US2 Frontend (T061-T068) - 2-3 days

**Total Sprint 1**: ~2-3 weeks for MVP (Define Curves + Enter Quotes)

**Sprint 2 - Roll Feature (US3)**:
7. Phase 5: US3 Backend (T069-T077) - 2-3 days
8. Phase 5: US3 Frontend (T078-T085) - 2-3 days

**Total Sprint 2**: ~1 week for Roll functionality

**Sprint 3 - Enhancement & Polish**:
9. Phase 6: Enhanced Features (T086-T090) - 2-3 days
10. Phase 7: Polish (T091-T106) - 3-4 days

**Total Sprint 3**: ~1 week for polish and production readiness

### Parallel Opportunities Within Each Phase

**Phase 1 (Setup)**:
- T003-T005, T008-T009 can all run in parallel (different files)

**Phase 2 (Foundational)**:
- T013-T015 (entities) can run in parallel
- T016-T018 (repositories) can run in parallel after entities
- T019-T021, T022-T024 can all run in parallel

**Phase 3 (US1)**:
- T025-T026 (reference data) can run in parallel
- T027-T030 (DTOs) can run in parallel
- T036 (tests), T038-T041 (endpoints) can run in parallel after service implementation
- T042 (frontend service), T048 (frontend tests) can run in parallel with backend

**Phase 4 (US2)**:
- T049-T052 (DTOs) can run in parallel
- T057, T060 (tests) can run in parallel after service implementation
- T061 (frontend service), T068 (frontend tests) can run in parallel with backend

**Phase 5 (US3)**:
- T069-T070 (DTOs) can run in parallel
- T075, T077 (tests) can run in parallel after service implementation
- T078 (frontend service), T085 (frontend tests) can run in parallel with backend

**Phase 6-7 (Enhancement & Polish)**:
- Most tasks in these phases can run in parallel (different concerns)

---

## Parallel Example: Implementing User Story 1

If you have 3 developers:

**Developer A (Backend)**:
- T025-T026: Reference data API (0.5 days)
- T027-T030: DTOs (0.5 days)
- T031-T035: CurveService implementation (2 days)
- T036: Unit tests (1 day)

**Developer B (Backend)**:
- T037: POST /api/curves (1 day)
- T038-T040: GET endpoints (1 day)
- T041: Integration tests (1 day)

**Developer C (Frontend)**:
- T042: Curve service (0.5 days)
- T043-T045: React components (2 days)
- T046-T047: Validation and errors (1 day)
- T048: Component tests (0.5 days)

**Result**: US1 completes in ~4 days with parallel execution vs ~8 days sequential

---

## Task Summary

**Total Tasks**: 106
- Phase 1 (Setup): 9 tasks
- Phase 2 (Foundational): 15 tasks
- Phase 3 (US1 - Define Curves): 24 tasks
- Phase 4 (US2 - Enter Quotes): 20 tasks
- Phase 5 (US3 - Roll Quotes): 17 tasks
- Phase 6 (Enhanced Features): 5 tasks
- Phase 7 (Polish): 16 tasks

**Parallelizable Tasks**: 48 tasks marked [P] (45% can run in parallel)

**MVP Scope** (Recommended for first delivery):
- Phase 1-2: Setup + Foundational (24 tasks)
- Phase 3: User Story 1 (24 tasks)
- Phase 4: User Story 2 (20 tasks)
- **Total MVP**: 68 tasks (~2-3 weeks)

**Story Breakdown**:
- [US1]: 24 tasks (Define Curves - foundational, testable independently)
- [US2]: 20 tasks (Enter Quotes - depends on curves for testing)
- [US3]: 17 tasks (Roll Quotes - convenience feature, can be delivered later)

**Independent Test Criteria**:
- After US1: Can create and retrieve curves with instruments
- After US2: Can enter and retrieve quotes for curves
- After US3: Can roll curves and quotes to new dates

---

## Implementation Strategy

**MVP-First Approach**:
1. Deliver US1 + US2 first (define curves + enter quotes) - core value
2. US3 (rolling) is convenience - can be added later
3. Polish incrementally during user feedback

**Incremental Delivery**:
- Week 1: Setup + Foundational + US1 backend → Can test via curl/Postman
- Week 2: US1 frontend + US2 backend → Can define curves in UI
- Week 3: US2 frontend → MVP complete, can enter quotes
- Week 4: US3 + polish → Full feature set

**Quality Gates**:
- After each user story: Run independent tests per "Independent Test" criteria
- Before moving to next story: All acceptance scenarios must pass
- Before production: All Phase 7 polish tasks complete
