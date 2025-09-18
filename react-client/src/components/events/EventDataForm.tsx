import React from 'react';

interface EventDataFormProps {
  eventData: {
    OrderType?: string;
    Phone?: string;
    Email?: string;
    Logo?: string;
    IsSuppressed?: boolean;
  };
  onUpdate: (field: string, value: any) => void;
}

export const EventDataForm: React.FC<EventDataFormProps> = ({ eventData, onUpdate }) => (
  <div>
    <div className="row mb-3">
      <div className="col-md-6">
        <label className="form-label">Order Type</label>
        <input
          type="text"
          className="form-control"
          value={eventData.OrderType || ''}
          onChange={(e) => onUpdate('OrderType', e.target.value)}
        />
      </div>
      <div className="col-md-6">
        <label className="form-label">Phone</label>
        <input
          type="text"
          className="form-control"
          value={eventData.Phone || ''}
          onChange={(e) => onUpdate('Phone', e.target.value)}
        />
      </div>
    </div>
    <div className="row mb-3">
      <div className="col-md-6">
        <label className="form-label">Email</label>
        <input
          type="text"
          className="form-control"
          value={eventData.Email || ''}
          onChange={(e) => onUpdate('Email', e.target.value)}
        />
      </div>
      <div className="col-md-6">
        <label className="form-label">Logo</label>
        <input
          type="text"
          className="form-control"
          value={eventData.Logo || ''}
          onChange={(e) => onUpdate('Logo', e.target.value)}
        />
      </div>
    </div>
    <div className="mb-3">
      <div className="form-check">
        <input
          type="checkbox"
          className="form-check-input"
          checked={eventData.IsSuppressed || false}
          onChange={(e) => onUpdate('IsSuppressed', e.target.checked)}
        />
        <label className="form-check-label">Is Suppressed</label>
      </div>
    </div>
  </div>
);