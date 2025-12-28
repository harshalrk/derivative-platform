package com.derivative.marketdata.model.enums;

/**
 * Supported interest rate indexes.
 * Hardcoded for initial implementation, designed for easy swap to external service later.
 */
public enum Index {
    SOFR("Secured Overnight Financing Rate"),
    LIBOR("London Interbank Offered Rate"),
    EURIBOR("Euro Interbank Offered Rate"),
    SONIA("Sterling Overnight Index Average");
    
    private final String description;
    
    Index(String description) {
        this.description = description;
    }
    
    public String getDescription() {
        return description;
    }
    
    public String getName() {
        return this.name();
    }
}
