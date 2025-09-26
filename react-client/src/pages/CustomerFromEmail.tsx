import React, { useState, useEffect } from 'react';
import { SaveButton, LoadingSpinner, CustomerSelector } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { useCustomersQuery, useCustomerFromEmailQuery, useCustomerFromEmailMutation } from '../hooks/useCustomerQuery';
import { useErrorHandler } from '../hooks/useErrorHandler';

export const CustomerFromEmail: React.FC = () => {
  const [selectedCustomer, setSelectedCustomer] = useState<string>('');
  const [email, setEmail] = useState('');
  const { handleError } = useErrorHandler({ context: 'CustomerFromEmail' });
  
  const { data: customers = [], isLoading: customersLoading } = useCustomersQuery();
  const { data: customerFromEmail, isLoading: fromEmailLoading } = useCustomerFromEmailQuery(selectedCustomer);
  const saveMutation = useCustomerFromEmailMutation();

  useEffect(() => {
    if (customerFromEmail !== undefined) {
      setEmail(customerFromEmail);
    }
  }, [customerFromEmail]);

  useEffect(() => {
    if (!selectedCustomer) {
      setEmail('');
    }
  }, [selectedCustomer]);

  const handleSave = async () => {
    if (!selectedCustomer) return;

    try {
      await saveMutation.mutateAsync({
        customerId: selectedCustomer,
        email,
      });
    } catch (error) {
      handleError(error, 'Failed to save customer from email');
    }
  };

  const isInitialLoading = customersLoading;
  const isFromEmailLoading = selectedCustomer && fromEmailLoading;

  return (
    <PageErrorBoundary pageName="Customer From Email">
      <h1 className="mb-4">
        <i className="fas fa-envelope me-2"></i>Customer From Email Management
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
            <div className="d-flex justify-content-between align-items-center">
              <h3>
                <i className="fas fa-edit me-2"></i>From Email Settings: {selectedCustomer}
              </h3>
              <SaveButton
                onClick={handleSave}
                loading={saveMutation.isPending}
                text="Save Settings"
                disabled={!email.trim()}
              />
            </div>
          </div>
          <div className="card-body">
            {isFromEmailLoading ? (
              <LoadingSpinner text="Loading from email settings..." />
            ) : (
              <ComponentErrorBoundary componentName="From Email Editor">
                <div className="mb-3">
                  <label htmlFor="customerFromEmail" className="form-label">From Email Address</label>
                  <input
                    type="email"
                    className="form-control"
                    id="customerFromEmail"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="Enter from email address"
                  />
                </div>
              </ComponentErrorBoundary>
            )}
          </div>
        </div>
      )}

      {!selectedCustomer && !isInitialLoading && (
        <div className="card">
          <div className="card-body text-center py-4">
            <i className="fas fa-envelope fa-3x text-muted mb-3"></i>
            <h4 className="text-muted">Select a Customer</h4>
            <p className="text-muted">
              Choose a customer from the search box above to configure their from email settings.
            </p>
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};