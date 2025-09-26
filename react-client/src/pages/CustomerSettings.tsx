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
  ContentVariablesTable,
  getRedefinedVariables,
} from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

export const CustomerSettings: React.FC = () => {
  const [selectedCustomer, setSelectedCustomer] = useState<string>('');
  const [contentVariables, setContentVariables] = useState<Record<string, string>>({});
  const [redefinedStates, setRedefinedStates] = useState<Record<string, boolean>>({});
  const { handleError } = useErrorHandler({ context: 'CustomerSettings' });

  const { data: customers = [], isLoading: customersLoading } = useCustomersQuery();
  const { data: customerContentVars = {}, isLoading: settingsLoading } =
    useCustomerContentVariablesQuery(selectedCustomer);
  const { data: globalContentVariables = {} } = useContentVariablesQuery();
  const saveMutation = useCustomerContentVariablesMutation();



  useEffect(() => {
    setContentVariables(selectedCustomer ? customerContentVars || {} : {});
  }, [selectedCustomer, JSON.stringify(customerContentVars)]);

  const handleSave = async () => {
    if (!selectedCustomer) return;

    const dataToSave = getRedefinedVariables(contentVariables, redefinedStates);

    try {
      await saveMutation.mutateAsync({
        customerId: selectedCustomer,
        data: dataToSave,
      });
    } catch (error) {
      handleError(error, 'Failed to save customer content variables');
    }
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
                  <ContentVariablesTable
                    contentVariables={contentVariables}
                    globalContentVariables={globalContentVariables}
                    onUpdate={setContentVariables}
                    onRedefinedStatesChange={setRedefinedStates}
                    title="Content Variables Editor"
                    saveButton={
                      <SaveButton
                        onClick={handleSave}
                        loading={saveMutation.isPending}
                        text="Save Variables"
                      />
                    }
                  />
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
