import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Paper,
  Box,
  Snackbar,
  Alert,
  Tabs,
  Tab,
} from '@mui/material';
import CreateCurveTab from '../components/tabs/CreateCurveTab';
import ManageCurvesTab, { CurveNameGroup } from '../components/tabs/ManageCurvesTab';
import EditCurveTab from '../components/tabs/EditCurveTab';
import DeleteCurveDialog from '../components/DeleteCurveDialog';
import {
  createCurve,
  updateCurve,
  deleteCurve,
  listCurveNames,
  listCurvesByName,
  getCurveByNameAndDate,
  CurveRequest,
  CurveResponse,
  UpdateCurveRequest,
} from '../services/curveService';
import { useNotification } from '../hooks/useNotification';

const DefineCurves: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const { notification, showSuccess, showError, hideNotification } = useNotification();
  
  const [activeTab, setActiveTab] = useState(0);
  const [showEditForm, setShowEditForm] = useState(false);
  const [editingCurve, setEditingCurve] = useState<CurveResponse | null>(null);
  const [curveGroups, setCurveGroups] = useState<CurveNameGroup[]>([]);
  const [loadingCurves, setLoadingCurves] = useState(true);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [curveToDelete, setCurveToDelete] = useState<{ name: string; date: string } | null>(null);

  useEffect(() => {
    loadCurveNames();
  }, []);

  const loadCurveNames = async () => {
    setLoadingCurves(true);
    try {
      const names = await listCurveNames();
      const groups: CurveNameGroup[] = [];
      
      for (const name of names) {
        const curvesForName = await listCurvesByName(name);
        const dates = curvesForName
          .map(c => c.curveDate)
          .sort((a, b) => new Date(b).getTime() - new Date(a).getTime());
        
        const selectedDate = dates[0];
        const curveDetails = curvesForName.find(c => c.curveDate === selectedDate) || null;
        
        groups.push({
          name,
          dates,
          selectedDate,
          curveDetails: curveDetails ? {
            currency: curveDetails.currency,
            index: curveDetails.index,
            instrumentCount: curveDetails.instrumentCount,
          } : null,
        });
      }
      
      setCurveGroups(groups);
    } catch (err: any) {
      console.error('Failed to load curves:', err);
      showError('Failed to load curve names');
    } finally {
      setLoadingCurves(false);
    }
  };

  const handleDateChange = async (curveName: string, newDate: string) => {
    setCurveGroups(prevGroups => 
      prevGroups.map(group => {
        if (group.name === curveName) {
          const curveDetails = group.curveDetails && group.selectedDate === newDate 
            ? group.curveDetails 
            : null;
          return { ...group, selectedDate: newDate, curveDetails };
        }
        return group;
      })
    );

    try {
      const curvesForName = await listCurvesByName(curveName);
      const curveDetails = curvesForName.find(c => c.curveDate === newDate);
      
      if (curveDetails) {
        setCurveGroups(prevGroups => 
          prevGroups.map(group => 
            group.name === curveName 
              ? { 
                  ...group, 
                  curveDetails: {
                    currency: curveDetails.currency,
                    index: curveDetails.index,
                    instrumentCount: curveDetails.instrumentCount,
                  }
                }
              : group
          )
        );
      }
    } catch (err: any) {
      console.error('Failed to load curve details:', err);
    }
  };

  const handleCreate = async (curveData: CurveRequest) => {
    setLoading(true);

    try {
      const response: CurveResponse = await createCurve(curveData);
      showSuccess(
        `Curve "${response.name}" created successfully with ${response.instruments.length} instruments!`
      );
      await loadCurveNames();
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message ||
        err.response?.data?.errors ||
        err.message ||
        'Failed to create curve';
      
      showError(
        typeof errorMessage === 'object'
          ? JSON.stringify(errorMessage, null, 2)
          : errorMessage
      );
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = async (curveData: CurveRequest) => {
    if (!editingCurve) return;
    
    setLoading(true);

    try {
      const updateRequest: UpdateCurveRequest = {
        instruments: curveData.instruments,
      };
      
      const response = await updateCurve(editingCurve.id, updateRequest);
      showSuccess(
        `Curve "${response.name}" updated successfully with ${response.instruments.length} instruments!`
      );
      setShowEditForm(false);
      setEditingCurve(null);
      setActiveTab(1);
      await loadCurveNames();
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message ||
        err.response?.data?.errors ||
        err.message ||
        'Failed to update curve';
      
      showError(
        typeof errorMessage === 'object'
          ? JSON.stringify(errorMessage, null, 2)
          : errorMessage
      );
    } finally {
      setLoading(false);
    }
  };

  const handleEditClick = async (curveName: string, date: string) => {
    setLoading(true);
    try {
      const dateOnly = new Date(date).toISOString().split('T')[0];
      const fullCurve = await getCurveByNameAndDate(curveName, dateOnly);
      setEditingCurve(fullCurve);
      setShowEditForm(true);
      setActiveTab(2);
    } catch (err: any) {
      showError(err.response?.data?.message || err.message || 'Failed to load curve for editing');
    } finally {
      setLoading(false);
    }
  };

  const handleCancelEdit = () => {
    setShowEditForm(false);
    setEditingCurve(null);
    setActiveTab(1);
  };

  const handleDeleteClick = (name: string, date: string) => {
    setCurveToDelete({ name, date });
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (!curveToDelete) return;
    
    setDeleteDialogOpen(false);
    setLoading(true);

    try {
      const date = new Date(curveToDelete.date).toISOString().split('T')[0];
      await deleteCurve(curveToDelete.name, date);
      showSuccess(`Curve "${curveToDelete.name}" for date ${date} deleted successfully!`);
      await loadCurveNames();
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message ||
        err.message ||
        'Failed to delete curve';
      
      showError(errorMessage);
    } finally {
      setLoading(false);
      setCurveToDelete(null);
    }
  };

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  };

  return (
    <Container maxWidth="lg">
      <Box sx={{ mt: 4, mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Define Curves
        </Typography>

        <Typography variant="body1" color="text.secondary" gutterBottom>
          Manage market data curves with term structures (instruments).
        </Typography>

        <Paper elevation={3} sx={{ mt: 3 }}>
          <Tabs value={activeTab} onChange={handleTabChange} sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tab label="Create New Curve" />
            <Tab label="Manage Curves" />
            {showEditForm && <Tab label="Edit Curve" />}
          </Tabs>

          {activeTab === 0 && (
            <CreateCurveTab loading={loading} onSubmit={handleCreate} />
          )}

          {activeTab === 1 && (
            <ManageCurvesTab
              loading={loadingCurves}
              curveGroups={curveGroups}
              onDateChange={handleDateChange}
              onEditClick={handleEditClick}
              onDeleteClick={handleDeleteClick}
            />
          )}

          {activeTab === 2 && showEditForm && editingCurve && (
            <EditCurveTab
              loading={loading}
              curve={editingCurve}
              onSubmit={handleEdit}
              onCancel={handleCancelEdit}
            />
          )}
        </Paper>
      </Box>

      <DeleteCurveDialog
        open={deleteDialogOpen}
        curveName={curveToDelete?.name || null}
        curveDate={curveToDelete?.date || null}
        onConfirm={handleDeleteConfirm}
        onCancel={() => setDeleteDialogOpen(false)}
      />

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
    </Container>
  );
};

export default DefineCurves;
