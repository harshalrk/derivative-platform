package com.derivative.marketdata.model.dto;

import java.util.UUID;

/**
 * DTO for instrument output in curve responses.
 */
public class InstrumentOutput {
    
    private UUID id;
    private String type;
    private String tenor;
    
    // Constructors
    public InstrumentOutput() {
    }
    
    public InstrumentOutput(UUID id, String type, String tenor) {
        this.id = id;
        this.type = type;
        this.tenor = tenor;
    }
    
    // Getters and Setters
    public UUID getId() {
        return id;
    }
    
    public void setId(UUID id) {
        this.id = id;
    }
    
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
