import React from 'react';

interface ToggleButtonGroupProps {
  options: { value: string; label: string }[];
  selected: string;
  onChange: (value: string) => void;
  size?: 'sm' | 'md' | 'lg';
}

export const ToggleButtonGroup: React.FC<ToggleButtonGroupProps> = ({
  options,
  selected,
  onChange,
  size = 'md'
}) => (
  <div className={`btn-group btn-group-${size}`} role="group">
    {options.map(({ value, label }) => (
      <button
        key={value}
        type="button"
        className={`btn ${selected === value ? 'btn-primary' : 'btn-outline-primary'}`}
        onClick={() => onChange(value)}
      >
        {label}
      </button>
    ))}
  </div>
);