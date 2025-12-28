package com.derivative.marketdata.model.entity;

import jakarta.persistence.*;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;
import java.time.Instant;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

/**
 * Curve entity representing a market data curve at a specific point in time.
 * Temporal versioning: Each curve is identified by name + curve_date combination.
 */
@Entity
@Table(name = "curves", 
       uniqueConstraints = @UniqueConstraint(
           name = "uq_curve_name_date", 
           columnNames = {"name", "curve_date"}
       ))
public class Curve {
    
    @Id
    @Column(name = "curve_id", updatable = false, nullable = false)
    private UUID id;
    
    @NotBlank(message = "Curve name is required")
    @Size(max = 100, message = "Curve name must not exceed 100 characters")
    @Column(name = "name", nullable = false, length = 100)
    private String name;
    
    @NotNull(message = "Curve date is required")
    @Column(name = "curve_date", nullable = false)
    private Instant curveDate;
    
    @NotBlank(message = "Currency is required")
    @Size(min = 3, max = 3, message = "Currency must be 3 characters (ISO code)")
    @Column(name = "currency", nullable = false, length = 3)
    private String currency;
    
    @NotBlank(message = "Index name is required")
    @Size(max = 50, message = "Index name must not exceed 50 characters")
    @Column(name = "index_name", nullable = false, length = 50)
    private String indexName;
    
    @Column(name = "created_at", nullable = false, updatable = false)
    private Instant createdAt;
    
    @OneToMany(mappedBy = "curve", cascade = CascadeType.ALL, orphanRemoval = true, fetch = FetchType.LAZY)
    private List<Instrument> instruments = new ArrayList<>();
    
    // Constructors
    public Curve() {
        this.id = UUID.randomUUID();
    }
    
    public Curve(String name, Instant curveDate, String currency, String indexName) {
        this();
        this.name = name;
        this.curveDate = curveDate;
        this.currency = currency;
        this.indexName = indexName;
    }
    
    // PrePersist callback
    @PrePersist
    protected void onCreate() {
        if (createdAt == null) {
            createdAt = Instant.now();
        }
    }
    
    // Helper methods for bidirectional relationship
    public void addInstrument(Instrument instrument) {
        instruments.add(instrument);
        instrument.setCurve(this);
    }
    
    public void removeInstrument(Instrument instrument) {
        instruments.remove(instrument);
        instrument.setCurve(null);
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
    
    public String getIndexName() {
        return indexName;
    }
    
    public void setIndexName(String indexName) {
        this.indexName = indexName;
    }
    
    public Instant getCreatedAt() {
        return createdAt;
    }
    
    public void setCreatedAt(Instant createdAt) {
        this.createdAt = createdAt;
    }
    
    public List<Instrument> getInstruments() {
        return instruments;
    }
    
    public void setInstruments(List<Instrument> instruments) {
        this.instruments = instruments;
    }
}
