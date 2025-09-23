import React from 'react';

interface EventDataFormProps {
  eventData: {
    orderType?: string;
    phone?: string;
    email?: string;
    isSuppressed?: boolean;
  };
  orderTypes: { name?: string }[];
  onUpdate: (field: string, value: any) => void;
}

export const EventDataForm: React.FC<EventDataFormProps> = ({ eventData, orderTypes, onUpdate }) => (
  <div className="table-responsive">
    <table className="table table-bordered">
      <thead className="table-light">
        <tr>
          <th style={{ width: '200px' }}>Field</th>
          <th>Value</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td><strong>Order Type</strong></td>
          <td>
            <select
              className="form-select"
              value={eventData.orderType || ''}
              onChange={(e) => onUpdate('orderType', e.target.value)}
            >
              <option value="">Select Order Type</option>
              {orderTypes
                .filter(orderType => orderType.name)
                .map(orderType => (
                  <option key={orderType.name} value={orderType.name}>
                    {orderType.name}
                  </option>
                ))}
            </select>
          </td>
        </tr>
        <tr>
          <td><strong>Phone</strong></td>
          <td>
            <input
              type="text"
              className="form-control"
              value={eventData.phone || ''}
              onChange={(e) => onUpdate('phone', e.target.value)}
            />
          </td>
        </tr>
        <tr>
          <td><strong>Email</strong></td>
          <td>
            <input
              type="text"
              className="form-control"
              value={eventData.email || ''}
              onChange={(e) => onUpdate('email', e.target.value)}
            />
          </td>
        </tr>
        <tr>
          <td><strong>Suppressed</strong></td>
          <td>
            <div className="form-check form-switch">
              <input
                className="form-check-input"
                type="checkbox"
                checked={eventData.isSuppressed || false}
                onChange={(e) => onUpdate('isSuppressed', e.target.checked)}
              />
              <label className="form-check-label">
                {eventData.isSuppressed ? 'Yes' : 'No'}
              </label>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
);
