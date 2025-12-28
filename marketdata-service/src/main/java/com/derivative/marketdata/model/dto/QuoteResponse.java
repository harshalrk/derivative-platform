package com.derivative.marketdata.model.dto;

import java.time.Instant;
import java.util.List;
import java.util.UUID;

/**
 * DTO for quote response after saving quotes.
 */
public class QuoteResponse {
    
    private UUID curveId;
    private String curveName;
    private Instant curveDate;
    private List<QuoteOutput> quotes;
    
    // Constructors
    public QuoteResponse() {
    }
    
    public QuoteResponse(UUID curveId, String curveName, Instant curveDate, List<QuoteOutput> quotes) {
        this.curveId = curveId;
        this.curveName = curveName;
        this.curveDate = curveDate;
        this.quotes = quotes;
    }
    
    // Getters and Setters
    public UUID getCurveId() {
        return curveId;
    }
    
    public void setCurveId(UUID curveId) {
        this.curveId = curveId;
    }
    
    public String getCurveName() {
        return curveName;
    }
    
    public void setCurveName(String curveName) {
        this.curveName = curveName;
    }
    
    public Instant getCurveDate() {
        return curveDate;
    }
    
    public void setCurveDate(Instant curveDate) {
        this.curveDate = curveDate;
    }
    
    public List<QuoteOutput> getQuotes() {
        return quotes;
    }
    
    public void setQuotes(List<QuoteOutput> quotes) {
        this.quotes = quotes;
    }
}
