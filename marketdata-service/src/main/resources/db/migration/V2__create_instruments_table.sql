-- V2: Create instruments table
-- Instruments represent points on a curve's term structure (instrument type + tenor)

CREATE TABLE instruments (
    instrument_id UUID PRIMARY KEY,
    curve_id UUID NOT NULL,
    instrument_type VARCHAR(20) NOT NULL,
    tenor VARCHAR(10) NOT NULL,
    
    -- Foreign key to curves (NO CASCADE DELETE to allow orphaned quotes)
    CONSTRAINT fk_instrument_curve FOREIGN KEY (curve_id) 
        REFERENCES curves(curve_id),
    
    -- Unique constraint: Each tenor appears only once per curve version
    CONSTRAINT uq_instrument_curve_tenor UNIQUE (curve_id, tenor)
);

-- Index for fast lookup of instruments by curve
CREATE INDEX idx_instruments_curve ON instruments(curve_id);

-- Comments
COMMENT ON TABLE instruments IS 'Points on curve term structure (instrument type + tenor)';
COMMENT ON COLUMN instruments.instrument_type IS 'MoneyMarket, Future, or Swap';
COMMENT ON COLUMN instruments.tenor IS 'ON, 1M, 3M, 6M, 1Y, 2Y, 5Y, 10Y, 30Y';
COMMENT ON CONSTRAINT uq_instrument_curve_tenor ON instruments IS 'No duplicate tenors within a curve version';
