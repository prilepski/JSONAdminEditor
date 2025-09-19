import React from 'react';

interface ToggleButtonProps {
  checked: boolean;
  onChange: (checked: boolean) => void;
  label: string;
  variant?: 'primary' | 'success' | 'warning' | 'danger';
  size?: 'sm' | 'md' | 'lg';
}

export const ToggleButton: React.FC<ToggleButtonProps> = ({
  checked,
  onChange,
  label,
  variant = 'primary',
  size = 'md'
}) => (
  <button
    type="button"
    className={`btn btn${checked ? '-' : '-outline-'}${variant} btn-${size}`}
    onClick={() => onChange(!checked)}
  >
    <i className={`fas fa-${checked ? 'check' : 'times'} me-2`}></i>
    {label}
  </button>
);