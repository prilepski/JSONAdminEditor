import React from 'react';

interface SaveButtonProps {
  onClick: () => void;
  loading?: boolean;
  disabled?: boolean;
  text?: string;
  loadingText?: string;
}

export const SaveButton: React.FC<SaveButtonProps> = ({
  onClick,
  loading = false,
  disabled = false,
  text = 'Save Changes',
  loadingText = 'Saving...',
}) => (
  <button
    type="button"
    className="btn btn-primary"
    onClick={loading ? undefined : onClick}
    disabled={loading || disabled}
  >
    {loading ? (
      <>
        <span className="spinner-border spinner-border-sm me-1"></span>
        {loadingText}
      </>
    ) : (
      <>
        <i className="fas fa-save me-1"></i>
        {text}
      </>
    )}
  </button>
);
