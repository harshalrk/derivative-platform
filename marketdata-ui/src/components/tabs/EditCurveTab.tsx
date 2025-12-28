import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  TextField,
  Typography,
  CircularProgress,
  Alert,
  Divider,
} from '@mui/material';
import InstrumentBuilder from '../InstrumentBuilder';
import { CurveRequest, CurveResponse, InstrumentInput } from '../../services/curveService';
import { validateInstruments } from '../../utils/validation';

interface EditCurveTabProps {
  loading: boolean;
  curve: CurveResponse;
  onSubmit: (data: CurveRequest) => Promise<void>;
  onCancel: () => void;
}

const EditCurveTab: React.FC<EditCurveTabProps> = ({ loading, curve, onSubmit, onCancel }) => {
  const [instruments, setInstruments] = useState<InstrumentInput[]>([]);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  const formatDate = (dateStr: string) => {
    try {
      return new Date(dateStr).toLocaleDateString();
    } catch {
      return dateStr;
    }
  };

  // Pre-populate instruments from the curve
  useEffect(() => {
    if (curve && curve.instruments) {
      const mappedInstruments: InstrumentInput[] = curve.instruments.map(inst => ({
        type: inst.type,
        tenor: inst.tenor,
      }));
      setInstruments(mappedInstruments);
    }
  }, [curve]);

  const handleInstrumentsChange = (newInstruments: InstrumentInput[]) => {
    setInstruments(newInstruments);
    // Clear instruments error when user makes changes
    if (errors.instruments) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors.instruments;
        return newErrors;
      });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const newErrors: { [key: string]: string } = {};
    const instrumentsError = validateInstruments(instruments);
    if (!instrumentsError.isValid) {
      newErrors.instruments = instrumentsError.errors.join(', ');
    }

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    const curveData: CurveRequest = {
      name: curve.name,
      date: curve.curveDate.split('T')[0], // Convert ISO instant to YYYY-MM-DD
      currency: curve.currency,
      index: curve.index,
      instruments,
    };

    await onSubmit(curveData);
  };

  return (
    <Box p={3}>
      <Typography variant="h6" gutterBottom>
        Edit Curve: {curve.name} ({formatDate(curve.curveDate)})
      </Typography>
      <Alert severity="info" sx={{ mb: 2 }}>
        Note: Only instruments can be modified. Name, date, currency, and index are immutable.
      </Alert>
      
      {loading ? (
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
          <CircularProgress />
        </Box>
      ) : (
        <Box component="form" onSubmit={handleSubmit} noValidate>
          {/* Immutable fields - disabled */}
          <TextField
            fullWidth
            label="Curve Name"
            value={curve.name}
            disabled
            margin="normal"
          />
          
          <TextField
            fullWidth
            label="Curve Date"
            type="date"
            value={curve.curveDate}
            disabled
            margin="normal"
            InputLabelProps={{ shrink: true }}
          />
          
          <TextField
            fullWidth
            label="Currency"
            value={curve.currency}
            disabled
            margin="normal"
          />
          
          <TextField
            fullWidth
            label="Index"
            value={curve.index}
            disabled
            margin="normal"
          />

          <Divider sx={{ my: 3 }} />

          {/* Editable instruments */}
          <Typography variant="h6" gutterBottom>
            Instruments (Editable)
          </Typography>
          
          <InstrumentBuilder
            instruments={instruments}
            onChange={handleInstrumentsChange}
            error={errors.instruments}
          />

          <Box mt={3} display="flex" gap={2}>
            <Button
              type="submit"
              variant="contained"
              color="primary"
              disabled={loading}
            >
              Update Curve
            </Button>
            <Button onClick={onCancel} variant="outlined">
              Cancel
            </Button>
          </Box>
        </Box>
      )}
    </Box>
  );
};

export default EditCurveTab;
