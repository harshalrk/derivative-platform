package com.derivative.marketdata.model.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

/**
 * DTO for instrument input when creating a curve.
 * Represents a point on the curve's term structure.
 */
public class InstrumentInput {
    
    @NotBlank(message = "Instrument type is required")
    @Size(max = 20, message = "Instrument type must not exceed 20 characters")
    private String type;
    
    @NotBlank(message = "Tenor is required")
    @Size(max = 10, message = "Tenor must not exceed 10 characters")
    private String tenor;
    
    // Constructors
    public InstrumentInput() {
    }
    
    public InstrumentInput(String type, String tenor) {
        this.type = type;
        this.tenor = tenor;
    }
    
    // Getters and Setters
    public String getType() {
        return type;
    }
    
    public void setType(String type) {
        this.type = type;
    }
    
    public String getTenor() {
        return tenor;
    }
    
    public void setTenor(String tenor) {
        this.tenor = tenor;
    }
}
