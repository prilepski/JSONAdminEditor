import React from 'react';

interface ContentVariablesFormProps {
  contentVariables: Record<string, string>;
  globalContentVariables?: Record<string, string>;
  onUpdate: (variables: Record<string, string>) => void;
}

export const ContentVariablesForm: React.FC<ContentVariablesFormProps> = ({
  contentVariables,
  globalContentVariables = {},
  onUpdate,
}) => {
  // Merge global and event-level variables following backend logic
  const mergedVariables = React.useMemo(() => {
    const merged: Record<string, { value: string; isRedefined: boolean; globalValue?: string }> = {};
    
    // Start with global variables
    Object.entries(globalContentVariables).forEach(([key, value]) => {
      merged[key] = {
        value,
        isRedefined: false,
        globalValue: value
      };
    });
    
    // Override with event-level variables (these are considered redefined)
    Object.entries(contentVariables).forEach(([key, value]) => {
      merged[key] = {
        value,
        isRedefined: true,
        globalValue: merged[key]?.globalValue
      };
    });
    
    return merged;
  }, [contentVariables, globalContentVariables]);
  const updateVariable = (oldKey: string, newKey: string, value: string) => {
    const newVars = { ...contentVariables };
    if (oldKey !== newKey) {
      delete newVars[oldKey];
    }
    newVars[newKey] = value;
    onUpdate(newVars);
  };
  
  const toggleRedefined = (key: string, isRedefined: boolean) => {
    const newVars = { ...contentVariables };
    if (isRedefined) {
      // Add to event variables with current value
      newVars[key] = mergedVariables[key]?.value || globalContentVariables[key] || '';
    } else {
      // Remove from event variables (use global value)
      delete newVars[key];
    }
    onUpdate(newVars);
  };

  const removeVariable = (key: string) => {
    const newVars = { ...contentVariables };
    delete newVars[key];
    onUpdate(newVars);
  };

  const addVariable = () => {
    onUpdate({ ...contentVariables, '': '' });
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h5>Content Variables</h5>
        <button type="button" className="btn btn-success" onClick={addVariable}>
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
            {Object.entries(mergedVariables).map(([key, data], index) => (
              <tr key={`content-var-${index}`}>
                <td>
                  <input
                    type="text"
                    className={`form-control form-control-sm ${data.globalValue ? 'bg-light' : ''}`}
                    value={key}
                    readOnly={!!data.globalValue}
                    onChange={data.globalValue ? undefined : (e) => updateVariable(key, e.target.value, data.value)}
                    placeholder="Variable name"
                  />
                </td>
                <td>
                  <input
                    type="text"
                    className="form-control form-control-sm"
                    value={data.value}
                    onChange={(e) => updateVariable(key, key, e.target.value)}
                    disabled={!data.isRedefined}
                    placeholder="Variable value"
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
                <td>
                  <button
                    type="button"
                    className="btn btn-danger btn-sm"
                    onClick={() => removeVariable(key)}
                  >
                    <i className="fas fa-trash"></i>
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};
