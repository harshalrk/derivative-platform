package com.derivative.marketdata.model.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;

import java.time.LocalDate;

/**
 * Request DTO for rolling curves and quotes to a new date.
 */
public class RollRequest {
    
    @NotBlank(message = "Curve name is required")
    private String curveName;
    
    @NotNull(message = "Target date is required")
    private LocalDate targetDate;
    
    private boolean overwrite = false;
    
    public RollRequest() {
    }
    
    public RollRequest(String curveName, LocalDate targetDate, boolean overwrite) {
        this.curveName = curveName;
        this.targetDate = targetDate;
        this.overwrite = overwrite;
    }
    
    public String getCurveName() {
        return curveName;
    }
    
    public void setCurveName(String curveName) {
        this.curveName = curveName;
    }
    
    public LocalDate getTargetDate() {
        return targetDate;
    }
    
    public void setTargetDate(LocalDate targetDate) {
        this.targetDate = targetDate;
    }
    
    public boolean isOverwrite() {
        return overwrite;
    }
    
    public void setOverwrite(boolean overwrite) {
        this.overwrite = overwrite;
    }
}
