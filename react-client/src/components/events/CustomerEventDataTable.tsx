import React from 'react';
import { EventField } from '../../types/customerEvent';

interface CustomerEventDataTableProps {
  eventFields: EventField[];
  onUpdateField: (fieldName: string, value: string) => void;
  onToggleRedefined: (fieldName: string, isRedefined: boolean) => void;
}

export const CustomerEventDataTable: React.FC<CustomerEventDataTableProps> = ({
  eventFields,
  onUpdateField,
  onToggleRedefined
}) => (
  <div>
    <div className="alert alert-info">
      <i className="fas fa-info-circle me-2"></i>
      <strong>Event Data Management:</strong> Check "Is Redefined" to override global settings for this customer.
    </div>
    <div className="table-responsive">
      <table className="table table-bordered">
        <thead className="table-light">
          <tr>
            <th>Field</th>
            <th>Value</th>
            <th>Is Redefined</th>
            <th>Global Value</th>
          </tr>
        </thead>
        <tbody>
          {eventFields.map(field => (
            <tr key={field.name}>
              <td><strong>{field.name}</strong></td>
              <td>
                {field.type === 'checkbox' ? (
                  <div className="form-check">
                    <input
                      className="form-check-input"
                      type="checkbox"
                      checked={field.value === 'true'}
                      disabled={!field.isRedefined}
                      onChange={(e) => onUpdateField(field.name, String(e.target.checked))}
                    />
                  </div>
                ) : (
                  <input
                    type="text"
                    className="form-control"
                    value={field.value}
                    readOnly={!field.isRedefined}
                    onChange={(e) => onUpdateField(field.name, e.target.value)}
                  />
                )}
              </td>
              <td className="text-center">
                <div className="form-check">
                  <input
                    className="form-check-input"
                    type="checkbox"
                    checked={field.isRedefined}
                    onChange={(e) => onToggleRedefined(field.name, e.target.checked)}
                  />
                </div>
              </td>
              <td>
                <span className="text-muted">{field.globalValue || '—'}</span>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  </div>
);