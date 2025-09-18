import React from 'react';

interface SaveButtonProps {
  onClick: () => void;
  loading?: boolean;
  disabled?: boolean;
  text?: string;
}

export const SaveButton: React.FC<SaveButtonProps> = ({ 
  onClick, 
  loading = false, 
  disabled = false, 
  text = "Save Changes" 
}) => (
  <button
    type="button"
    className="btn btn-primary"
    onClick={onClick}
    disabled={loading || disabled}
  >
    {loading ? (
      <>
        <span className="spinner-border spinner-border-sm me-1"></span>
        Saving...
      </>
    ) : (
      <>
        <i className="fas fa-save me-1"></i>
        {text}
      </>
    )}
  </button>
);