package com.derivative.marketdata.repository;

import com.derivative.marketdata.model.entity.Curve;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.Instant;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

/**
 * Repository interface for Curve entity operations.
 * Provides custom queries for curve lookup by name, date, and combinations.
 */
@Repository
public interface CurveRepository extends JpaRepository<Curve, UUID> {
    
    /**
     * Find a curve by name and exact curve date.
     * @param name Curve name (e.g., "USD-SOFR")
     * @param curveDate Exact curve date (5 PM Eastern as UTC Instant)
     * @return Optional containing the curve if found
     */
    Optional<Curve> findByNameAndCurveDate(String name, Instant curveDate);
    
    /**
     * Find all curves with a specific name (all temporal versions).
     * @param name Curve name
     * @return List of curves ordered by curve date descending (newest first)
     */
    @Query("SELECT c FROM Curve c WHERE c.name = :name ORDER BY c.curveDate DESC")
    List<Curve> findByNameOrderByCurveDateDesc(@Param("name") String name);
    
    /**
     * Find all unique curve names.
     * @return List of distinct curve names
     */
    @Query("SELECT DISTINCT c.name FROM Curve c ORDER BY c.name")
    List<String> findAllDistinctNames();
    
    /**
     * Find all distinct dates for a curve name, ordered descending (newest first).
     * @param name Curve name
     * @return List of distinct curve dates
     */
    @Query("SELECT DISTINCT c.curveDate FROM Curve c WHERE c.name = :name ORDER BY c.curveDate DESC")
    List<Instant> findDistinctCurveDatesByName(@Param("name") String name);
    
    /**
     * Find curves by name pattern (for search functionality).
     * @param namePattern Name pattern (use % for wildcard)
     * @return List of matching curves
     */
    @Query("SELECT c FROM Curve c WHERE c.name LIKE :namePattern ORDER BY c.name, c.curveDate DESC")
    List<Curve> findByNameContaining(@Param("namePattern") String namePattern);
    
    /**
     * Check if a curve exists with the given name and date.
     * @param name Curve name
     * @param curveDate Curve date
     * @return true if exists, false otherwise
     */
    boolean existsByNameAndCurveDate(String name, Instant curveDate);
}
