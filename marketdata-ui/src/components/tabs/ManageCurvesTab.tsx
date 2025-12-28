import React from 'react';
import {
  Box,
  Typography,
  CircularProgress,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Chip,
  Select,
  MenuItem,
  FormControl,
} from '@mui/material';
import { Edit as EditIcon, Delete as DeleteIcon } from '@mui/icons-material';

export interface CurveNameGroup {
  name: string;
  dates: string[];
  selectedDate: string;
  curveDetails: {
    currency: string;
    index: string;
    instrumentCount: number;
  } | null;
}

interface ManageCurvesTabProps {
  loading: boolean;
  curveGroups: CurveNameGroup[];
  onDateChange: (curveName: string, newDate: string) => Promise<void>;
  onEditClick: (curveName: string, date: string) => Promise<void>;
  onDeleteClick: (name: string, date: string) => void;
}

const ManageCurvesTab: React.FC<ManageCurvesTabProps> = ({
  loading,
  curveGroups,
  onDateChange,
  onEditClick,
  onDeleteClick,
}) => {
  const formatDate = (dateStr: string) => {
    try {
      return new Date(dateStr).toLocaleDateString();
    } catch {
      return dateStr;
    }
  };

  return (
    <Box p={2}>
      <Typography variant="h6" gutterBottom>
        Existing Curves
      </Typography>
      
      {loading ? (
        <Box display="flex" justifyContent="center" p={4}>
          <CircularProgress />
        </Box>
      ) : curveGroups.length === 0 ? (
        <Typography color="text.secondary" align="center" p={4}>
          No curves found. Create your first curve using the "Create New Curve" tab.
        </Typography>
      ) : (
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Curve Name</strong></TableCell>
                <TableCell><strong>Select Date</strong></TableCell>
                <TableCell><strong>Currency</strong></TableCell>
                <TableCell><strong>Index</strong></TableCell>
                <TableCell align="center"><strong>Instruments</strong></TableCell>
                <TableCell align="center"><strong>Actions</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {curveGroups.map((group) => (
                <TableRow key={group.name} hover>
                  <TableCell>
                    <Typography variant="body1" fontWeight="medium">
                      {group.name}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <FormControl size="small" sx={{ minWidth: 150 }}>
                      <Select
                        value={group.selectedDate}
                        onChange={(e) => onDateChange(group.name, e.target.value)}
                      >
                        {group.dates.map((date) => (
                          <MenuItem key={date} value={date}>
                            {formatDate(date)}
                          </MenuItem>
                        ))}
                      </Select>
                    </FormControl>
                  </TableCell>
                  <TableCell>
                    {group.curveDetails ? (
                      <Chip label={group.curveDetails.currency} size="small" />
                    ) : (
                      <CircularProgress size={20} />
                    )}
                  </TableCell>
                  <TableCell>
                    {group.curveDetails ? (
                      <Chip label={group.curveDetails.index} size="small" variant="outlined" />
                    ) : (
                      <CircularProgress size={20} />
                    )}
                  </TableCell>
                  <TableCell align="center">
                    {group.curveDetails ? group.curveDetails.instrumentCount : '-'}
                  </TableCell>
                  <TableCell align="center">
                    <IconButton
                      size="small"
                      color="primary"
                      onClick={() => onEditClick(group.name, group.selectedDate)}
                      title="Edit curve"
                      disabled={!group.curveDetails}
                    >
                      <EditIcon />
                    </IconButton>
                    <IconButton
                      size="small"
                      color="error"
                      onClick={() => onDeleteClick(group.name, group.selectedDate)}
                      title="Delete curve"
                      disabled={!group.curveDetails}
                    >
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
};

export default ManageCurvesTab;
