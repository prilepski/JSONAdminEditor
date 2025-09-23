import React from 'react';
import { FileType } from '../types';

interface DictionarySelectorProps {
  selectedFileType: FileType;
  onFileTypeChange: (fileType: FileType) => void;
}

const dictionaryOptions = [
  { value: FileType.None, label: 'Select dictionary type...', description: '' },
  { value: FileType.Templates, label: 'Notification Templates', description: 'Email, SMS, and Voice templates for notifications' },
  { value: FileType.EventTriggers, label: 'Event Triggers', description: 'Events that trigger notifications (Ready For Scheduling, Next Stop Update, etc.)' },
  { value: FileType.EventChannels, label: 'Event Channels', description: 'Communication channels (Email, SMS, Voice)' },
  { value: FileType.OrderTypes, label: 'Order Types', description: 'Types of orders (Delivery, Pickup, ALL)' },
  { value: FileType.Customers, label: 'Customers', description: 'Customer information and contact details' },
  { value: FileType.LogoUrls, label: 'Logo URLs', description: 'Logo file names and URLs for branding' },
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
          {selectedFileType !== FileType.None && (
            <div className="form-text">
              <i className="fas fa-info-circle me-1"></i>
              {dictionaryOptions.find(opt => opt.value === selectedFileType)?.description}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
