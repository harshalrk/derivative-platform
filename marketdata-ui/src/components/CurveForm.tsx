import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  TextField,
  MenuItem,
  Typography,
  Divider,
} from '@mui/material';
import InstrumentBuilder from './InstrumentBuilder';
import {
  CurveRequest,
  InstrumentInput,
  ReferenceDataItem,
  getCurrencies,
  getIndexes,
} from '../services/curveService';
import {
  validateCurveName,
  validateCurrency,
  validateIndex,
  validateCurveDate,
  validateInstruments,
} from '../utils/validation';

interface CurveFormProps {
  onSubmit: (data: CurveRequest) => void;
}

/**
 * Curve Form component for creating a new curve.
 */
const CurveForm: React.FC<CurveFormProps> = ({ onSubmit }) => {
  const [name, setName] = useState('');
  const [date, setDate] = useState('');
  const [currency, setCurrency] = useState('');
  const [index, setIndex] = useState('');
  const [instruments, setInstruments] = useState<InstrumentInput[]>([]);
  
  const [currencies, setCurrencies] = useState<ReferenceDataItem[]>([]);
  const [indexes, setIndexes] = useState<ReferenceDataItem[]>([]);
  
  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  // Load reference data on mount
  useEffect(() => {
    const loadReferenceData = async () => {
      try {
        const [currenciesData, indexesData] = await Promise.all([
          getCurrencies(),
          getIndexes(),
        ]);
        setCurrencies(currenciesData);
        setIndexes(indexesData);
      } catch (err) {
        console.error('Failed to load reference data:', err);
      }
    };
    loadReferenceData();
  }, []);

  const validate = (): boolean => {
    const newErrors: { [key: string]: string } = {};

    const nameValidation = validateCurveName(name);
    if (!nameValidation.isValid) {
      newErrors.name = nameValidation.errors[0];
    }

    const dateValidation = validateCurveDate(date);
    if (!dateValidation.isValid) {
      newErrors.date = dateValidation.errors[0];
    }

    const currencyValidation = validateCurrency(currency);
    if (!currencyValidation.isValid) {
      newErrors.currency = currencyValidation.errors[0];
    }

    const indexValidation = validateIndex(index);
    if (!indexValidation.isValid) {
      newErrors.index = indexValidation.errors[0];
    }

    const instrumentsValidation = validateInstruments(instruments);
    if (!instrumentsValidation.isValid) {
      newErrors.instruments = instrumentsValidation.errors[0];
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validate()) {
      const curveData: CurveRequest = {
        name,
        date,
        currency,
        index,
        instruments,
      };
      onSubmit(curveData);
    }
  };

  return (
    <Box component="form" onSubmit={handleSubmit} noValidate>
      <Typography variant="h6" gutterBottom>
        Curve Details
      </Typography>

      <TextField
        fullWidth
        label="Curve Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        error={!!errors.name}
        helperText={errors.name || 'e.g., USD-SOFR'}
        margin="normal"
        required
      />

      <TextField
        fullWidth
        label="Curve Date"
        type="date"
        value={date}
        onChange={(e) => setDate(e.target.value)}
        error={!!errors.date}
        helperText={errors.date || 'Date at 5 PM Eastern Time'}
        margin="normal"
        required
        InputLabelProps={{
          shrink: true,
        }}
      />

      <TextField
        fullWidth
        select
        label="Currency"
        value={currency}
        onChange={(e) => setCurrency(e.target.value)}
        error={!!errors.currency}
        helperText={errors.currency}
        margin="normal"
        required
      >
        {currencies.map((curr) => (
          <MenuItem key={curr.code} value={curr.code}>
            {curr.code} - {curr.description}
          </MenuItem>
        ))}
      </TextField>

      <TextField
        fullWidth
        select
        label="Index"
        value={index}
        onChange={(e) => setIndex(e.target.value)}
        error={!!errors.index}
        helperText={errors.index}
        margin="normal"
        required
      >
        {indexes.map((idx) => (
          <MenuItem key={idx.name} value={idx.name}>
            {idx.name} - {idx.description}
          </MenuItem>
        ))}
      </TextField>

      <Divider sx={{ my: 3 }} />

      <Typography variant="h6" gutterBottom>
        Term Structure (Instruments)
      </Typography>

      <InstrumentBuilder
        instruments={instruments}
        onChange={setInstruments}
        error={errors.instruments}
      />

      <Box sx={{ mt: 3, display: 'flex', justifyContent: 'flex-end' }}>
        <Button
          type="submit"
          variant="contained"
          color="primary"
          size="large"
        >
          Create Curve
        </Button>
      </Box>
    </Box>
  );
};

export default CurveForm;
