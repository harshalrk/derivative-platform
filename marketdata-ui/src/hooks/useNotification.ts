import { useState, useCallback } from 'react';

export type NotificationType = 'success' | 'error' | 'info' | 'warning';

export interface Notification {
  open: boolean;
  message: string;
  type: NotificationType;
}

export const useNotification = () => {
  const [notification, setNotification] = useState<Notification>({
    open: false,
    message: '',
    type: 'info',
  });

  const showSuccess = useCallback((message: string) => {
    setNotification({ open: true, message, type: 'success' });
  }, []);

  const showError = useCallback((message: string) => {
    setNotification({ open: true, message, type: 'error' });
  }, []);

  const showInfo = useCallback((message: string) => {
    setNotification({ open: true, message, type: 'info' });
  }, []);

  const showWarning = useCallback((message: string) => {
    setNotification({ open: true, message, type: 'warning' });
  }, []);

  const hideNotification = useCallback(() => {
    setNotification((prev) => ({ ...prev, open: false }));
  }, []);

  return {
    notification,
    showSuccess,
    showError,
    showInfo,
    showWarning,
    hideNotification,
  };
};
