package com.derivative.marketdata.repository;

import com.derivative.marketdata.model.entity.Instrument;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

/**
 * Repository interface for Instrument entity operations.
 * Provides queries for instruments by curve and tenor.
 */
@Repository
public interface InstrumentRepository extends JpaRepository<Instrument, UUID> {
    
    /**
     * Find all instruments for a specific curve.
     * @param curveId Curve UUID
     * @return List of instruments ordered by tenor
     */
    @Query("SELECT i FROM Instrument i WHERE i.curve.id = :curveId ORDER BY i.tenor")
    List<Instrument> findByCurveId(@Param("curveId") UUID curveId);
    
    /**
     * Find an instrument by curve and tenor.
     * @param curveId Curve UUID
     * @param tenor Tenor (e.g., "1M", "1Y")
     * @return Optional containing the instrument if found
     */
    @Query("SELECT i FROM Instrument i WHERE i.curve.id = :curveId AND i.tenor = :tenor")
    Optional<Instrument> findByCurveIdAndTenor(@Param("curveId") UUID curveId, @Param("tenor") String tenor);
    
    /**
     * Check if an instrument exists with the given curve and tenor.
     * @param curveId Curve UUID
     * @param tenor Tenor
     * @return true if exists, false otherwise
     */
    @Query("SELECT COUNT(i) > 0 FROM Instrument i WHERE i.curve.id = :curveId AND i.tenor = :tenor")
    boolean existsByCurveIdAndTenor(@Param("curveId") UUID curveId, @Param("tenor") String tenor);
    
    /**
     * Count instruments for a specific curve.
     * @param curveId Curve UUID
     * @return Number of instruments
     */
    @Query("SELECT COUNT(i) FROM Instrument i WHERE i.curve.id = :curveId")
    long countByCurveId(@Param("curveId") UUID curveId);
}
