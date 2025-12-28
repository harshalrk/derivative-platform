package com.derivative.marketdata.service;

import com.derivative.marketdata.model.dto.*;
import com.derivative.marketdata.model.entity.Curve;
import com.derivative.marketdata.model.entity.Instrument;
import com.derivative.marketdata.repository.CurveRepository;
import com.derivative.marketdata.repository.InstrumentRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import jakarta.persistence.EntityManager;

import java.time.Instant;
import java.time.LocalDate;
import java.util.*;
import java.util.stream.Collectors;

/**
 * Service for curve operations including creation, retrieval, and validation.
 */
@Service
public class CurveService {
    
    private final CurveRepository curveRepository;
    private final InstrumentRepository instrumentRepository;
    private final DateTimeService dateTimeService;
    private final EntityManager entityManager;
    
    public CurveService(CurveRepository curveRepository, 
                       InstrumentRepository instrumentRepository,
                       DateTimeService dateTimeService,
                       EntityManager entityManager) {
        this.curveRepository = curveRepository;
        this.instrumentRepository = instrumentRepository;
        this.dateTimeService = dateTimeService;
        this.entityManager = entityManager;
    }
    
    /**
     * Create a new curve with instruments.
     * Validates: at least 1 instrument, no duplicate tenors, unique name+date.
     */
    @Transactional
    public CurveResponse createCurve(CurveRequest request) {
        // Validation: At least 1 instrument (FR-006)
        if (request.getInstruments() == null || request.getInstruments().isEmpty()) {
            throw new IllegalArgumentException("At least 1 instrument is required");
        }
        
        // Validation: No duplicate tenors (FR-005)
        Set<String> tenors = new HashSet<>();
        for (InstrumentInput inst : request.getInstruments()) {
            if (!tenors.add(inst.getTenor())) {
                throw new IllegalArgumentException("Duplicate tenor found: " + inst.getTenor());
            }
        }
        
        // Convert date to 5 PM Eastern Time
        Instant curveDate = dateTimeService.toEasternTimeInstant(request.getDate());
        
        // Validation: Unique name+date (FR-026)
        if (curveRepository.existsByNameAndCurveDate(request.getName(), curveDate)) {
            throw new IllegalArgumentException(
                "Curve with name '" + request.getName() + 
                "' and date '" + request.getDate() + "' already exists"
            );
        }
        
        // Create curve entity
        Curve curve = new Curve(
            request.getName(),
            curveDate,
            request.getCurrency(),
            request.getIndex()
        );
        
        // Create instrument entities
        for (InstrumentInput instInput : request.getInstruments()) {
            Instrument instrument = new Instrument(
                instInput.getType(),
                instInput.getTenor()
            );
            curve.addInstrument(instrument);
        }
        
        // Save curve (cascades to instruments)
        Curve savedCurve = curveRepository.save(curve);
        
        return toResponse(savedCurve);
    }
    
    /**
     * Get a curve by name and date.
     */
    @Transactional(readOnly = true)
    public Optional<CurveResponse> getCurveByNameAndDate(String name, LocalDate date) {
        Instant curveDate = dateTimeService.toEasternTimeInstant(date);
        return curveRepository.findByNameAndCurveDate(name, curveDate)
            .map(this::toResponse);
    }
    
    /**
     * Get a curve by ID.
     */
    @Transactional(readOnly = true)
    public Optional<CurveResponse> getCurveById(UUID id) {
        return curveRepository.findById(id)
            .map(this::toResponse);
    }
    
    /**
     * List all unique curve names.
     */
    @Transactional(readOnly = true)
    public List<String> listCurveNames() {
        return curveRepository.findAllDistinctNames();
    }
    
    /**
     * Get all distinct dates for a curve name in descending order.
     */
    @Transactional(readOnly = true)
    public List<LocalDate> getCurveDates(String name) {
        return curveRepository.findDistinctCurveDatesByName(name).stream()
            .map(dateTimeService::toEasternDate)
            .collect(Collectors.toList());
    }
    
    /**
     * List all curves for a given name (all temporal versions).
     */
    @Transactional(readOnly = true)
    public List<CurveSummary> listCurvesByName(String name) {
        return curveRepository.findByNameOrderByCurveDateDesc(name).stream()
            .map(this::toSummary)
            .collect(Collectors.toList());
    }
    
    /**
     * Convert Curve entity to CurveResponse DTO.
     */
    private CurveResponse toResponse(Curve curve) {
        List<InstrumentOutput> instruments = curve.getInstruments().stream()
            .map(i -> new InstrumentOutput(
                i.getId(),
                i.getInstrumentType(),
                i.getTenor()
            ))
            .collect(Collectors.toList());
        
        return new CurveResponse(
            curve.getId(),
            curve.getName(),
            curve.getCurveDate(),
            curve.getCurrency(),
            curve.getIndexName(),
            curve.getCreatedAt(),
            instruments
        );
    }
    
    /**
     * Update an existing curve's instruments.
     * Validates: at least 1 instrument, no duplicate tenors.
     * Removes old instruments and creates new ones.
     */
    @Transactional
    public CurveResponse updateCurve(UUID curveId, UpdateCurveRequest request) {
        // Find curve
        Curve curve = curveRepository.findById(curveId)
            .orElseThrow(() -> new IllegalArgumentException("Curve not found with id: " + curveId));
        
        // Validation: At least 1 instrument (FR-006)
        if (request.getInstruments() == null || request.getInstruments().isEmpty()) {
            throw new IllegalArgumentException("At least 1 instrument is required");
        }
        
        // Validation: No duplicate tenors (FR-005)
        Set<String> tenors = new HashSet<>();
        for (InstrumentInput inst : request.getInstruments()) {
            if (!tenors.add(inst.getTenor())) {
                throw new IllegalArgumentException("Duplicate tenor found: " + inst.getTenor());
            }
        }
        
        // Remove old instruments
        curve.getInstruments().clear();
        entityManager.flush(); // Ensure deletions are persisted
        
        // Add new instruments
        for (InstrumentInput instInput : request.getInstruments()) {
            Instrument instrument = new Instrument(
                instInput.getType(),
                instInput.getTenor()
            );
            curve.addInstrument(instrument);
        }
        
        // Save curve (cascades to instruments)
        Curve updatedCurve = curveRepository.save(curve);
        
        return toResponse(updatedCurve);
    }
    
    /**
     * Delete a curve version by name and date.
     * Cascades to instruments and quotes due to database foreign key constraints.
     */
    @Transactional
    public void deleteCurve(String name, LocalDate date) {
        Instant curveDate = dateTimeService.toEasternTimeInstant(date);
        
        Curve curve = curveRepository.findByNameAndCurveDate(name, curveDate)
            .orElseThrow(() -> new IllegalArgumentException(
                "Curve not found with name '" + name + "' and date '" + date + "'"
            ));
        
        curveRepository.delete(curve);
    }
    
    /**
     * Convert Curve entity to CurveSummary DTO.
     */
    private CurveSummary toSummary(Curve curve) {
        return new CurveSummary(
            curve.getId(),
            curve.getName(),
            curve.getCurveDate(),
            curve.getCurrency(),
            curve.getIndexName(),
            curve.getInstruments().size(),
            curve.getCreatedAt()
        );
    }
}
