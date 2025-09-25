import React from 'react';
import { ContentVariable } from '../../types/components';
import { useContentVariablesQuery } from '../../hooks/useContentVariableQuery';
import { useEventQuery } from '../../hooks/useEventQuery';

interface CustomerContentVariablesTableProps {
  contentVariables: Record<string, ContentVariable>;
  selectedEvent: string;
  selectedOrderType: string;
  onAdd: () => void;
  onUpdate: (oldKey: string, newKey: string, value: string) => void;
  onRemove: (key: string) => void;
  onToggleRedefined: (key: string, isRedefined: boolean) => void;
}

export const CustomerContentVariablesTable: React.FC<CustomerContentVariablesTableProps> = ({
  contentVariables,
  selectedEvent,
  selectedOrderType,
  onAdd,
  onUpdate,
  onRemove,
  onToggleRedefined,
}) => {
  const [editingKeys, setEditingKeys] = React.useState<Record<string, string>>({});
  const { data: globalContentVariables = {} } = useContentVariablesQuery();
  const { data: eventData } = useEventQuery(selectedEvent, selectedOrderType);
  
  const mergedVariables = React.useMemo(() => {
    const merged: Record<string, { value: string; isRedefined: boolean; globalValue?: string; eventValue?: string }> = {};
    
    Object.entries(globalContentVariables).forEach(([key, value]) => {
      merged[key] = { value, isRedefined: false, globalValue: value };
    });
    
    if (eventData?.contentVariables) {
      Object.entries(eventData.contentVariables).forEach(([key, value]) => {
        merged[key] = {
          value: String(value),
          isRedefined: false,
          globalValue: merged[key]?.globalValue,
          eventValue: String(value)
        };
      });
    }
    
    Object.entries(contentVariables).forEach(([key, data]) => {
      merged[key] = {
        value: data.value,
        isRedefined: true,
        globalValue: merged[key]?.globalValue,
        eventValue: merged[key]?.eventValue
      };
    });
    
    return merged;
  }, [globalContentVariables, eventData?.contentVariables, contentVariables]);
  
  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h5>
          <i className="fas fa-code me-2"></i>Content Variables
        </h5>
        <button type="button" className="btn btn-success" onClick={onAdd}>
          <i className="fas fa-plus me-1"></i>Add Variable
        </button>
      </div>
      <div className="table-responsive">
        <table className="table table-striped table-hover">
          <thead className="table-dark">
            <tr>
              <th>Variable Name</th>
              <th>Variable Value</th>
              <th>Is Redefined</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {Object.entries(mergedVariables).map(([key, data]) => (
              <tr key={key}>
                <td>
                  <input
                    type="text"
                    className={`form-control ${data.globalValue || data.eventValue ? 'bg-light' : ''}`}
                    value={editingKeys[key] !== undefined ? editingKeys[key] : key}
                    readOnly={!!(data.globalValue || data.eventValue)}
                    onChange={data.globalValue || data.eventValue ? undefined : (e) => 
                      setEditingKeys(prev => ({ ...prev, [key]: e.target.value }))
                    }
                    onBlur={() => {
                      if (editingKeys[key] !== undefined && editingKeys[key] !== key) {
                        onUpdate(key, editingKeys[key], data.value);
                      }
                      setEditingKeys(prev => {
                        const { [key]: _, ...rest } = prev;
                        return rest;
                      });
                    }}
                  />
                </td>
                <td>
                  <input
                    type="text"
                    className="form-control"
                    value={data.value}
                    disabled={!data.isRedefined}
                    onChange={(e) => onUpdate(key, key, e.target.value)}
                  />
                </td>
                <td className="text-center">
                  <div className="form-check">
                    <input
                      className="form-check-input"
                      type="checkbox"
                      checked={data.isRedefined}
                      onChange={(e) => onToggleRedefined(key, e.target.checked)}
                    />
                  </div>
                </td>
                <td className="text-center">
                  <button
                    type="button"
                    className="btn btn-danger btn-sm"
                    onClick={() => onRemove(key)}
                  >
                    <i className="fas fa-trash"></i>
                  </button>
                </td>
              </tr>
            ))}
            {Object.keys(mergedVariables).length === 0 && (
              <tr>
                <td colSpan={4} className="text-center text-muted py-4">
                  No content variables. Click "Add Variable" to create one.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};