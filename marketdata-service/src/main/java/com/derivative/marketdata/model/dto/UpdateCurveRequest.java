package com.derivative.marketdata.model.dto;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import java.util.List;

/**
 * Request DTO for updating an existing curve's instruments.
 * Only allows modification of the instrument list (add/remove instruments).
 */
public class UpdateCurveRequest {
    
    @NotNull(message = "Instruments list is required")
    @Size(min = 1, message = "At least 1 instrument is required")
    @Valid
    private List<InstrumentInput> instruments;
    
    public UpdateCurveRequest() {
    }
    
    public UpdateCurveRequest(List<InstrumentInput> instruments) {
        this.instruments = instruments;
    }
    
    public List<InstrumentInput> getInstruments() {
        return instruments;
    }
    
    public void setInstruments(List<InstrumentInput> instruments) {
        this.instruments = instruments;
    }
}
