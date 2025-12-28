package com.derivative.marketdata.model.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import java.math.BigDecimal;

/**
 * DTO for quote input when saving quotes.
 * Represents a quote value for a specific instrument in a curve.
 */
public class QuoteInput {
    
    @NotBlank(message = "Instrument type is required")
    @Size(max = 20, message = "Instrument type must not exceed 20 characters")
    private String instrumentType;
    
    @NotBlank(message = "Tenor is required")
    @Size(max = 10, message = "Tenor must not exceed 10 characters")
    private String tenor;
    
    @NotNull(message = "Quote value is required")
    private BigDecimal value;
    
    // Constructors
    public QuoteInput() {
    }
    
    public QuoteInput(String instrumentType, String tenor, BigDecimal value) {
        this.instrumentType = instrumentType;
        this.tenor = tenor;
        this.value = value;
    }
    
    // Getters and Setters
    public String getInstrumentType() {
        return instrumentType;
    }
    
    public void setInstrumentType(String instrumentType) {
        this.instrumentType = instrumentType;
    }
    
    public String getTenor() {
        return tenor;
    }
    
    public void setTenor(String tenor) {
        this.tenor = tenor;
    }
    
    public BigDecimal getValue() {
        return value;
    }
    
    public void setValue(BigDecimal value) {
        this.value = value;
    }
}
