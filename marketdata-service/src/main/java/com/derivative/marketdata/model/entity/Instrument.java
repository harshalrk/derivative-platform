package com.derivative.marketdata.model.entity;

import jakarta.persistence.*;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;
import java.util.UUID;

/**
 * Instrument entity representing a point on a curve's term structure.
 * Each instrument has a type (MoneyMarket, Future, Swap) and tenor (ON, 1M, etc.).
 */
@Entity
@Table(name = "instruments",
       uniqueConstraints = @UniqueConstraint(
           name = "uq_instrument_curve_tenor",
           columnNames = {"curve_id", "tenor"}
       ))
public class Instrument {
    
    @Id
    @Column(name = "instrument_id", updatable = false, nullable = false)
    private UUID id;
    
    @NotNull(message = "Curve is required")
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "curve_id", nullable = false)
    private Curve curve;
    
    @NotBlank(message = "Instrument type is required")
    @Size(max = 20, message = "Instrument type must not exceed 20 characters")
    @Column(name = "instrument_type", nullable = false, length = 20)
    private String instrumentType;
    
    @NotBlank(message = "Tenor is required")
    @Size(max = 10, message = "Tenor must not exceed 10 characters")
    @Column(name = "tenor", nullable = false, length = 10)
    private String tenor;
    
    @OneToOne(mappedBy = "instrument", cascade = CascadeType.ALL, orphanRemoval = true, fetch = FetchType.LAZY)
    private Quote quote;
    
    // Constructors
    public Instrument() {
        this.id = UUID.randomUUID();
    }
    
    public Instrument(String instrumentType, String tenor) {
        this();
        this.instrumentType = instrumentType;
        this.tenor = tenor;
    }
    
    // Helper method for bidirectional relationship with Quote
    public void setQuote(Quote quote) {
        this.quote = quote;
        if (quote != null) {
            quote.setInstrument(this);
        }
    }
    
    // Getters and Setters
    public UUID getId() {
        return id;
    }
    
    public void setId(UUID id) {
        this.id = id;
    }
    
    public Curve getCurve() {
        return curve;
    }
    
    public void setCurve(Curve curve) {
        this.curve = curve;
    }
    
    public String getInstrumentType() {
        return instrumentType;
    }
    
    public void setInstrumentType(String instrumentType) {
        this.instrumentType = instrumentType;
    }
    
    public String getTenor() {
        return tenor;
    }
    
    public void setTenor(String tenor) {
        this.tenor = tenor;
    }
    
    public Quote getQuote() {
        return quote;
    }
}
