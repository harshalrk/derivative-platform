package com.derivative.marketdata.controller;

import com.derivative.marketdata.model.enums.Currency;
import com.derivative.marketdata.model.enums.Index;
import com.derivative.marketdata.model.enums.InstrumentType;
import com.derivative.marketdata.model.enums.Tenor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

/**
 * REST controller for reference data endpoints.
 * Provides hardcoded reference data for currencies, indexes, instrument types, and tenors.
 */
@RestController
@RequestMapping("/api/reference")
public class ReferenceDataController {
    
    /**
     * Get all available currencies.
     * @return List of currency objects with code and description
     */
    @GetMapping("/currencies")
    public ResponseEntity<List<Map<String, String>>> getCurrencies() {
        List<Map<String, String>> currencies = Arrays.stream(Currency.values())
            .map(c -> Map.of(
                "code", c.getCode(),
                "description", c.getDescription()
            ))
            .collect(Collectors.toList());
        
        return ResponseEntity.ok(currencies);
    }
    
    /**
     * Get all available interest rate indexes.
     * @return List of index objects with name and description
     */
    @GetMapping("/indexes")
    public ResponseEntity<List<Map<String, String>>> getIndexes() {
        List<Map<String, String>> indexes = Arrays.stream(Index.values())
            .map(i -> Map.of(
                "name", i.getName(),
                "description", i.getDescription()
            ))
            .collect(Collectors.toList());
        
        return ResponseEntity.ok(indexes);
    }
    
    /**
     * Get all available instrument types.
     * @return List of instrument type objects with type and description
     */
    @GetMapping("/instrument-types")
    public ResponseEntity<List<Map<String, String>>> getInstrumentTypes() {
        List<Map<String, String>> types = Arrays.stream(InstrumentType.values())
            .map(t -> Map.of(
                "type", t.getTypeName(),
                "description", t.getDescription()
            ))
            .collect(Collectors.toList());
        
        return ResponseEntity.ok(types);
    }
    
    /**
     * Get all available tenors.
     * @return List of tenor objects with code and description
     */
    @GetMapping("/tenors")
    public ResponseEntity<List<Map<String, String>>> getTenors() {
        List<Map<String, String>> tenors = Arrays.stream(Tenor.values())
            .map(t -> Map.of(
                "code", t.getCode(),
                "description", t.getDescription()
            ))
            .collect(Collectors.toList());
        
        return ResponseEntity.ok(tenors);
    }
}
