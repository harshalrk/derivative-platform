import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  MenuItem,
  Alert,
  Paper,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import {
  InstrumentInput,
  ReferenceDataItem,
  getInstrumentTypes,
  getTenors,
} from '../services/curveService';

interface InstrumentBuilderProps {
  instruments: InstrumentInput[];
  onChange: (instruments: InstrumentInput[]) => void;
  error?: string;
}

/**
 * Instrument Builder component for adding/removing instruments.
 */
const InstrumentBuilder: React.FC<InstrumentBuilderProps> = ({
  instruments,
  onChange,
  error,
}) => {
  const [instrumentTypes, setInstrumentTypes] = useState<ReferenceDataItem[]>([]);
  const [tenors, setTenors] = useState<ReferenceDataItem[]>([]);

  // Load reference data
  useEffect(() => {
    const loadReferenceData = async () => {
      try {
        const [typesData, tenorsData] = await Promise.all([
          getInstrumentTypes(),
          getTenors(),
        ]);
        setInstrumentTypes(typesData);
        setTenors(tenorsData);
      } catch (err) {
        console.error('Failed to load instrument reference data:', err);
      }
    };
    loadReferenceData();
  }, []);

  const handleAdd = () => {
    onChange([...instruments, { type: '', tenor: '' }]);
  };

  const handleRemove = (index: number) => {
    const newInstruments = instruments.filter((_, i) => i !== index);
    onChange(newInstruments);
  };

  const handleChange = (index: number, field: 'type' | 'tenor', value: string) => {
    const newInstruments = [...instruments];
    newInstruments[index] = { ...newInstruments[index], [field]: value };
    onChange(newInstruments);
  };

  return (
    <Box>
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <TableContainer component={Paper} variant="outlined">
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Instrument Type</TableCell>
              <TableCell>Tenor</TableCell>
              <TableCell width="80px">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {instruments.length === 0 && (
              <TableRow>
                <TableCell colSpan={3} align="center" sx={{ py: 3 }}>
                  <em>No instruments added yet. Click "Add Instrument" to begin.</em>
                </TableCell>
              </TableRow>
            )}
            {instruments.map((instrument, index) => (
              <TableRow key={index}>
                <TableCell>
                  <TextField
                    select
                    fullWidth
                    size="small"
                    value={instrument.type}
                    onChange={(e) => handleChange(index, 'type', e.target.value)}
                    required
                  >
                    {instrumentTypes.map((type) => (
                      <MenuItem key={type.type} value={type.type}>
                        {type.description}
                      </MenuItem>
                    ))}
                  </TextField>
                </TableCell>
                <TableCell>
                  <TextField
                    select
                    fullWidth
                    size="small"
                    value={instrument.tenor}
                    onChange={(e) => handleChange(index, 'tenor', e.target.value)}
                    required
                  >
                    {tenors.map((tenor) => (
                      <MenuItem key={tenor.code} value={tenor.code}>
                        {tenor.description}
                      </MenuItem>
                    ))}
                  </TextField>
                </TableCell>
                <TableCell>
                  <IconButton
                    size="small"
                    color="error"
                    onClick={() => handleRemove(index)}
                  >
                    <DeleteIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Box sx={{ mt: 2 }}>
        <Button
          variant="outlined"
          startIcon={<AddIcon />}
          onClick={handleAdd}
        >
          Add Instrument
        </Button>
      </Box>
    </Box>
  );
};

export default InstrumentBuilder;
