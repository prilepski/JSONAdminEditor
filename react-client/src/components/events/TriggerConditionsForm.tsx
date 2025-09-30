import React from 'react';

interface TriggerConditionsFormProps {
  triggerConditions: Record<string, boolean>;
  onUpdate: (conditions: Record<string, boolean>) => void;
}

const TRIGGER_CONDITIONS = [
  'IsSchedulable',
  'IsOpen', 
  'IsScheduled',
  'IsCompleted',
  'IsReadyForScheduling'
];

export const TriggerConditionsForm: React.FC<TriggerConditionsFormProps> = ({
  triggerConditions,
  onUpdate,
}) => {
  const addCondition = () => {
    const newKey = `new_condition_${Date.now()}`;
    onUpdate({ ...triggerConditions, [newKey]: false });
  };

  const updateConditionName = (oldKey: string, newKey: string) => {
    if (oldKey === newKey) return;
    const updated = { ...triggerConditions };
    const value = updated[oldKey];
    delete updated[oldKey];
    updated[newKey] = value;
    onUpdate(updated);
  };

  const updateConditionValue = (key: string, value: boolean) => {
    onUpdate({ ...triggerConditions, [key]: value });
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
              <th>Value</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {Object.entries(triggerConditions).map(([key, value]) => (
              <tr key={key}>
                <td>
                  <select
                    className="form-select"
                    value={TRIGGER_CONDITIONS.includes(key) ? key : ''}
                    onChange={(e) => updateConditionName(key, e.target.value)}
                  >
                    <option value="">Select condition...</option>
                    {TRIGGER_CONDITIONS.filter(condition => 
                      condition === key || !Object.keys(triggerConditions).includes(condition)
                    ).map(condition => (
                      <option key={condition} value={condition}>{condition}</option>
                    ))}
                  </select>
                </td>
                <td>
                  <div className="form-check form-switch">
                    <input
                      className="form-check-input"
                      type="checkbox"
                      checked={value}
                      onChange={(e) => updateConditionValue(key, e.target.checked)}
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