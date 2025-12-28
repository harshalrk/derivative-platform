package com.derivative.marketdata.model.enums;

/**
 * Supported currency codes (ISO 4217).
 * Hardcoded for initial implementation, designed for easy swap to external service later.
 */
public enum Currency {
    USD("US Dollar"),
    EUR("Euro"),
    GBP("British Pound"),
    JPY("Japanese Yen");
    
    private final String description;
    
    Currency(String description) {
        this.description = description;
    }
    
    public String getDescription() {
        return description;
    }
    
    public String getCode() {
        return this.name();
    }
}
