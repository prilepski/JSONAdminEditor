import React from 'react';

interface ContentVariablesTableProps {
  contentVariables: Record<string, string>;
  globalContentVariables?: Record<string, string>;
  eventContentVariables?: Record<string, string>;
  onUpdate: (variables: Record<string, string>) => void;
  onRedefinedStatesChange?: (states: Record<string, boolean>) => void;
  title?: string;
  showAddButton?: boolean;
}

export const ContentVariablesTable: React.FC<ContentVariablesTableProps> = ({
  contentVariables,
  globalContentVariables = {},
  eventContentVariables = {},
  onUpdate,
  onRedefinedStatesChange,
  title = "Content Variables",
  showAddButton = true,
}) => {
  const [redefinedStates, setRedefinedStates] = React.useState<Record<string, boolean>>({});
  const [editingKeys, setEditingKeys] = React.useState<Record<string, string>>({});

  // Initialize redefined states based on contentVariables
  React.useEffect(() => {
    const states: Record<string, boolean> = {};
    Object.keys(contentVariables).forEach(key => {
      states[key] = true;
    });
    setRedefinedStates(states);
  }, [JSON.stringify(contentVariables)]);

  const mergedVariables = React.useMemo(() => {
    const merged: Record<string, { value: string; isRedefined: boolean; globalValue?: string; eventValue?: string }> = {};
    
    // Add global variables
    Object.entries(globalContentVariables).forEach(([key, value]) => {
      merged[key] = { value, isRedefined: redefinedStates[key] ?? false, globalValue: value };
    });
    
    // Add event variables
    Object.entries(eventContentVariables).forEach(([key, value]) => {
      merged[key] = {
        value,
        isRedefined: redefinedStates[key] ?? false,
        globalValue: merged[key]?.globalValue,
        eventValue: value
      };
    });
    
    // Add content variables
    Object.entries(contentVariables).forEach(([key, value]) => {
      const isRedefined = redefinedStates[key] ?? true;
      merged[key] = {
        value,
        isRedefined,
        globalValue: merged[key]?.globalValue,
        eventValue: merged[key]?.eventValue
      };
    });
    
    return merged;
  }, [contentVariables, globalContentVariables, eventContentVariables, redefinedStates]);

  const getRedefinedVariables = (): Record<string, string> => {
    const redefined: Record<string, string> = {};
    Object.entries(contentVariables).forEach(([key, value]) => {
      if (redefinedStates[key]) {
        redefined[key] = value;
      }
    });
    return redefined;
  };

  const updateVariable = (oldKey: string, newKey: string, value: string) => {
    const newVars = { ...contentVariables };
    if (oldKey !== newKey) {
      delete newVars[oldKey];
      setRedefinedStates(prev => {
        const newStates = { ...prev };
        delete newStates[oldKey];
        newStates[newKey] = prev[oldKey] ?? true;
        return newStates;
      });
    }
    newVars[newKey] = value;
    onUpdate(newVars);
  };

  const toggleRedefined = (key: string, isRedefined: boolean) => {
    const newStates = { ...redefinedStates, [key]: isRedefined };
    setRedefinedStates(newStates);
    onRedefinedStatesChange?.(newStates);
    if (isRedefined && !contentVariables[key]) {
      const defaultValue = globalContentVariables[key] || eventContentVariables[key] || '';
      const newVars = { ...contentVariables, [key]: defaultValue };
      onUpdate(newVars);
    }
  };

  const removeVariable = (key: string) => {
    const newVars = { ...contentVariables };
    delete newVars[key];
    setRedefinedStates(prev => {
      const { [key]: _, ...rest } = prev;
      return rest;
    });
    onUpdate(newVars);
  };

  const addVariable = () => {
    const newKey = `new_var_${Date.now()}`;
    const newVars = { ...contentVariables, [newKey]: '' };
    const newStates = { ...redefinedStates, [newKey]: true };
    setRedefinedStates(newStates);
    onRedefinedStatesChange?.(newStates);
    onUpdate(newVars);
  };

  // Notify parent of redefined states changes
  React.useEffect(() => {
    onRedefinedStatesChange?.(redefinedStates);
  }, [redefinedStates, onRedefinedStatesChange]);

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h5>
          <i className="fas fa-code me-2"></i>{title}
        </h5>
        {showAddButton && (
          <button type="button" className="btn btn-success" onClick={addVariable}>
            <i className="fas fa-plus me-1"></i>Add Variable
          </button>
        )}
      </div>
      
      {Object.keys(mergedVariables).length > 0 ? (
        <div className="table-responsive">
          <table className="table table-striped table-hover">
            <thead className="table-dark">
              <tr>
                <th style={{ width: '30%' }}>Variable Name</th>
                <th style={{ width: '40%' }}>Value</th>
                <th style={{ width: '15%' }}>Is Redefined</th>
                <th style={{ width: '15%' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {Object.entries(mergedVariables).map(([key, data]) => (
                <tr key={key}>
                  <td>
                    <input
                      type="text"
                      className={`form-control form-control-sm ${data.globalValue || data.eventValue ? 'bg-light' : ''}`}
                      value={editingKeys[key] !== undefined ? editingKeys[key] : key}
                      readOnly={!!(data.globalValue || data.eventValue)}
                      onChange={data.globalValue || data.eventValue ? undefined : (e) => 
                        setEditingKeys(prev => ({ ...prev, [key]: e.target.value }))
                      }
                      onBlur={() => {
                        if (editingKeys[key] !== undefined && editingKeys[key] !== key) {
                          updateVariable(key, editingKeys[key], data.value);
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
                      className="form-control form-control-sm"
                      value={data.value}
                      onChange={(e) => updateVariable(key, key, e.target.value)}
                      disabled={!data.isRedefined}
                    />
                  </td>
                  <td className="text-center">
                    <div className="form-check">
                      <input
                        className="form-check-input"
                        type="checkbox"
                        checked={data.isRedefined}
                        onChange={(e) => toggleRedefined(key, e.target.checked)}
                      />
                    </div>
                  </td>
                  <td className="text-center">
                    {!data.globalValue && !data.eventValue && (
                      <button
                        type="button"
                        className="btn btn-danger btn-sm"
                        onClick={() => removeVariable(key)}
                      >
                        <i className="fas fa-trash"></i>
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <div className="text-center py-4">
          <i className="fas fa-code fa-3x text-muted mb-3"></i>
          <h5 className="text-muted">No Content Variables</h5>
          <p className="text-muted">
            No content variables found. {showAddButton && 'Click "Add Variable" to create one.'}
          </p>
        </div>
      )}
    </div>
  );
};

// Utility function to get redefined variables
export const getRedefinedVariables = (contentVariables: Record<string, string>, redefinedStates: Record<string, boolean>): Record<string, string> => {
  const redefined: Record<string, string> = {};
  Object.entries(contentVariables).forEach(([key, value]) => {
    if (redefinedStates[key]) {
      redefined[key] = value;
    }
  });
  return redefined;
};