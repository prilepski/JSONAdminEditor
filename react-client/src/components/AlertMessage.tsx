import React, { useEffect } from 'react';

interface AlertMessageProps {
  message: string;
  type: 'success' | 'error' | 'warning';
  onClose: () => void;
  autoClose?: boolean;
  duration?: number;
}

export const AlertMessage: React.FC<AlertMessageProps> = ({
  message,
  type,
  onClose,
  autoClose = true,
  duration = 5000,
}) => {
  useEffect(() => {
    if (autoClose) {
      const timer = setTimeout(onClose, duration);
      return () => clearTimeout(timer);
    }
  }, [autoClose, duration, onClose]);

  const alertClass = type === 'success' ? 'alert-success' : type === 'error' ? 'alert-danger' : 'alert-warning';
  const iconClass = type === 'success' ? 'fa-check-circle' : type === 'error' ? 'fa-exclamation-triangle' : 'fa-exclamation-triangle';

  return (
    <div className={`alert ${alertClass} alert-dismissible fade show`} role="alert">
      <span>
        <i className={`fas ${iconClass} me-2`}></i>
        {message}
      </span>
      <button
        type="button"
        className="btn-close"
        onClick={onClose}
        aria-label="Close"
      ></button>
    </div>
  );
};