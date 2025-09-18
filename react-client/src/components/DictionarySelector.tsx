import React from 'react';
import { FileType } from '../types';

interface DictionarySelectorProps {
  selectedFileType: FileType;
  onFileTypeChange: (fileType: FileType) => void;
}

const dictionaryOptions = [
  { value: FileType.None, label: 'Select dictionary type...' },
  { value: FileType.Templates, label: 'Notification Templates' },
  { value: FileType.EventTriggers, label: 'Event Triggers' },
  { value: FileType.EventChannels, label: 'Event Channels' },
  { value: FileType.OrderTypes, label: 'Order Types' },
  { value: FileType.Customers, label: 'Customers' },
];

export const DictionarySelector: React.FC<DictionarySelectorProps> = ({
  selectedFileType,
  onFileTypeChange,
}) => {
  return (
    <div className="card mb-4">
      <div className="card-header">
        <h3>
          <i className="fas fa-cog me-2"></i>Select a Dictionary
        </h3>
        <p className="mb-0 text-muted">
          Manage system dictionaries that provide reference data for notifications, events, and
          customer configurations.
        </p>
      </div>
      <div className="card-body">
        <div className="mb-3">
          <select
            className="form-select"
            value={selectedFileType}
            onChange={(e) => onFileTypeChange(Number(e.target.value) as FileType)}
            required
          >
            {dictionaryOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </div>
      </div>
    </div>
  );
};
