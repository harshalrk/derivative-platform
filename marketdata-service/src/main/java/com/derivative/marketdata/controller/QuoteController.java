package com.derivative.marketdata.controller;

import com.derivative.marketdata.model.dto.QuoteRequest;
import com.derivative.marketdata.model.dto.QuoteResponse;
import com.derivative.marketdata.model.dto.RollRequest;
import com.derivative.marketdata.model.dto.RollResponse;
import com.derivative.marketdata.service.QuoteService;
import com.derivative.marketdata.service.RollService;
import jakarta.validation.Valid;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;

/**
 * REST controller for quote operations.
 */
@RestController
@RequestMapping("/api/quotes")
public class QuoteController {
    
    private final QuoteService quoteService;
    private final RollService rollService;
    
    public QuoteController(QuoteService quoteService, RollService rollService) {
        this.quoteService = quoteService;
        this.rollService = rollService;
    }
    
    /**
     * Save quotes for a curve.
     * POST /api/quotes
     */
    @PostMapping
    public ResponseEntity<QuoteResponse> saveQuotes(@Valid @RequestBody QuoteRequest request) {
        QuoteResponse response = quoteService.saveQuotes(request);
        return ResponseEntity.status(HttpStatus.OK).body(response);
    }
    
    /**
     * Get quotes for a curve by name and date.
     * GET /api/quotes?curveName=USD-SOFR&curveDate=2025-12-25
     */
    @GetMapping
    public ResponseEntity<QuoteResponse> getQuotesByCurve(
            @RequestParam String curveName,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate curveDate) {
        
        return quoteService.getQuotesByCurve(curveName, curveDate)
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.notFound().build());
    }
    
    /**
     * Roll curve and quotes from previous date to target date.
     * POST /api/quotes/roll
     */
    @PostMapping("/roll")
    public ResponseEntity<RollResponse> rollQuotes(@Valid @RequestBody RollRequest request) {
        RollResponse response = rollService.rollCurveAndQuotes(request);
        return ResponseEntity.ok(response);
    }
}
