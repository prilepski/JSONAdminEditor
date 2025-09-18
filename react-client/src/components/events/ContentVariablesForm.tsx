import React from 'react';

interface ContentVariablesFormProps {
  contentVariables: Record<string, string>;
  onUpdate: (variables: Record<string, string>) => void;
}

export const ContentVariablesForm: React.FC<ContentVariablesFormProps> = ({
  contentVariables,
  onUpdate
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
      <p className="text-muted mb-3">Content variables for this event:</p>
      {Object.entries(contentVariables).map(([key, value]) => (
        <div key={key} className="row mb-2">
          <div className="col-md-4">
            <input
              type="text"
              className="form-control form-control-sm"
              value={key}
              onChange={(e) => updateVariable(key, e.target.value, value)}
              placeholder="Variable name"
            />
          </div>
          <div className="col-md-6">
            <input
              type="text"
              className="form-control form-control-sm"
              value={value}
              onChange={(e) => updateVariable(key, key, e.target.value)}
              placeholder="Variable value"
            />
          </div>
          <div className="col-md-2">
            <button
              type="button"
              className="btn btn-danger btn-sm"
              onClick={() => removeVariable(key)}
            >
              <i className="fas fa-trash"></i>
            </button>
          </div>
        </div>
      ))}
      <button
        type="button"
        className="btn btn-success btn-sm"
        onClick={addVariable}
      >
        <i className="fas fa-plus me-1"></i>Add Variable
      </button>
    </div>
  );
};