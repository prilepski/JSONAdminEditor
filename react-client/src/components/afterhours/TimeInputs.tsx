import React from 'react';

interface TimeInputsProps {
  startTime: string;
  endTime: string;
  onTimeChange: (field: 'start' | 'end', value: string) => void;
}

export const TimeInputs: React.FC<TimeInputsProps> = ({ startTime, endTime, onTimeChange }) => (
  <div className="row mb-4">
    <div className="col-md-6">
      <label className="form-label">Start Time</label>
      <input
        type="time"
        className="form-control"
        value={startTime}
        onChange={(e) => onTimeChange('start', e.target.value)}
      />
    </div>
    <div className="col-md-6">
      <label className="form-label">End Time</label>
      <input
        type="time"
        className="form-control"
        value={endTime}
        onChange={(e) => onTimeChange('end', e.target.value)}
      />
    </div>
  </div>
);