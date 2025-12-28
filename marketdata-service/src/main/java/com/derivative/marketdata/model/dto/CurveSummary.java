package com.derivative.marketdata.model.dto;

import java.time.Instant;
import java.util.UUID;

/**
 * DTO for curve summary (listing curves without full instrument details).
 */
public class CurveSummary {
    
    private UUID id;
    private String name;
    private Instant curveDate;
    private String currency;
    private String index;
    private int instrumentCount;
    private Instant createdAt;
    
    // Constructors
    public CurveSummary() {
    }
    
    public CurveSummary(UUID id, String name, Instant curveDate, String currency, String index, 
                       int instrumentCount, Instant createdAt) {
        this.id = id;
        this.name = name;
        this.curveDate = curveDate;
        this.currency = currency;
        this.index = index;
        this.instrumentCount = instrumentCount;
        this.createdAt = createdAt;
    }
    
    // Getters and Setters
    public UUID getId() {
        return id;
    }
    
    public void setId(UUID id) {
        this.id = id;
    }
    
    public String getName() {
        return name;
    }
    
    public void setName(String name) {
        this.name = name;
    }
    
    public Instant getCurveDate() {
        return curveDate;
    }
    
    public void setCurveDate(Instant curveDate) {
        this.curveDate = curveDate;
    }
    
    public String getCurrency() {
        return currency;
    }
    
    public void setCurrency(String currency) {
        this.currency = currency;
    }
    
    public String getIndex() {
        return index;
    }
    
    public void setIndex(String index) {
        this.index = index;
    }
    
    public int getInstrumentCount() {
        return instrumentCount;
    }
    
    public void setInstrumentCount(int instrumentCount) {
        this.instrumentCount = instrumentCount;
    }
    
    public Instant getCreatedAt() {
        return createdAt;
    }
    
    public void setCreatedAt(Instant createdAt) {
        this.createdAt = createdAt;
    }
}
