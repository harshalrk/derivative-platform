package com.derivative.marketdata.model.dto;

import java.time.Instant;
import java.util.List;
import java.util.UUID;

/**
 * DTO for curve response (full curve with instruments).
 */
public class CurveResponse {
    
    private UUID id;
    private String name;
    private Instant curveDate;
    private String currency;
    private String index;
    private Instant createdAt;
    private List<InstrumentOutput> instruments;
    
    // Constructors
    public CurveResponse() {
    }
    
    public CurveResponse(UUID id, String name, Instant curveDate, String currency, String index, 
                        Instant createdAt, List<InstrumentOutput> instruments) {
        this.id = id;
        this.name = name;
        this.curveDate = curveDate;
        this.currency = currency;
        this.index = index;
        this.createdAt = createdAt;
        this.instruments = instruments;
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
    
    public Instant getCreatedAt() {
        return createdAt;
    }
    
    public void setCreatedAt(Instant createdAt) {
        this.createdAt = createdAt;
    }
    
    public List<InstrumentOutput> getInstruments() {
        return instruments;
    }
    
    public void setInstruments(List<InstrumentOutput> instruments) {
        this.instruments = instruments;
    }
}
