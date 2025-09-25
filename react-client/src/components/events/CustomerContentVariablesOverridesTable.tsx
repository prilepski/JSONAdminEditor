import React from 'react';

interface CustomerContentVariablesOverridesTableProps {
  contentVariablesOverrides: Record<string, Record<string, Record<string, string>>>;
  onAdd: () => void;
  onUpdate: (eventKey: string, variableKey: string, overrideKey: string, value: string) => void;
  onUpdateKey: (oldEventKey: string, oldVariableKey: string, oldOverrideKey: string, newEventKey: string, newVariableKey: string, newOverrideKey: string) => void;
  onRemove: (eventKey: string, variableKey: string, overrideKey: string) => void;
}

export const CustomerContentVariablesOverridesTable: React.FC<CustomerContentVariablesOverridesTableProps> = ({
  contentVariablesOverrides,
  onAdd,
  onUpdate,
  onUpdateKey,
  onRemove,
}) => {
  const flattenedOverrides = React.useMemo(() => {
    const flattened: Array<{
      eventKey: string;
      variableKey: string;
      overrideKey: string;
      value: string;
      displayKey: string;
    }> = [];

    Object.entries(contentVariablesOverrides).forEach(([eventKey, variables]) => {
      Object.entries(variables).forEach(([variableKey, overrides]) => {
        Object.entries(overrides).forEach(([overrideKey, value]) => {
          flattened.push({
            eventKey,
            variableKey,
            overrideKey,
            value,
            displayKey: `${eventKey}.${variableKey}.${overrideKey}`,
          });
        });
      });
    });

    return flattened;
  }, [contentVariablesOverrides]);

  return (
    <div className="mt-3">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h5>
          <i className="fas fa-layer-group me-2"></i>Content Variables Overrides
        </h5>
        <button type="button" className="btn btn-success" onClick={onAdd}>
          <i className="fas fa-plus me-1"></i>Add Override
        </button>
      </div>

      {flattenedOverrides.length > 0 ? (
        <div className="table-responsive">
          <table className="table table-striped table-hover">
            <thead className="table-dark">
              <tr>
                <th style={{ width: '30%' }}>Event Key</th>
                <th style={{ width: '25%' }}>Variable Key</th>
                <th style={{ width: '20%' }}>Override Key</th>
                <th style={{ width: '20%' }}>Value</th>
                <th style={{ width: '5%' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {flattenedOverrides.map((override, index) => (
                <tr key={index}>
                  <td>
                    <input
                      type="text"
                      className="form-control form-control-sm"
                      defaultValue={override.eventKey}
                      onBlur={(e) => onUpdateKey(override.eventKey, override.variableKey, override.overrideKey, e.target.value, override.variableKey, override.overrideKey)}
                    />
                  </td>
                  <td>
                    <input
                      type="text"
                      className="form-control form-control-sm"
                      defaultValue={override.variableKey}
                      onBlur={(e) => onUpdateKey(override.eventKey, override.variableKey, override.overrideKey, override.eventKey, e.target.value, override.overrideKey)}
                    />
                  </td>
                  <td>
                    <input
                      type="text"
                      className="form-control form-control-sm"
                      defaultValue={override.overrideKey}
                      onBlur={(e) => onUpdateKey(override.eventKey, override.variableKey, override.overrideKey, override.eventKey, override.variableKey, e.target.value)}
                    />
                  </td>
                  <td>
                    <input
                      type="text"
                      className="form-control form-control-sm"
                      value={override.value}
                      onChange={(e) => onUpdate(override.eventKey, override.variableKey, override.overrideKey, e.target.value)}
                    />
                  </td>
                  <td className="text-center">
                    <button
                      type="button"
                      className="btn btn-danger btn-sm"
                      onClick={() => onRemove(override.eventKey, override.variableKey, override.overrideKey)}
                    >
                      <i className="fas fa-trash"></i>
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <div className="text-center py-4">
          <i className="fas fa-layer-group fa-3x text-muted mb-3"></i>
          <h5 className="text-muted">No Content Variables Overrides</h5>
          <p className="text-muted">
            No content variables overrides found. Click "Add Override" to start adding overrides.
          </p>
        </div>
      )}
    </div>
  );
};