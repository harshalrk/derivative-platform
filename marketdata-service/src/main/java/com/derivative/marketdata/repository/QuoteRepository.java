package com.derivative.marketdata.repository;

import com.derivative.marketdata.model.entity.Quote;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

/**
 * Repository interface for Quote entity operations.
 * Provides queries for quotes by instrument and bulk operations.
 */
@Repository
public interface QuoteRepository extends JpaRepository<Quote, UUID> {
    
    /**
     * Find a quote for a specific instrument.
     * @param instrumentId Instrument UUID
     * @return Optional containing the quote if found
     */
    @Query("SELECT q FROM Quote q WHERE q.instrument.id = :instrumentId")
    Optional<Quote> findByInstrumentId(@Param("instrumentId") UUID instrumentId);
    
    /**
     * Find all quotes for instruments in a specific curve.
     * @param curveId Curve UUID
     * @return List of quotes
     */
    @Query("SELECT q FROM Quote q WHERE q.instrument.curve.id = :curveId")
    List<Quote> findByCurveId(@Param("curveId") UUID curveId);
    
    /**
     * Check if a quote exists for a specific instrument.
     * @param instrumentId Instrument UUID
     * @return true if exists, false otherwise
     */
    @Query("SELECT COUNT(q) > 0 FROM Quote q WHERE q.instrument.id = :instrumentId")
    boolean existsByInstrumentId(@Param("instrumentId") UUID instrumentId);
    
    /**
     * Delete all quotes for a specific curve.
     * @param curveId Curve UUID
     * @return Number of quotes deleted
     */
    @Modifying
    @Query("DELETE FROM Quote q WHERE q.instrument.curve.id = :curveId")
    int deleteByCurveId(@Param("curveId") UUID curveId);
    
    /**
     * Count quotes for a specific curve.
     * @param curveId Curve UUID
     * @return Number of quotes
     */
    @Query("SELECT COUNT(q) FROM Quote q WHERE q.instrument.curve.id = :curveId")
    long countByCurveId(@Param("curveId") UUID curveId);
}
