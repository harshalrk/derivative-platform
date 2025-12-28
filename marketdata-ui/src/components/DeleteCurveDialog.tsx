import React from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
} from '@mui/material';

interface DeleteCurveDialogProps {
  open: boolean;
  curveName: string | null;
  curveDate: string | null;
  onConfirm: () => void;
  onCancel: () => void;
}

const DeleteCurveDialog: React.FC<DeleteCurveDialogProps> = ({
  open,
  curveName,
  curveDate,
  onConfirm,
  onCancel,
}) => {
  const formatDate = (dateStr: string | null) => {
    if (!dateStr) return '';
    try {
      return new Date(dateStr).toLocaleDateString();
    } catch {
      return dateStr;
    }
  };

  return (
    <Dialog open={open} onClose={onCancel}>
      <DialogTitle>Confirm Delete</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Are you sure you want to delete curve <strong>{curveName}</strong> for date{' '}
          <strong>{formatDate(curveDate)}</strong>?
          <br /><br />
          This will also delete all associated quotes. This action cannot be undone.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel}>Cancel</Button>
        <Button onClick={onConfirm} color="error" variant="contained">
          Delete
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeleteCurveDialog;
