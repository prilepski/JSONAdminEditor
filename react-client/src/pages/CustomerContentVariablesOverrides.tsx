import React, { useState } from 'react';
import { LoadingSpinner, CustomerSelector, ComponentErrorBoundary } from '../components/common';
import { PageErrorBoundary } from '../components/common';
import { NestedVariablesEditor } from '../components/common/NestedVariablesEditor';
import { useCustomersQuery, useCustomerContentVariablesOverridesQuery, useCustomerContentVariablesOverridesMutation } from '../hooks/useCustomerQuery';
import { useErrorHandler } from '../hooks/useErrorHandler';

export const CustomerContentVariablesOverrides: React.FC = () => {
  const [selectedCustomer, setSelectedCustomer] = useState<string>('');
  const { handleError } = useErrorHandler({ context: 'CustomerContentVariablesOverrides' });

  const { data: customers = [], isLoading: customersLoading } = useCustomersQuery();
  const { data: overridesData, isLoading: overridesLoading, error: overridesError } = useCustomerContentVariablesOverridesQuery(selectedCustomer);
  const saveMutation = useCustomerContentVariablesOverridesMutation();

  const handleSave = async (category: string, subcategory: string, variables: Record<string, string>) => {
    if (!selectedCustomer) return;

    const updatedData = {
      ...overridesData,
      [category]: {
        ...overridesData?.[category],
        [subcategory]: variables
      }
    };

    try {
      await saveMutation.mutateAsync({
        customerId: selectedCustomer,
        data: updatedData,
      });
    } catch (error) {
      handleError(error, 'Failed to save customer content variables overrides');
    }
  };

  const handleAddCategory = async (name: string) => {
    if (!name.trim() || Object.keys(overridesData || {}).includes(name)) return;
    
    const updatedData = { ...overridesData, [name]: {} };
    try {
      await saveMutation.mutateAsync({ customerId: selectedCustomer, data: updatedData });
    } catch (error) {
      handleError(error, 'Failed to add category');
    }
  };

  const handleAddSubcategory = async (category: string, name: string) => {
    if (!name.trim() || Object.keys(overridesData?.[category] || {}).includes(name)) return;
    
    const updatedData = {
      ...overridesData,
      [category]: { ...overridesData?.[category], [name]: {} }
    };
    try {
      await saveMutation.mutateAsync({ customerId: selectedCustomer, data: updatedData });
    } catch (error) {
      handleError(error, 'Failed to add subcategory');
    }
  };

  const isInitialLoading = customersLoading;
  const isDataLoading = Boolean(selectedCustomer && overridesLoading);

  return (
    <PageErrorBoundary pageName="Customer Content Variables Overrides">
      <h1 className="mb-4">
        <i className="fas fa-layer-group me-2"></i>Customer Content Variables Overrides
      </h1>

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
        <div className="card">
          <div className="card-header">
            <h3>
              <i className="fas fa-edit me-2"></i>Content Variables Overrides: {selectedCustomer}
            </h3>
          </div>
          <div className="card-body">
            {isDataLoading ? (
              <LoadingSpinner text="Loading content variables overrides..." />
            ) : overridesError ? (
              <div className="alert alert-danger">
                Error loading content variables overrides: {overridesError.message}
              </div>
            ) : (
              <NestedVariablesEditor
                data={overridesData || {}}
                onSave={handleSave}
                onAddCategory={handleAddCategory}
                onAddSubcategory={handleAddSubcategory}
                isLoading={isDataLoading}
                isSaving={saveMutation.isPending}
              />
            )}
          </div>
        </div>
      )}

      {!selectedCustomer && !isInitialLoading && (
        <div className="card">
          <div className="card-body text-center py-4">
            <i className="fas fa-layer-group fa-3x text-muted mb-3"></i>
            <h4 className="text-muted">Select a Customer</h4>
            <p className="text-muted">
              Choose a customer from the search box above to configure their content variables overrides.
            </p>
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};