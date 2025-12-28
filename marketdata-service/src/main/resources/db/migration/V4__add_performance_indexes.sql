-- V4: Add performance indexes for common query patterns
-- Per data-model.md requirements for fast lookups

-- Composite index for common query pattern (curve name + date lookup together)
-- This optimizes the frequent getCurveByNameAndDate queries
CREATE INDEX IF NOT EXISTS idx_curves_name_date ON curves(name, curve_date);

-- Note: Individual indexes on name and date already exist from V1:
--   - idx_curves_name ON curves(name)
--   - idx_curves_date ON curves(curve_date)
-- Note: Indexes on instrument_id and curve_id already exist from V2 and V3:
--   - idx_instruments_curve ON instruments(curve_id)
--   - idx_quotes_instrument ON quotes(instrument_id)

