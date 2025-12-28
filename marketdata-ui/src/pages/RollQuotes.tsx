import { useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  CircularProgress,
  MenuItem,
  Grid,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Snackbar,
  Alert,
} from '@mui/material';
import { listCurveNames } from '../services/curveService';
import { rollQuotes, RollRequest, RollResponse } from '../services/quoteService';
import { useEffect } from 'react';
import { useNotification } from '../hooks/useNotification';

export default function RollQuotes() {
  const [curveNames, setCurveNames] = useState<string[]>([]);
  const [selectedCurveName, setSelectedCurveName] = useState<string>('');
  const [targetDate, setTargetDate] = useState<string>('');
  const [rollResult, setRollResult] = useState<RollResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const { notification, showSuccess, showError, hideNotification } = useNotification();
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);

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

  const handleRoll = async (overwrite: boolean = false) => {
    setRollResult(null);

    if (!selectedCurveName || !targetDate) {
      showError('Please select a curve name and target date');
      return;
    }

    setLoading(true);

    try {
      const request: RollRequest = {
        curveName: selectedCurveName,
        targetDate,
        overwrite
      };

      const response = await rollQuotes(request);
      setRollResult(response);
      showSuccess(response.message);
      setConfirmDialogOpen(false);
    } catch (err: any) {
      const errorMsg = err.response?.data?.message || err.message || 'Failed to roll quotes';
      
      // Check if it's an overwrite conflict
      if (errorMsg.includes('already exists') && !overwrite) {
        setConfirmDialogOpen(true);
      } else {
        showError(errorMsg);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleRollClick = () => {
    handleRoll(false);
  };

  const handleConfirmOverwrite = () => {
    handleRoll(true);
  };

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Roll Curves and Quotes
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
              fullWidth
              type="date"
              label="Target Date"
              value={targetDate}
              onChange={(e) => setTargetDate(e.target.value)}
              InputLabelProps={{ shrink: true }}
              disabled={loading}
            />
          </Grid>
        </Grid>

        <Box sx={{ mt: 2 }}>
          <Button
            variant="contained"
            color="primary"
            onClick={handleRollClick}
            disabled={loading || !selectedCurveName || !targetDate}
          >
            {loading ? 'Rolling...' : 'Roll to Target Date'}
          </Button>
        </Box>
      </Paper>

      {loading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
          <CircularProgress />
        </Box>
      )}

      {rollResult && (
        <Paper sx={{ p: 3 }}>
          <Typography variant="h6" gutterBottom>
            Roll Summary
          </Typography>
          
          <Grid container spacing={2} sx={{ mb: 2 }}>
            <Grid item xs={12} sm={6}>
              <Typography variant="body2" color="text.secondary">
                Source Date
              </Typography>
              <Typography variant="body1">
                {rollResult.sourceDate}
              </Typography>
            </Grid>
            <Grid item xs={12} sm={6}>
              <Typography variant="body2" color="text.secondary">
                Target Date
              </Typography>
              <Typography variant="body1">
                {rollResult.targetDate}
              </Typography>
            </Grid>
            <Grid item xs={12} sm={6}>
              <Typography variant="body2" color="text.secondary">
                Instrument Count
              </Typography>
              <Typography variant="body1">
                {rollResult.instrumentCount}
              </Typography>
            </Grid>
            <Grid item xs={12} sm={6}>
              <Typography variant="body2" color="text.secondary">
                Quotes Copied
              </Typography>
              <Typography variant="body1">
                {rollResult.quotes.length}
              </Typography>
            </Grid>
          </Grid>

          <Typography variant="h6" gutterBottom sx={{ mt: 3 }}>
            Copied Quotes
          </Typography>
          
          <TableContainer>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Instrument Type</TableCell>
                  <TableCell>Tenor</TableCell>
                  <TableCell align="right">Value</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {rollResult.quotes.map((quote) => (
                  <TableRow key={quote.quoteId}>
                    <TableCell>{quote.instrumentType}</TableCell>
                    <TableCell>{quote.tenor}</TableCell>
                    <TableCell align="right">{quote.value}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      )}

      {/* Overwrite Confirmation Dialog */}
      <Dialog open={confirmDialogOpen} onClose={() => setConfirmDialogOpen(false)}>
        <DialogTitle>Curve Already Exists</DialogTitle>
        <DialogContent>
          <DialogContentText>
            A curve for {selectedCurveName} already exists on {targetDate}. 
            Do you want to overwrite it with the rolled data?
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmDialogOpen(false)} color="inherit">
            Cancel
          </Button>
          <Button onClick={handleConfirmOverwrite} color="primary" variant="contained">
            Overwrite
          </Button>
        </DialogActions>
      </Dialog>

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
