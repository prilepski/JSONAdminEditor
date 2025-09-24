import React from 'react';

interface ContentVariablesFormProps {
  contentVariables: Record<string, string>;
  onUpdate: (variables: Record<string, string>) => void;
}

export const ContentVariablesForm: React.FC<ContentVariablesFormProps> = ({
  contentVariables,
  onUpdate,
}) => {
  const updateVariable = (oldKey: string, newKey: string, value: string) => {
    const newVars = { ...contentVariables };
    if (oldKey !== newKey) {
      delete newVars[oldKey];
    }
    newVars[newKey] = value;
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
            {Object.entries(contentVariables).map(([key, value]) => (
              <tr key={key}>
                <td>
                  <input
                    type="text"
                    className="form-control form-control-sm"
                    value={key}
                    onChange={(e) => updateVariable(key, e.target.value, value)}
                    placeholder="Variable name"
                  />
                </td>
                <td>
                  <input
                    type="text"
                    className="form-control form-control-sm"
                    value={value}
                    onChange={(e) => updateVariable(key, key, e.target.value)}
                    placeholder="Variable value"
                  />
                </td>
                <td className="text-center">
                  <div className="form-check">
                    <input
                      className="form-check-input"
                      type="checkbox"
                      checked={true}
                      readOnly
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
