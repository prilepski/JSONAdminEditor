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
  const { handleError } = useErrorHandler({ context: 'CustomerSettings' });

  const { data: customers = [], isLoading: customersLoading } = useCustomersQuery();
  const { data: customerContentVars = {}, isLoading: settingsLoading } =
    useCustomerContentVariablesQuery(selectedCustomer);
  const { data: globalContentVariables = {} } = useContentVariablesQuery();
  const saveMutation = useCustomerContentVariablesMutation();

  useEffect(() => {
    if (selectedCustomer) {
      // Merge global and customer variables
      const merged = { ...globalContentVariables, ...customerContentVars };
      setContentVariables(merged);
    } else {
      setContentVariables({});
    }
  }, [selectedCustomer, globalContentVariables, JSON.stringify(customerContentVars)]);

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
    setContentVariables((prev) => {
      const newVars = { ...prev };
      if (oldKey !== newKey) {
        delete newVars[oldKey];
      }
      newVars[newKey] = value;
      return newVars;
    });
  };

  const removeVariable = (key: string) => {
    setContentVariables((prev) => {
      const newVars = { ...prev };
      delete newVars[key];
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

                  {Object.keys(contentVariables).length > 0 ? (
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
                          {Object.entries(contentVariables).map(([key, value], index) => {
                            const isRedefined = key in customerContentVars;
                            const isGlobal = key in globalContentVariables;
                            return (
                              <tr key={`setting-${index}-${key}`}>
                                <td>
                                  <input
                                    type="text"
                                    className={`form-control form-control-sm ${isGlobal ? 'bg-light' : ''}`}
                                    value={key}
                                    readOnly={isGlobal}
                                    onChange={isGlobal ? undefined : (e) => updateVariable(key, e.target.value, value)}
                                  />
                                </td>
                                <td>
                                  <input
                                    type="text"
                                    className="form-control form-control-sm"
                                    value={value}
                                    onChange={(e) => updateVariable(key, key, e.target.value)}
                                  />
                                </td>
                                <td className="text-center">
                                  <div className="form-check">
                                    <input
                                      className="form-check-input"
                                      type="checkbox"
                                      checked={isRedefined}
                                      readOnly
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
                            );
                          })}
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
