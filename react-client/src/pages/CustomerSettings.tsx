import React, { useState, useEffect } from 'react';
import {
  useCustomersQuery,
  useCustomerContentVariablesQuery,
  useCustomerContentVariablesMutation,
} from '../hooks/useCustomerQuery';
import { useContentVariablesQuery } from '../hooks/useContentVariableQuery';
import { useErrorHandler } from '../hooks/useErrorHandler';
import {
  PageHeader,
  CustomerSelector,
  SaveButton,
  LoadingSpinner,
  TableSkeleton,
} from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

export const CustomerSettings: React.FC = () => {
  const [selectedCustomer, setSelectedCustomer] = useState<string>('');
  const [contentVariables, setContentVariables] = useState<Record<string, string>>({});
  const [editingKeys, setEditingKeys] = useState<Record<string, string>>({});
  const { handleError } = useErrorHandler({ context: 'CustomerSettings' });

  const { data: customers = [], isLoading: customersLoading } = useCustomersQuery();
  const { data: customerContentVars = {}, isLoading: settingsLoading } =
    useCustomerContentVariablesQuery(selectedCustomer);
  const { data: globalContentVariables = {} } = useContentVariablesQuery();
  const saveMutation = useCustomerContentVariablesMutation();

  const mergedVariables = React.useMemo(() => {
    const merged: Record<string, { value: string; isRedefined: boolean; globalValue?: string }> = {};
    
    Object.entries(globalContentVariables).forEach(([key, value]) => {
      merged[key] = { value, isRedefined: false, globalValue: value };
    });
    
    Object.entries(contentVariables).forEach(([key, value]) => {
      merged[key] = { value, isRedefined: true, globalValue: merged[key]?.globalValue };
    });
    
    return merged;
  }, [JSON.stringify(globalContentVariables), JSON.stringify(contentVariables)]);

  useEffect(() => {
    setContentVariables(selectedCustomer ? customerContentVars || {} : {});
  }, [selectedCustomer, JSON.stringify(customerContentVars)]);

  const handleSave = async () => {
    if (!selectedCustomer) return;

    try {
      await saveMutation.mutateAsync({
        customerId: selectedCustomer,
        data: contentVariables,
      });
    } catch (error) {
      handleError(error, 'Failed to save customer content variables');
    }
  };

  const addVariable = () => {
    const newKey = `new_var_${Date.now()}`;
    setContentVariables((prev) => ({ ...prev, [newKey]: '' }));
  };

  const updateVariable = (oldKey: string, newKey: string, value: string) => {
    setContentVariables(prev => {
      const newVars = { ...prev };
      if (oldKey !== newKey) delete newVars[oldKey];
      newVars[newKey] = value;
      return newVars;
    });
  };

  const removeVariable = (key: string) => {
    setContentVariables(prev => {
      const { [key]: _, ...rest } = prev;
      return rest;
    });
  };

  const toggleRedefined = (key: string, isRedefined: boolean) => {
    setContentVariables(prev => {
      const newVars = { ...prev };
      if (isRedefined) {
        newVars[key] = mergedVariables[key]?.value || globalContentVariables[key] || '';
      } else {
        delete newVars[key];
      }
      return newVars;
    });
  };

  const isInitialLoading = customersLoading;
  const isSettingsLoading = selectedCustomer && settingsLoading;

  return (
    <PageErrorBoundary pageName="Customer Settings">
      <PageHeader
        icon="fa-code"
        title="Customer Content Variables"
        description="Choose a customer to view and edit their specific content variable overrides."
      />

      {isInitialLoading ? (
        <LoadingSpinner text="Loading customer data..." />
      ) : (
        <ComponentErrorBoundary componentName="Customer Selector">
          <CustomerSelector
            customers={customers}
            selectedCustomer={selectedCustomer}
            onCustomerChange={setSelectedCustomer}
          />
        </ComponentErrorBoundary>
      )}

      {selectedCustomer && (
        <ComponentErrorBoundary componentName="Customer Variables Editor">
          <div className="card">
            <div className="card-header">
              <h3>
                <i className="fas fa-code me-2"></i>
                Edit Content Variables: {selectedCustomer}
              </h3>
              <small className="text-muted">
                Configure customer-specific content variable overrides
              </small>
            </div>
            <div className="card-body">
              {isSettingsLoading ? (
                <TableSkeleton rows={5} columns={4} />
              ) : (
                <>
                  <div className="d-flex justify-content-between align-items-center mb-3">
                    <h4>Content Variables Editor</h4>
                    <div>
                      <button type="button" className="btn btn-success me-2" onClick={addVariable}>
                        <i className="fas fa-plus me-1"></i>Add Variable
                      </button>
                      <SaveButton
                        onClick={handleSave}
                        loading={saveMutation.isPending}
                        text="Save Variables"
                      />
                    </div>
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
                                    className={`form-control form-control-sm ${data.globalValue ? 'bg-light' : ''}`}
                                    value={editingKeys[key] !== undefined ? editingKeys[key] : key}
                                    readOnly={!!data.globalValue}
                                    onChange={data.globalValue ? undefined : (e) => 
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
                  ) : (
                    <div className="text-center py-4">
                      <i className="fas fa-code fa-3x text-muted mb-3"></i>
                      <h5 className="text-muted">No Content Variables</h5>
                      <p className="text-muted">
                        No content variables found for this customer. Click "Add Variable" to start
                        adding variables.
                      </p>
                    </div>
                  )}
                </>
              )}
            </div>
          </div>
        </ComponentErrorBoundary>
      )}

      {!selectedCustomer && !isInitialLoading && (
        <div className="card">
          <div className="card-body text-center py-4">
            <i className="fas fa-code fa-3x text-muted mb-3"></i>
            <h4 className="text-muted">Select a Customer</h4>
            <p className="text-muted">
              Choose a customer from the search box above to start managing their content variable
              overrides.
            </p>
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};
