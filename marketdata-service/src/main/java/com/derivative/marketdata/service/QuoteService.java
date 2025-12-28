package com.derivative.marketdata.service;

import com.derivative.marketdata.model.dto.QuoteInput;
import com.derivative.marketdata.model.dto.QuoteOutput;
import com.derivative.marketdata.model.dto.QuoteRequest;
import com.derivative.marketdata.model.dto.QuoteResponse;
import com.derivative.marketdata.model.entity.Curve;
import com.derivative.marketdata.model.entity.Instrument;
import com.derivative.marketdata.model.entity.Quote;
import com.derivative.marketdata.repository.CurveRepository;
import com.derivative.marketdata.repository.InstrumentRepository;
import com.derivative.marketdata.repository.QuoteRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.time.Instant;
import java.time.LocalDate;
import java.util.*;
import java.util.stream.Collectors;

/**
 * Service for quote operations including save, retrieve, and validation.
 */
@Service
public class QuoteService {
    
    private final CurveRepository curveRepository;
    private final InstrumentRepository instrumentRepository;
    private final QuoteRepository quoteRepository;
    private final DateTimeService dateTimeService;
    
    public QuoteService(CurveRepository curveRepository,
                       InstrumentRepository instrumentRepository,
                       QuoteRepository quoteRepository,
                       DateTimeService dateTimeService) {
        this.curveRepository = curveRepository;
        this.instrumentRepository = instrumentRepository;
        this.quoteRepository = quoteRepository;
        this.dateTimeService = dateTimeService;
    }
    
    /**
     * Save quotes for a curve.
     * Validates: all instruments have quotes, 2 decimal precision.
     * Implements overwrite logic: updates existing quotes or creates new ones.
     */
    @Transactional
    public QuoteResponse saveQuotes(QuoteRequest request) {
        // Convert date to 5 PM Eastern Time
        Instant curveDate = dateTimeService.toEasternTimeInstant(request.getCurveDate());
        
        // Find the curve
        Curve curve = curveRepository.findByNameAndCurveDate(request.getCurveName(), curveDate)
            .orElseThrow(() -> new IllegalArgumentException(
                "Curve not found with name '" + request.getCurveName() + 
                "' and date '" + request.getCurveDate() + "'"
            ));
        
        // Get all instruments for this curve
        List<Instrument> instruments = instrumentRepository.findByCurveId(curve.getId());
        
        // Validation: Check all instruments have quotes (FR-014, FR-017)
        Set<String> providedTenors = request.getQuotes().stream()
            .map(QuoteInput::getTenor)
            .collect(Collectors.toSet());
        
        List<String> missingInstruments = instruments.stream()
            .filter(i -> !providedTenors.contains(i.getTenor()))
            .map(i -> i.getInstrumentType() + "/" + i.getTenor())
            .collect(Collectors.toList());
        
        if (!missingInstruments.isEmpty()) {
            throw new IllegalArgumentException(
                "All instruments must have quote values. Missing quotes for: " + 
                String.join(", ", missingInstruments)
            );
        }
        
        // Create a map of tenor -> instrument for quick lookup
        Map<String, Instrument> instrumentMap = instruments.stream()
            .collect(Collectors.toMap(Instrument::getTenor, i -> i));
        
        // Process each quote
        List<QuoteOutput> savedQuotes = new ArrayList<>();
        
        for (QuoteInput quoteInput : request.getQuotes()) {
            Instrument instrument = instrumentMap.get(quoteInput.getTenor());
            
            if (instrument == null) {
                throw new IllegalArgumentException(
                    "No instrument found with tenor: " + quoteInput.getTenor()
                );
            }
            
            // Validation: 2 decimal precision (FR-016)
            BigDecimal value = quoteInput.getValue();
            if (value.scale() > 2) {
                value = value.setScale(2, RoundingMode.HALF_UP);
            }
            
            // Check if quote already exists (overwrite logic - FR-020)
            Optional<Quote> existingQuote = quoteRepository.findByInstrumentId(instrument.getId());
            
            Quote quote;
            if (existingQuote.isPresent()) {
                // Update existing quote
                quote = existingQuote.get();
                quote.setValue(value);
            } else {
                // Create new quote
                quote = new Quote(value);
                quote.setInstrument(instrument);
            }
            
            quote = quoteRepository.save(quote);
            
            savedQuotes.add(new QuoteOutput(
                quote.getId(),
                instrument.getId(),
                instrument.getInstrumentType(),
                instrument.getTenor(),
                quote.getValue(),
                quote.getCreatedAt(),
                quote.getUpdatedAt()
            ));
        }
        
        return new QuoteResponse(
            curve.getId(),
            curve.getName(),
            curve.getCurveDate(),
            savedQuotes
        );
    }
    
    /**
     * Get all quotes for a curve by name and date.
     */
    @Transactional(readOnly = true)
    public Optional<QuoteResponse> getQuotesByCurve(String curveName, LocalDate curveDate) {
        Instant curveInstant = dateTimeService.toEasternTimeInstant(curveDate);
        
        Optional<Curve> curveOpt = curveRepository.findByNameAndCurveDate(curveName, curveInstant);
        
        if (curveOpt.isEmpty()) {
            return Optional.empty();
        }
        
        Curve curve = curveOpt.get();
        List<Quote> quotes = quoteRepository.findByCurveId(curve.getId());
        
        // Get instruments for additional info
        List<Instrument> instruments = instrumentRepository.findByCurveId(curve.getId());
        Map<UUID, Instrument> instrumentMap = instruments.stream()
            .collect(Collectors.toMap(Instrument::getId, i -> i));
        
        List<QuoteOutput> quoteOutputs = quotes.stream()
            .map(q -> {
                Instrument inst = instrumentMap.get(q.getInstrument().getId());
                return new QuoteOutput(
                    q.getId(),
                    inst.getId(),
                    inst.getInstrumentType(),
                    inst.getTenor(),
                    q.getValue(),
                    q.getCreatedAt(),
                    q.getUpdatedAt()
                );
            })
            .collect(Collectors.toList());
        
        return Optional.of(new QuoteResponse(
            curve.getId(),
            curve.getName(),
            curve.getCurveDate(),
            quoteOutputs
        ));
    }
}
