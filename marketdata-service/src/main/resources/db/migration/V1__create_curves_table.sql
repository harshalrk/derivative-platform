-- V1: Create curves table
-- Curves represent market data curves at specific points in time (temporal versioning)
-- Each curve is identified by name + curve_date combination (5 PM Eastern time)

CREATE TABLE curves (
    curve_id UUID PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    curve_date TIMESTAMP NOT NULL,
    currency VARCHAR(3) NOT NULL,
    index_name VARCHAR(50) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    
    -- Unique constraint: Each curve name+date combination is unique
    CONSTRAINT uq_curve_name_date UNIQUE (name, curve_date)
);

-- Indexes for fast lookups
CREATE INDEX idx_curves_name ON curves(name);
CREATE INDEX idx_curves_date ON curves(curve_date);

-- Comments
COMMENT ON TABLE curves IS 'Market data curves with temporal versioning (name+date)';
COMMENT ON COLUMN curves.curve_date IS 'Date at 5 PM Eastern time (stored as UTC)';
COMMENT ON COLUMN curves.currency IS 'ISO currency code (USD, EUR, GBP, JPY)';
COMMENT ON COLUMN curves.index_name IS 'Interest rate index (SOFR, LIBOR, EURIBOR, SONIA)';
