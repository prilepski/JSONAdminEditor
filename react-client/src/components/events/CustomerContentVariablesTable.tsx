import React from 'react';
import { ContentVariable } from '../../types/customerEvent';

interface CustomerContentVariablesTableProps {
  contentVariables: Record<string, ContentVariable>;
  onAdd: () => void;
  onUpdate: (oldKey: string, newKey: string, value: string) => void;
  onRemove: (key: string) => void;
}

export const CustomerContentVariablesTable: React.FC<CustomerContentVariablesTableProps> = ({
  contentVariables,
  onAdd,
  onUpdate,
  onRemove
}) => (
  <div>
    <div className="d-flex justify-content-between align-items-center mb-3">
      <h5><i className="fas fa-code me-2"></i>Content Variables</h5>
      <button
        type="button"
        className="btn btn-outline-primary"
        onClick={onAdd}
      >
        <i className="fas fa-plus me-1"></i>Add Variable
      </button>
    </div>
    <div className="table-responsive">
      <table className="table table-bordered">
        <thead className="table-light">
          <tr>
            <th>Variable Name</th>
            <th>Value</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {Object.entries(contentVariables).map(([key, data]) => (
            <tr key={key}>
              <td>
                <input
                  type="text"
                  className="form-control"
                  value={key}
                  onChange={(e) => onUpdate(key, e.target.value, data.value)}
                />
              </td>
              <td>
                <input
                  type="text"
                  className="form-control"
                  value={data.value}
                  onChange={(e) => onUpdate(key, key, e.target.value)}
                />
              </td>
              <td className="text-center">
                <button
                  type="button"
                  className="btn btn-outline-danger btn-sm"
                  onClick={() => onRemove(key)}
                >
                  <i className="fas fa-trash"></i>
                </button>
              </td>
            </tr>
          ))}
          {Object.keys(contentVariables).length === 0 && (
            <tr>
              <td colSpan={3} className="text-center text-muted py-4">
                No content variables. Click "Add Variable" to create one.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  </div>
);