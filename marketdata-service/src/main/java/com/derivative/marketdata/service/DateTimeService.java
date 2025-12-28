package com.derivative.marketdata.service;

import org.springframework.stereotype.Service;

import java.time.Instant;
import java.time.LocalDate;
import java.time.ZoneId;
import java.time.ZonedDateTime;

/**
 * Service for handling date/time conversions and Eastern Time operations.
 * All curve dates are stored as 5 PM Eastern Time (EST/EDT).
 */
@Service
public class DateTimeService {
    
    private static final ZoneId EASTERN_ZONE = ZoneId.of("America/New_York");
    private static final int CURVE_HOUR = 17; // 5 PM
    
    /**
     * Convert a LocalDate to an Instant representing 5 PM Eastern Time on that date.
     * Handles EST/EDT transition automatically.
     * 
     * @param date The date
     * @return Instant representing 5 PM Eastern Time on the given date
     */
    public Instant toEasternTimeInstant(LocalDate date) {
        return ZonedDateTime.of(
            date.atTime(CURVE_HOUR, 0),
            EASTERN_ZONE
        ).toInstant();
    }
    
    /**
     * Convert an Instant to a LocalDate in Eastern Time.
     * 
     * @param instant The instant
     * @return LocalDate in Eastern Time zone
     */
    public LocalDate toEasternDate(Instant instant) {
        return instant.atZone(EASTERN_ZONE).toLocalDate();
    }
    
    /**
     * Get the current date in Eastern Time.
     * 
     * @return Current LocalDate in Eastern Time zone
     */
    public LocalDate getCurrentEasternDate() {
        return LocalDate.now(EASTERN_ZONE);
    }
    
    /**
     * Get an Instant representing 5 PM Eastern Time today.
     * 
     * @return Instant for today at 5 PM Eastern
     */
    public Instant getTodayAt5PMEastern() {
        return toEasternTimeInstant(getCurrentEasternDate());
    }
}
