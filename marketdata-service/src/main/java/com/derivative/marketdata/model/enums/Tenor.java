package com.derivative.marketdata.model.enums;

/**
 * Supported tenor values for instruments.
 * Hardcoded for initial implementation, designed for easy swap to external service later.
 */
public enum Tenor {
    ON("Overnight"),
    ONE_MONTH("1M", "1 Month"),
    THREE_MONTHS("3M", "3 Months"),
    SIX_MONTHS("6M", "6 Months"),
    ONE_YEAR("1Y", "1 Year"),
    TWO_YEARS("2Y", "2 Years"),
    FIVE_YEARS("5Y", "5 Years"),
    TEN_YEARS("10Y", "10 Years"),
    THIRTY_YEARS("30Y", "30 Years");
    
    private final String code;
    private final String description;
    
    Tenor(String description) {
        this.code = this.name();
        this.description = description;
    }
    
    Tenor(String code, String description) {
        this.code = code;
        this.description = description;
    }
    
    public String getCode() {
        return code;
    }
    
    public String getDescription() {
        return description;
    }
    
    /**
     * Get Tenor enum from code string (e.g., "1M", "1Y").
     * @param code Tenor code
     * @return Tenor enum
     * @throws IllegalArgumentException if code is not valid
     */
    public static Tenor fromCode(String code) {
        for (Tenor tenor : values()) {
            if (tenor.code.equalsIgnoreCase(code)) {
                return tenor;
            }
        }
        throw new IllegalArgumentException("Invalid tenor code: " + code);
    }
}
