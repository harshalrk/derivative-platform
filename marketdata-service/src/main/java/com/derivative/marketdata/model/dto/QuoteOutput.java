package com.derivative.marketdata.model.dto;

import java.math.BigDecimal;
import java.time.Instant;
import java.util.UUID;

/**
 * DTO for quote output in responses.
 */
public class QuoteOutput {
    
    private UUID quoteId;
    private UUID instrumentId;
    private String instrumentType;
    private String tenor;
    private BigDecimal value;
    private Instant createdAt;
    private Instant updatedAt;
    
    // Constructors
    public QuoteOutput() {
    }
    
    public QuoteOutput(UUID quoteId, UUID instrumentId, String instrumentType, String tenor, 
                      BigDecimal value, Instant createdAt, Instant updatedAt) {
        this.quoteId = quoteId;
        this.instrumentId = instrumentId;
        this.instrumentType = instrumentType;
        this.tenor = tenor;
        this.value = value;
        this.createdAt = createdAt;
        this.updatedAt = updatedAt;
    }
    
    // Getters and Setters
    public UUID getQuoteId() {
        return quoteId;
    }
    
    public void setQuoteId(UUID quoteId) {
        this.quoteId = quoteId;
    }
    
    public UUID getInstrumentId() {
        return instrumentId;
    }
    
    public void setInstrumentId(UUID instrumentId) {
        this.instrumentId = instrumentId;
    }
    
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
    
    public Instant getCreatedAt() {
        return createdAt;
    }
    
    public void setCreatedAt(Instant createdAt) {
        this.createdAt = createdAt;
    }
    
    public Instant getUpdatedAt() {
        return updatedAt;
    }
    
    public void setUpdatedAt(Instant updatedAt) {
        this.updatedAt = updatedAt;
    }
}
