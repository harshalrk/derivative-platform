package com.derivative.marketdata.model.entity;

import jakarta.persistence.*;
import jakarta.validation.constraints.NotNull;
import java.math.BigDecimal;
import java.time.Instant;
import java.util.UUID;

/**
 * Quote entity representing a market data value for a specific instrument.
 * Values are stored with 2 decimal precision and can be negative.
 */
@Entity
@Table(name = "quotes")
public class Quote {
    
    @Id
    @Column(name = "quote_id", updatable = false, nullable = false)
    private UUID id;
    
    @NotNull(message = "Instrument is required")
    @OneToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "instrument_id", nullable = false)
    private Instrument instrument;
    
    @NotNull(message = "Quote value is required")
    @Column(name = "value", nullable = false, precision = 10, scale = 2)
    private BigDecimal value;
    
    @Column(name = "created_at", nullable = false, updatable = false)
    private Instant createdAt;
    
    @Column(name = "updated_at", nullable = false)
    private Instant updatedAt;
    
    // Constructors
    public Quote() {
        this.id = UUID.randomUUID();
    }
    
    public Quote(BigDecimal value) {
        this();
        this.value = value;
    }
    
    // PrePersist and PreUpdate callbacks
    @PrePersist
    protected void onCreate() {
        Instant now = Instant.now();
        if (createdAt == null) {
            createdAt = now;
        }
        updatedAt = now;
    }
    
    @PreUpdate
    protected void onUpdate() {
        updatedAt = Instant.now();
    }
    
    // Getters and Setters
    public UUID getId() {
        return id;
    }
    
    public void setId(UUID id) {
        this.id = id;
    }
    
    public Instrument getInstrument() {
        return instrument;
    }
    
    public void setInstrument(Instrument instrument) {
        this.instrument = instrument;
    }
    
    public BigDecimal getValue() {
        return value;
    }
    
    public void setValue(BigDecimal value) {
        this.value = value;
    }
    
    public Instant getCreatedAt() {
        return createdAt;
    }
    
    public void setCreatedAt(Instant createdAt) {
        this.createdAt = createdAt;
    }
    
    public Instant getUpdatedAt() {
        return updatedAt;
    }
    
    public void setUpdatedAt(Instant updatedAt) {
        this.updatedAt = updatedAt;
    }
}
