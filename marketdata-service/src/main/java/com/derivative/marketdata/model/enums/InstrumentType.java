package com.derivative.marketdata.model.enums;

/**
 * Supported instrument types for curve term structure.
 * Hardcoded for initial implementation, designed for easy swap to external service later.
 */
public enum InstrumentType {
    MONEY_MARKET("Money Market"),
    FUTURE("Future"),
    SWAP("Swap");
    
    private final String description;
    
    InstrumentType(String description) {
        this.description = description;
    }
    
    public String getDescription() {
        return description;
    }
    
    public String getTypeName() {
        return this.name();
    }
}
