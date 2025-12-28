package com.derivative.marketdata.service;

import com.derivative.marketdata.model.dto.QuoteOutput;
import com.derivative.marketdata.model.dto.RollRequest;
import com.derivative.marketdata.model.dto.RollResponse;
import com.derivative.marketdata.model.entity.Curve;
import com.derivative.marketdata.model.entity.Instrument;
import com.derivative.marketdata.model.entity.Quote;
import com.derivative.marketdata.repository.CurveRepository;
import com.derivative.marketdata.repository.InstrumentRepository;
import com.derivative.marketdata.repository.QuoteRepository;
import jakarta.persistence.EntityManager;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.time.Instant;
import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

/**
 * Service for rolling curves and quotes to new dates.
 * Implements User Story 3: Copy both curve structure and quotes from previous date to target date.
 */
@Service
public class RollService {
    
    private final CurveRepository curveRepository;
    private final InstrumentRepository instrumentRepository;
    private final QuoteRepository quoteRepository;
    private final DateTimeService dateTimeService;
    private final EntityManager entityManager;
    
    public RollService(CurveRepository curveRepository,
                      InstrumentRepository instrumentRepository,
                      QuoteRepository quoteRepository,
                      DateTimeService dateTimeService,
                      EntityManager entityManager) {
        this.curveRepository = curveRepository;
        this.instrumentRepository = instrumentRepository;
        this.quoteRepository = quoteRepository;
        this.dateTimeService = dateTimeService;
        this.entityManager = entityManager;
    }
    
    /**
     * Roll curve and quotes from the most recent previous date to the target date.
     * Validates that the previous curve has complete quotes before rolling.
     */
    @Transactional
    public RollResponse rollCurveAndQuotes(RollRequest request) {
        String curveName = request.getCurveName();
        LocalDate targetDate = request.getTargetDate();
        boolean overwrite = request.isOverwrite();
        
        // Find the most recent previous curve with complete quotes
        Curve sourceCurve = findPreviousCurveWithQuotes(curveName, targetDate)
            .orElseThrow(() -> new IllegalArgumentException(
                "No previous curve found for '" + curveName + "' before " + targetDate + " with complete quotes"
            ));
        
        LocalDate sourceDate = dateTimeService.toEasternDate(sourceCurve.getCurveDate());
        
        // Validate source curve has complete quotes
        List<String> missingQuotes = validateCompleteQuotes(sourceCurve);
        if (!missingQuotes.isEmpty()) {
            throw new IllegalArgumentException(
                "Source curve on " + sourceDate + " has incomplete quotes. Missing: " + 
                String.join(", ", missingQuotes)
            );
        }
        
        // Check if target already exists
        Instant targetDateInstant = dateTimeService.toEasternTimeInstant(targetDate);
        Optional<Curve> existingTarget = curveRepository.findByNameAndCurveDate(curveName, targetDateInstant);
        
        if (existingTarget.isPresent() && !overwrite) {
            throw new IllegalArgumentException(
                "Curve '" + curveName + "' already exists on " + targetDate + 
                ". Set overwrite=true to replace it."
            );
        }
        
        // Delete existing target if overwriting
        Curve targetCurve;
        if (existingTarget.isPresent() && overwrite) {
            Curve curveToDelete = existingTarget.get();
            // Delete old quotes
            quoteRepository.deleteAll(quoteRepository.findByCurveId(curveToDelete.getId()));
            // Delete old instruments
            instrumentRepository.deleteAll(curveToDelete.getInstruments());
            // Delete old curve
            curveRepository.delete(curveToDelete);
            // Flush to ensure deletion is committed before creating new curve
            entityManager.flush();
        }
        
        // Create new curve with same structure
        targetCurve = new Curve(
            sourceCurve.getName(),
            targetDateInstant,
            sourceCurve.getCurrency(),
            sourceCurve.getIndexName()
        );
        
        // Copy instruments
        for (Instrument sourceInst : sourceCurve.getInstruments()) {
            Instrument newInst = new Instrument(
                sourceInst.getInstrumentType(),
                sourceInst.getTenor()
            );
            targetCurve.addInstrument(newInst);
        }
        
        // Save curve (cascades to instruments)
        targetCurve = curveRepository.save(targetCurve);
        
        // Copy quotes
        List<Quote> sourceQuotes = quoteRepository.findByCurveId(sourceCurve.getId());
        List<QuoteOutput> copiedQuotes = new ArrayList<>();
        
        for (Quote sourceQuote : sourceQuotes) {
            // Find matching instrument in target curve
            Instrument targetInst = targetCurve.getInstruments().stream()
                .filter(i -> i.getInstrumentType().equals(sourceQuote.getInstrument().getInstrumentType()) &&
                            i.getTenor().equals(sourceQuote.getInstrument().getTenor()))
                .findFirst()
                .orElseThrow(() -> new IllegalStateException("Matching instrument not found"));
            
            // Create new quote
            Quote newQuote = new Quote(sourceQuote.getValue());
            newQuote.setInstrument(targetInst);
            Quote savedQuote = quoteRepository.save(newQuote);
            
            // Add to response
            copiedQuotes.add(new QuoteOutput(
                savedQuote.getId(),
                targetInst.getId(),
                targetInst.getInstrumentType(),
                targetInst.getTenor(),
                savedQuote.getValue(),
                savedQuote.getCreatedAt(),
                savedQuote.getUpdatedAt()
            ));
        }
        
        String message = overwrite 
            ? "Rolled curve from " + sourceDate + " to " + targetDate + " (overwrite)"
            : "Rolled curve from " + sourceDate + " to " + targetDate;
        
        return new RollResponse(
            sourceCurve.getId(),
            sourceDate,
            targetCurve.getId(),
            targetDate,
            targetCurve.getInstruments().size(),
            copiedQuotes,
            message
        );
    }
    
    /**
     * Find the most recent previous curve with complete quotes before the target date.
     */
    @Transactional(readOnly = true)
    public Optional<Curve> findPreviousCurveWithQuotes(String curveName, LocalDate targetDate) {
        Instant targetInstant = dateTimeService.toEasternTimeInstant(targetDate);
        
        // Get all curves for this name, ordered by date descending
        List<Curve> allCurves = curveRepository.findByNameOrderByCurveDateDesc(curveName);
        
        // Find the first curve before target date that has complete quotes
        for (Curve curve : allCurves) {
            if (curve.getCurveDate().isBefore(targetInstant)) {
                List<String> missing = validateCompleteQuotes(curve);
                if (missing.isEmpty()) {
                    return Optional.of(curve);
                }
            }
        }
        
        return Optional.empty();
    }
    
    /**
     * Validate that all instruments have quotes.
     * @return List of missing instrument descriptions (empty if complete)
     */
    @Transactional(readOnly = true)
    public List<String> validateCompleteQuotes(Curve curve) {
        List<Quote> quotes = quoteRepository.findByCurveId(curve.getId());
        List<String> missingInstruments = new ArrayList<>();
        
        for (Instrument instrument : curve.getInstruments()) {
            boolean hasQuote = quotes.stream()
                .anyMatch(q -> q.getInstrument().getId().equals(instrument.getId()));
            
            if (!hasQuote) {
                missingInstruments.add(instrument.getInstrumentType() + "/" + instrument.getTenor());
            }
        }
        
        return missingInstruments;
    }
}
