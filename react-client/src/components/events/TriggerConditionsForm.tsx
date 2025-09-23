import React from 'react';

interface TriggerConditionsFormProps {
  triggerConditions: Record<string, boolean>;
  onUpdate: (conditions: Record<string, boolean>) => void;
}

export const TriggerConditionsForm: React.FC<TriggerConditionsFormProps> = ({
  triggerConditions,
  onUpdate,
}) => {
  const addCondition = () => {
    const newKey = `condition_${Date.now()}`;
    onUpdate({ ...triggerConditions, [newKey]: false });
  };

  const updateCondition = (oldKey: string, newKey: string, value: boolean) => {
    const updated = { ...triggerConditions };
    if (oldKey !== newKey) {
      delete updated[oldKey];
    }
    updated[newKey] = value;
    onUpdate(updated);
  };

  const removeCondition = (key: string) => {
    const updated = { ...triggerConditions };
    delete updated[key];
    onUpdate(updated);
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h5>Trigger Conditions</h5>
        <button className="btn btn-success btn-sm" onClick={addCondition}>
          <i className="fas fa-plus me-1"></i>Add Condition
        </button>
      </div>
      <div className="table-responsive">
        <table className="table table-bordered">
          <thead className="table-light">
            <tr>
              <th>Condition Name</th>
              <th>Enabled</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {Object.entries(triggerConditions).map(([key, value]) => (
              <tr key={key}>
                <td>
                  <input
                    type="text"
                    className="form-control"
                    value={key}
                    onChange={(e) => updateCondition(key, e.target.value, value)}
                  />
                </td>
                <td>
                  <div className="form-check form-switch">
                    <input
                      className="form-check-input"
                      type="checkbox"
                      checked={value}
                      onChange={(e) => updateCondition(key, key, e.target.checked)}
                    />
                    <label className="form-check-label">
                      {value ? 'Yes' : 'No'}
                    </label>
                  </div>
                </td>
                <td>
                  <button
                    className="btn btn-danger btn-sm"
                    onClick={() => removeCondition(key)}
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