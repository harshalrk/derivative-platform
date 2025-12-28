import { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  CircularProgress,
  MenuItem,
  Grid,
  Snackbar,
  Alert,
} from '@mui/material';
import { listCurveNames, getCurveByNameAndDate, getCurveDates } from '../services/curveService';
import { saveQuotes, getQuotes, QuoteInput, QuoteRequest } from '../services/quoteService';
import { useNotification } from '../hooks/useNotification';

interface QuoteRow {
  instrumentId: string;
  instrumentType: string;
  tenor: string;
  value: string;
}

export default function EnterQuotes() {
  const [curveNames, setCurveNames] = useState<string[]>([]);
  const [availableDates, setAvailableDates] = useState<string[]>([]);
  const [selectedCurveName, setSelectedCurveName] = useState<string>('');
  const [selectedDate, setSelectedDate] = useState<string>('');
  const [quoteRows, setQuoteRows] = useState<QuoteRow[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const { notification, showSuccess, showError, hideNotification } = useNotification();

  // Load curve names on mount
  useEffect(() => {
    const loadCurveNames = async () => {
      try {
        const names = await listCurveNames();
        setCurveNames(names);
      } catch (err: any) {
        showError(err.response?.data?.message || err.message || 'Failed to load curve names');
      }
    };
    loadCurveNames();
  }, []);

  // Load available dates when curve name changes
  useEffect(() => {
    if (selectedCurveName) {
      const loadDates = async () => {
        try {
          const dates = await getCurveDates(selectedCurveName);
          setAvailableDates(dates);
          // Auto-select most recent date if available
          if (dates.length > 0) {
            setSelectedDate(dates[0]);
          } else {
            setSelectedDate('');
          }
        } catch (err: any) {
          showError(err.response?.data?.message || err.message || 'Failed to load curve dates');
          setAvailableDates([]);
          setSelectedDate('');
        }
      };
      loadDates();
    } else {
      setAvailableDates([]);
      setSelectedDate('');
    }
  }, [selectedCurveName]);

  // Load curve and quotes when selection changes
  useEffect(() => {
    if (selectedCurveName && selectedDate) {
      loadCurveAndQuotes();
    } else {
      setQuoteRows([]);
    }
  }, [selectedCurveName, selectedDate]);

  const loadCurveAndQuotes = async () => {
    setLoading(true);
    
    try {
      // Load curve to get instruments
      const curve = await getCurveByNameAndDate(selectedCurveName, selectedDate);
      
      // Try to load existing quotes
      const existingQuotes = await getQuotes(selectedCurveName, selectedDate);
      
      if (existingQuotes) {
        // Prepopulate with existing quotes
        const rows: QuoteRow[] = curve.instruments.map(inst => {
          const quote = existingQuotes.quotes.find(q => q.instrumentId === inst.id);
          return {
            instrumentId: inst.id,
            instrumentType: inst.type,
            tenor: inst.tenor,
            value: quote ? quote.value : ''
          };
        });
        setQuoteRows(rows);
      } else {
        // Initialize empty rows
        const rows: QuoteRow[] = curve.instruments.map(inst => ({
          instrumentId: inst.id,
          instrumentType: inst.type,
          tenor: inst.tenor,
          value: ''
        }));
        setQuoteRows(rows);
      }
    } catch (err: any) {
      showError(err.response?.data?.message || err.message || 'Failed to load curve or quotes');
    } finally {
      setLoading(false);
    }
  };

  const handleQuoteChange = (index: number, value: string) => {
    const newRows = [...quoteRows];
    newRows[index].value = value;
    setQuoteRows(newRows);
  };

  const validateQuotes = (): string | null => {
    const missingInstruments: string[] = [];
    
    for (const row of quoteRows) {
      if (!row.value || row.value.trim() === '') {
        missingInstruments.push(`${row.instrumentType} ${row.tenor}`);
      }
    }
    
    if (missingInstruments.length > 0) {
      return `All instruments must have quote values. Missing quotes for: ${missingInstruments.join(', ')}`;
    }
    
    // Validate numeric values
    for (const row of quoteRows) {
      const numValue = parseFloat(row.value);
      if (isNaN(numValue)) {
        return `Invalid quote value for ${row.instrumentType} ${row.tenor}`;
      }
    }
    
    return null;
  };

  const handleSave = async () => {
    // Validate all quotes are filled
    const validationError = validateQuotes();
    if (validationError) {
      showError(validationError);
      return;
    }
    
    setSaving(true);
    
    try {
      const quotes: QuoteInput[] = quoteRows.map(row => ({
        instrumentType: row.instrumentType,
        tenor: row.tenor,
        value: parseFloat(row.value).toFixed(2) // Format to 2 decimals
      }));
      
      const request: QuoteRequest = {
        curveName: selectedCurveName,
        curveDate: selectedDate,
        quotes
      };
      
      await saveQuotes(request);
      showSuccess('Quotes saved successfully');
    } catch (err: any) {
      showError(err.response?.data?.message || err.message || 'Failed to save quotes');
    } finally {
      setSaving(false);
    }
  };

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Enter Daily Quotes
      </Typography>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} md={6}>
            <TextField
              select
              fullWidth
              label="Curve Name"
              value={selectedCurveName}
              onChange={(e) => setSelectedCurveName(e.target.value)}
              disabled={loading}
            >
              <MenuItem value="">Select a curve...</MenuItem>
              {curveNames.map((name) => (
                <MenuItem key={name} value={name}>
                  {name}
                </MenuItem>
              ))}
            </TextField>
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              select
              fullWidth
              label="Curve Date"
              value={selectedDate}
              onChange={(e) => setSelectedDate(e.target.value)}
              disabled={loading || !selectedCurveName || availableDates.length === 0}
            >
              <MenuItem value="">Select a date...</MenuItem>
              {availableDates.map((date) => (
                <MenuItem key={date} value={date}>
                  {date}
                </MenuItem>
              ))}
            </TextField>
          </Grid>
        </Grid>
      </Paper>

      {loading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
          <CircularProgress />
        </Box>
      )}

      {!loading && quoteRows.length > 0 && (
        <>
          <TableContainer component={Paper} sx={{ mb: 3 }}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Instrument Type</TableCell>
                  <TableCell>Tenor</TableCell>
                  <TableCell>Quote Value</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {quoteRows.map((row, index) => (
                  <TableRow key={row.instrumentId}>
                    <TableCell>{row.instrumentType}</TableCell>
                    <TableCell>{row.tenor}</TableCell>
                    <TableCell>
                      <TextField
                        type="number"
                        value={row.value}
                        onChange={(e) => handleQuoteChange(index, e.target.value)}
                        inputProps={{
                          step: '0.01',
                          min: '-999999.99',
                          max: '999999.99'
                        }}
                        sx={{ width: '150px' }}
                      />
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>

          <Box sx={{ display: 'flex', gap: 2 }}>
            <Button
              variant="contained"
              color="primary"
              onClick={handleSave}
              disabled={saving}
            >
              {saving ? 'Saving...' : 'Save Quotes'}
            </Button>
            <Button
              variant="outlined"
              onClick={loadCurveAndQuotes}
              disabled={loading || saving}
            >
              Reload
            </Button>
          </Box>
        </>
      )}

      {!loading && selectedCurveName && selectedDate && quoteRows.length === 0 && (
        <Alert severity="info">
          No curve found for {selectedCurveName} on {selectedDate}
        </Alert>
      )}

      {/* Snackbar for notifications */}
      <Snackbar
        open={notification.open}
        autoHideDuration={6000}
        onClose={hideNotification}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert onClose={hideNotification} severity={notification.type} sx={{ width: '100%' }}>
          {notification.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
