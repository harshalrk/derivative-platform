package com.derivative.marketdata.model.dto;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotEmpty;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import java.time.LocalDate;
import java.util.List;

/**
 * DTO for creating a new curve.
 * Includes curve metadata and list of instruments.
 */
public class CurveRequest {
    
    @NotBlank(message = "Curve name is required")
    @Size(max = 100, message = "Curve name must not exceed 100 characters")
    private String name;
    
    @NotNull(message = "Curve date is required")
    private LocalDate date;
    
    @NotBlank(message = "Currency is required")
    @Size(min = 3, max = 3, message = "Currency must be 3 characters (ISO code)")
    private String currency;
    
    @NotBlank(message = "Index is required")
    @Size(max = 50, message = "Index must not exceed 50 characters")
    private String index;
    
    @NotEmpty(message = "At least 1 instrument is required")
    @Valid
    private List<InstrumentInput> instruments;
    
    // Constructors
    public CurveRequest() {
    }
    
    public CurveRequest(String name, LocalDate date, String currency, String index, List<InstrumentInput> instruments) {
        this.name = name;
        this.date = date;
        this.currency = currency;
        this.index = index;
        this.instruments = instruments;
    }
    
    // Getters and Setters
    public String getName() {
        return name;
    }
    
    public void setName(String name) {
        this.name = name;
    }
    
    public LocalDate getDate() {
        return date;
    }
    
    public void setDate(LocalDate date) {
        this.date = date;
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
    
    public List<InstrumentInput> getInstruments() {
        return instruments;
    }
    
    public void setInstruments(List<InstrumentInput> instruments) {
        this.instruments = instruments;
    }
}
