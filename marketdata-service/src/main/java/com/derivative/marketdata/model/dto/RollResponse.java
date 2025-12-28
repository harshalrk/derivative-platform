package com.derivative.marketdata.model.dto;

import java.time.Instant;
import java.time.LocalDate;
import java.util.List;
import java.util.UUID;

/**
 * Response DTO for roll operation showing source and target details.
 */
public class RollResponse {
    
    private UUID sourceCurveId;
    private LocalDate sourceDate;
    private UUID targetCurveId;
    private LocalDate targetDate;
    private int instrumentCount;
    private List<QuoteOutput> quotes;
    private String message;
    
    public RollResponse() {
    }
    
    public RollResponse(UUID sourceCurveId, LocalDate sourceDate, UUID targetCurveId, 
                       LocalDate targetDate, int instrumentCount, List<QuoteOutput> quotes, String message) {
        this.sourceCurveId = sourceCurveId;
        this.sourceDate = sourceDate;
        this.targetCurveId = targetCurveId;
        this.targetDate = targetDate;
        this.instrumentCount = instrumentCount;
        this.quotes = quotes;
        this.message = message;
    }
    
    public UUID getSourceCurveId() {
        return sourceCurveId;
    }
    
    public void setSourceCurveId(UUID sourceCurveId) {
        this.sourceCurveId = sourceCurveId;
    }
    
    public LocalDate getSourceDate() {
        return sourceDate;
    }
    
    public void setSourceDate(LocalDate sourceDate) {
        this.sourceDate = sourceDate;
    }
    
    public UUID getTargetCurveId() {
        return targetCurveId;
    }
    
    public void setTargetCurveId(UUID targetCurveId) {
        this.targetCurveId = targetCurveId;
    }
    
    public LocalDate getTargetDate() {
        return targetDate;
    }
    
    public void setTargetDate(LocalDate targetDate) {
        this.targetDate = targetDate;
    }
    
    public int getInstrumentCount() {
        return instrumentCount;
    }
    
    public void setInstrumentCount(int instrumentCount) {
        this.instrumentCount = instrumentCount;
    }
    
    public List<QuoteOutput> getQuotes() {
        return quotes;
    }
    
    public void setQuotes(List<QuoteOutput> quotes) {
        this.quotes = quotes;
    }
    
    public String getMessage() {
        return message;
    }
    
    public void setMessage(String message) {
        this.message = message;
    }
}
