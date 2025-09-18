import React, { useState, useEffect } from 'react';
import { useCustomersQuery, useCustomerSettingsQuery, useCustomerSettingsMutation } from '../hooks/useCustomerQuery';
import { useContentVariablesQuery } from '../hooks/useContentVariableQuery';
import toast from 'react-hot-toast';
import { CustomerContentVariable } from '../types/customer';
import { CustomerSettingsData } from '../types/customerSettings';

export const CustomerSettings: React.FC = () => {
  const [selectedCustomer, setSelectedCustomer] = useState<string>('');
  const [contentVariables, setContentVariables] = useState<CustomerContentVariable[]>([]);
  const [showEditingSection, setShowEditingSection] = useState(false);
  
  const { data: customers = [] } = useCustomersQuery();
  const { data: customerSettings } = useCustomerSettingsQuery(selectedCustomer);
  const { data: globalVariables = [] } = useContentVariablesQuery();
  const saveMutation = useCustomerSettingsMutation();

  const loadContentVariables = () => {
    const customerVars = customerSettings?.contentVariables || {};
    const variables: CustomerContentVariable[] = [];

    // Add customer-specific variables (not in global)
    Object.entries(customerVars).forEach(([name, value]) => {
      const isInGlobal = globalVariables.some(gv => gv['Variable Name'] === name);
      if (!isInGlobal) {
        variables.push({
          name,
          value: String(value),
          isRedefined: false,
          isCustomerSpecific: true
        });
      }
    });

    // Add global variables with redefinition status
    globalVariables.forEach(globalVar => {
      const name = String(globalVar['Variable Name']);
      const globalValue = globalVar['Variable Mapping'] || globalVar['Variable Value'] || '';
      const isRedefined = customerVars.hasOwnProperty(name);
      const value = isRedefined ? String(customerVars[name as keyof typeof customerVars]) : String(globalValue);

      variables.push({
        name,
        value,
        isRedefined,
        isCustomerSpecific: false,
        globalValue: String(globalValue)
      });
    });

    setContentVariables(variables);
  };

  useEffect(() => {
    if (selectedCustomer) {
      loadContentVariables();
      setShowEditingSection(true);
    } else {
      setShowEditingSection(false);
      setContentVariables([]);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedCustomer, customerSettings, globalVariables]);



  const handleSave = async () => {
    if (!selectedCustomer) return;

    const saveData: CustomerSettingsData = {
      contentVariables: {}
    };

    // Save customer-specific variables and redefined global variables
    contentVariables.forEach(variable => {
      if (variable.isCustomerSpecific || variable.isRedefined) {
        saveData.contentVariables[variable.name] = variable.value;
      }
    });

    try {
      const success = await saveMutation.mutateAsync({ customerId: selectedCustomer, data: saveData });
      if (success) {
        toast.success(`Customer content variables for '${selectedCustomer}' saved successfully!`);
      } else {
        toast.error('Failed to save customer settings');
      }
    } catch (error) {
      toast.error('Error saving customer settings');
    }
  };

  const addVariable = () => {
    const newVariable: CustomerContentVariable = {
      name: `new_var_${Date.now()}`,
      value: '',
      isRedefined: false,
      isCustomerSpecific: true
    };
    setContentVariables(prev => [newVariable, ...prev]);
  };

  const updateVariable = (index: number, field: keyof CustomerContentVariable, value: any) => {
    setContentVariables(prev => prev.map((variable, i) => 
      i === index ? { ...variable, [field]: value } : variable
    ));
  };

  const removeVariable = (index: number) => {
    setContentVariables(prev => prev.filter((_, i) => i !== index));
  };

  const toggleRedefined = (index: number, isRedefined: boolean) => {
    setContentVariables(prev => prev.map((variable, i) => {
      if (i === index) {
        return {
          ...variable,
          isRedefined,
          value: isRedefined ? variable.value : (variable.globalValue || '')
        };
      }
      return variable;
    }));
  };

  return (
    <>
      <h1 className="mb-4">
        <i className="fas fa-code me-2"></i>Customer Content Variables
      </h1>

      {/* Customer Selection */}
      <div className="card mb-4">
        <div className="card-header">
          <h3><i className="fas fa-user-search me-2"></i>Select Customer</h3>
          <p className="mb-0 text-muted">Choose a customer to view and edit their specific content variable overrides.</p>
        </div>
        <div className="card-body">
          <select
            className="form-select"
            value={selectedCustomer}
            onChange={(e) => setSelectedCustomer(e.target.value)}
          >
            <option value="">Select a customer...</option>
            {customers.map(customer => (
              <option key={customer} value={customer}>{customer}</option>
            ))}
          </select>
        </div>
      </div>

      {/* Editing Section */}
      {showEditingSection && selectedCustomer && (
        <div className="card">
          <div className="card-header">
            <h3>
              <i className="fas fa-code me-2"></i>
              Edit Content Variables: {selectedCustomer}
            </h3>
            <small className="text-muted">Configure customer-specific content variable overrides</small>
          </div>
          <div className="card-body">
            <div className="d-flex justify-content-between align-items-center mb-3">
              <h4>Content Variables Editor</h4>
              <div>
                <button
                  type="button"
                  className="btn btn-success me-2"
                  onClick={addVariable}
                >
                  <i className="fas fa-plus me-1"></i>Add Variable
                </button>
                <button
                  type="button"
                  className="btn btn-primary"
                  onClick={handleSave}
                  disabled={saveMutation.isPending}
                >
                  {saveMutation.isPending ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-1"></span>
                      Saving...
                    </>
                  ) : (
                    <>
                      <i className="fas fa-save me-1"></i>Save Variables
                    </>
                  )}
                </button>
              </div>
            </div>

            {contentVariables.length > 0 ? (
              <div className="table-responsive">
                <table className="table table-striped table-hover">
                  <thead className="table-dark">
                    <tr>
                      <th style={{ width: '25%' }}>Variable Name</th>
                      <th style={{ width: '50%' }}>Value</th>
                      <th style={{ width: '15%' }}>Is Redefined</th>
                      <th style={{ width: '10%' }}>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {contentVariables.map((variable, index) => (
                      <tr key={`${variable.name}-${index}`}>
                        <td>
                          <input
                            type="text"
                            className="form-control form-control-sm"
                            value={variable.name}
                            readOnly={!variable.isCustomerSpecific}
                            onChange={(e) => updateVariable(index, 'name', e.target.value)}
                          />
                          {variable.isCustomerSpecific && (
                            <small className="text-warning">
                              <i className="fas fa-star me-1"></i>Customer-specific
                            </small>
                          )}
                        </td>
                        <td>
                          <input
                            type="text"
                            className="form-control form-control-sm"
                            value={variable.value}
                            readOnly={!variable.isCustomerSpecific && !variable.isRedefined}
                            onChange={(e) => updateVariable(index, 'value', e.target.value)}
                          />
                          {variable.isRedefined && (
                            <small className="text-warning">
                              <i className="fas fa-edit me-1"></i>Overridden
                            </small>
                          )}
                        </td>
                        <td className="text-center">
                          {variable.isCustomerSpecific ? (
                            <span className="text-muted">-</span>
                          ) : (
                            <div className="form-check">
                              <input
                                className="form-check-input"
                                type="checkbox"
                                checked={variable.isRedefined}
                                onChange={(e) => toggleRedefined(index, e.target.checked)}
                              />
                            </div>
                          )}
                        </td>
                        <td className="text-center">
                          {variable.isCustomerSpecific ? (
                            <button
                              type="button"
                              className="btn btn-danger btn-sm"
                              onClick={() => removeVariable(index)}
                            >
                              <i className="fas fa-trash"></i>
                            </button>
                          ) : (
                            <span className="text-muted">-</span>
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
                <p className="text-muted">No content variables found for this customer. Click "Add Variable" to start adding variables.</p>
              </div>
            )}
          </div>
        </div>
      )}

      {/* Default State */}
      {!showEditingSection && (
        <div className="card">
          <div className="card-body text-center py-4">
            <i className="fas fa-code fa-3x text-muted mb-3"></i>
            <h4 className="text-muted">Select a Customer</h4>
            <p className="text-muted">Choose a customer from the search box above to start managing their content variable overrides.</p>
          </div>
        </div>
      )}
    </>
  );
};