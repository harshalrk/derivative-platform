/**
 * Validation utilities for Market Data UI.
 * Provides client-side validation for curves, instruments, and quotes.
 */

/**
 * Valid currency codes (must match backend enum).
 */
export const VALID_CURRENCIES = ['USD', 'EUR', 'GBP', 'JPY'] as const;
export type Currency = typeof VALID_CURRENCIES[number];

/**
 * Valid index names (must match backend enum).
 */
export const VALID_INDEXES = ['SOFR', 'LIBOR', 'EURIBOR', 'SONIA'] as const;
export type Index = typeof VALID_INDEXES[number];

/**
 * Valid instrument types (must match backend enum).
 */
export const VALID_INSTRUMENT_TYPES = ['MONEY_MARKET', 'FUTURE', 'SWAP'] as const;
export type InstrumentType = typeof VALID_INSTRUMENT_TYPES[number];

/**
 * Valid tenor codes (must match backend enum).
 */
export const VALID_TENORS = ['ON', '1M', '3M', '6M', '1Y', '2Y', '5Y', '10Y', '30Y'] as const;
export type Tenor = typeof VALID_TENORS[number];

/**
 * Validation result interface.
 */
export interface ValidationResult {
  isValid: boolean;
  errors: string[];
}

/**
 * Validate curve name.
 */
export function validateCurveName(name: string): ValidationResult {
  const errors: string[] = [];
  
  if (!name || name.trim().length === 0) {
    errors.push('Curve name is required');
  } else if (name.length > 100) {
    errors.push('Curve name must not exceed 100 characters');
  }
  
  return {
    isValid: errors.length === 0,
    errors,
  };
}

/**
 * Validate currency code.
 */
export function validateCurrency(currency: string): ValidationResult {
  const errors: string[] = [];
  
  if (!currency) {
    errors.push('Currency is required');
  } else if (!VALID_CURRENCIES.includes(currency as Currency)) {
    errors.push(`Currency must be one of: ${VALID_CURRENCIES.join(', ')}`);
  }
  
  return {
    isValid: errors.length === 0,
    errors,
  };
}

/**
 * Validate index name.
 */
export function validateIndex(index: string): ValidationResult {
  const errors: string[] = [];
  
  if (!index) {
    errors.push('Index is required');
  } else if (!VALID_INDEXES.includes(index as Index)) {
    errors.push(`Index must be one of: ${VALID_INDEXES.join(', ')}`);
  }
  
  return {
    isValid: errors.length === 0,
    errors,
  };
}

/**
 * Validate curve date.
 */
export function validateCurveDate(date: string | Date): ValidationResult {
  const errors: string[] = [];
  
  if (!date) {
    errors.push('Curve date is required');
  } else {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    if (isNaN(dateObj.getTime())) {
      errors.push('Invalid date format');
    }
  }
  
  return {
    isValid: errors.length === 0,
    errors,
  };
}

/**
 * Validate instrument type.
 */
export function validateInstrumentType(type: string): ValidationResult {
  const errors: string[] = [];
  
  if (!type) {
    errors.push('Instrument type is required');
  } else if (!VALID_INSTRUMENT_TYPES.includes(type as InstrumentType)) {
    errors.push(`Instrument type must be one of: ${VALID_INSTRUMENT_TYPES.join(', ')}`);
  }
  
  return {
    isValid: errors.length === 0,
    errors,
  };
}

/**
 * Validate tenor.
 */
export function validateTenor(tenor: string): ValidationResult {
  const errors: string[] = [];
  
  if (!tenor) {
    errors.push('Tenor is required');
  } else if (!VALID_TENORS.includes(tenor as Tenor)) {
    errors.push(`Tenor must be one of: ${VALID_TENORS.join(', ')}`);
  }
  
  return {
    isValid: errors.length === 0,
    errors,
  };
}

/**
 * Validate quote value (2 decimal precision, can be negative).
 */
export function validateQuoteValue(value: number | string): ValidationResult {
  const errors: string[] = [];
  
  if (value === null || value === undefined || value === '') {
    errors.push('Quote value is required');
  } else {
    const numValue = typeof value === 'string' ? parseFloat(value) : value;
    
    if (isNaN(numValue)) {
      errors.push('Quote value must be a valid number');
    } else {
      // Check decimal places (max 2)
      const strValue = numValue.toString();
      const decimalIndex = strValue.indexOf('.');
      if (decimalIndex !== -1 && strValue.length - decimalIndex - 1 > 2) {
        errors.push('Quote value must have at most 2 decimal places');
      }
    }
  }
  
  return {
    isValid: errors.length === 0,
    errors,
  };
}

/**
 * Validate array of instruments (at least 1, no duplicate tenors).
 */
export function validateInstruments(instruments: { tenor: string }[]): ValidationResult {
  const errors: string[] = [];
  
  if (!instruments || instruments.length === 0) {
    errors.push('At least 1 instrument is required');
  } else {
    // Check for duplicate tenors
    const tenors = instruments.map(i => i.tenor);
    const uniqueTenors = new Set(tenors);
    
    if (tenors.length !== uniqueTenors.size) {
      errors.push('Duplicate tenors are not allowed within a curve');
    }
  }
  
  return {
    isValid: errors.length === 0,
    errors,
  };
}

/**
 * Format a decimal number to 2 decimal places.
 */
export function formatQuoteValue(value: number): string {
  return value.toFixed(2);
}

/**
 * Parse a quote value string to number (2 decimal places).
 */
export function parseQuoteValue(value: string): number {
  return parseFloat(parseFloat(value).toFixed(2));
}
