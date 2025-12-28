-- V3: Create quotes table
-- Quotes represent market data values for specific instruments in specific curve versions

CREATE TABLE quotes (
    quote_id UUID PRIMARY KEY,
    instrument_id UUID NOT NULL,
    value DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    
    -- Foreign key to instruments
    CONSTRAINT fk_quote_instrument FOREIGN KEY (instrument_id) 
        REFERENCES instruments(instrument_id)
);

-- Index for fast lookup of quotes by instrument
CREATE INDEX idx_quotes_instrument ON quotes(instrument_id);

-- Comments
COMMENT ON TABLE quotes IS 'Market data values for instruments (2 decimal precision)';
COMMENT ON COLUMN quotes.value IS 'Quote value with 2 decimal places, can be negative';
COMMENT ON COLUMN quotes.created_at IS 'First save timestamp';
COMMENT ON COLUMN quotes.updated_at IS 'Last update timestamp';
