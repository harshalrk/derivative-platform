import React from 'react';
import { Box, Typography, CircularProgress } from '@mui/material';
import CurveForm from '../CurveForm';
import { CurveRequest } from '../../services/curveService';

interface CreateCurveTabProps {
  loading: boolean;
  onSubmit: (data: CurveRequest) => Promise<void>;
}

const CreateCurveTab: React.FC<CreateCurveTabProps> = ({ loading, onSubmit }) => {
  return (
    <Box p={3}>
      <Typography variant="h6" gutterBottom>
        Create New Curve
      </Typography>
      {loading ? (
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
          <CircularProgress />
        </Box>
      ) : (
        <CurveForm onSubmit={onSubmit} />
      )}
    </Box>
  );
};

export default CreateCurveTab;
