# Data Model: Trader UI + Pricing (No Auth)

## Entities

### Trade
- instrument: string
- side: enum { Buy, Sell }
- quantity: decimal (> 0)
- price: decimal (>= 0)
- currency: string (ISO 4217)
- tradeDate: date
- bookedBy: string (session display name)

Validation:
- quantity > 0
- currency in supported set
- non-empty instrument and bookedBy

### PricingRequest
- tradeId: string
- seed: int (for deterministic random data)

Validation:
- tradeId exists
- seed provided

### PricingResult
- tradeId: string
- price: decimal
- currency: string

Notes:
- Precision: Display to two decimals in UI; backend may compute with higher precision.
- Event Sourcing: Trade booked as events via Marten; queries via document views.
