package com.derivative.marketdata.controller;

import com.derivative.marketdata.model.dto.CurveRequest;
import com.derivative.marketdata.model.dto.CurveResponse;
import com.derivative.marketdata.model.dto.CurveSummary;
import com.derivative.marketdata.model.dto.UpdateCurveRequest;
import com.derivative.marketdata.service.CurveService;
import jakarta.validation.Valid;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;
import java.util.UUID;

/**
 * REST controller for curve operations.
 */
@RestController
@RequestMapping("/api/curves")
public class CurveController {
    
    private final CurveService curveService;
    
    public CurveController(CurveService curveService) {
        this.curveService = curveService;
    }
    
    /**
     * Create a new curve.
     * POST /api/curves
     */
    @PostMapping
    public ResponseEntity<CurveResponse> createCurve(@Valid @RequestBody CurveRequest request) {
        CurveResponse response = curveService.createCurve(request);
        return ResponseEntity.status(HttpStatus.CREATED).body(response);
    }
    
    /**
     * Get a curve by name and date.
     * GET /api/curves/query?name=USD-SOFR&date=2025-12-25
     */
    @GetMapping("/query")
    public ResponseEntity<CurveResponse> getCurveByNameAndDate(
            @RequestParam String name,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        
        return curveService.getCurveByNameAndDate(name, date)
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.notFound().build());
    }
    
    /**
     * Get a curve by ID.
     * GET /api/curves/{id}
     */
    @GetMapping("/{id}")
    public ResponseEntity<CurveResponse> getCurveById(@PathVariable UUID id) {
        return curveService.getCurveById(id)
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.notFound().build());
    }
    
    /**
     * List all curves for a given name (all temporal versions).
     * GET /api/curves?name=USD-SOFR
     */
    @GetMapping(params = "name")
    public ResponseEntity<List<CurveSummary>> listCurvesByName(@RequestParam String name) {
        List<CurveSummary> curves = curveService.listCurvesByName(name);
        return ResponseEntity.ok(curves);
    }
    
    /**
     * List all unique curve names.
     * GET /api/curves
     */
    @GetMapping
    public ResponseEntity<List<String>> listCurveNames() {
        List<String> names = curveService.listCurveNames();
        return ResponseEntity.ok(names);
    }
    
    /**
     * Get all available dates for a curve name.
     * GET /api/curves/dates?name=USD-SOFR
     */
    @GetMapping("/dates")
    public ResponseEntity<List<LocalDate>> getCurveDates(@RequestParam String name) {
        List<LocalDate> dates = curveService.getCurveDates(name);
        return ResponseEntity.ok(dates);
    }
    
    /**
     * Update an existing curve's instruments.
     * PUT /api/curves/{id}
     */
    @PutMapping("/{id}")
    public ResponseEntity<CurveResponse> updateCurve(
            @PathVariable UUID id,
            @Valid @RequestBody UpdateCurveRequest request) {
        CurveResponse response = curveService.updateCurve(id, request);
        return ResponseEntity.ok(response);
    }
    
    /**
     * Delete a curve version by name and date.
     * DELETE /api/curves?name=USD-SOFR&date=2025-12-25
     */
    @DeleteMapping
    public ResponseEntity<Void> deleteCurve(
            @RequestParam String name,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        curveService.deleteCurve(name, date);
        return ResponseEntity.noContent().build();
    }
}
