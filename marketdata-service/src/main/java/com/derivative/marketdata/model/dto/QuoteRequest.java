package com.derivative.marketdata.model.dto;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotEmpty;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import java.time.LocalDate;
import java.util.List;

/**
 * DTO for saving quotes for a curve.
 * Includes curve identification and list of quotes.
 */
public class QuoteRequest {
    
    @NotBlank(message = "Curve name is required")
    @Size(max = 100, message = "Curve name must not exceed 100 characters")
    private String curveName;
    
    @NotNull(message = "Curve date is required")
    private LocalDate curveDate;
    
    @NotEmpty(message = "At least 1 quote is required")
    @Valid
    private List<QuoteInput> quotes;
    
    // Constructors
    public QuoteRequest() {
    }
    
    public QuoteRequest(String curveName, LocalDate curveDate, List<QuoteInput> quotes) {
        this.curveName = curveName;
        this.curveDate = curveDate;
        this.quotes = quotes;
    }
    
    // Getters and Setters
    public String getCurveName() {
        return curveName;
    }
    
    public void setCurveName(String curveName) {
        this.curveName = curveName;
    }
    
    public LocalDate getCurveDate() {
        return curveDate;
    }
    
    public void setCurveDate(LocalDate curveDate) {
        this.curveDate = curveDate;
    }
    
    public List<QuoteInput> getQuotes() {
        return quotes;
    }
    
    public void setQuotes(List<QuoteInput> quotes) {
        this.quotes = quotes;
    }
}
