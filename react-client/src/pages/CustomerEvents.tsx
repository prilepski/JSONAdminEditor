import React, { useState } from 'react';
import { useCustomersQuery, useCustomerEventsQuery, useCustomerEventsMutation } from '../hooks/useCustomerQuery';
import toast from 'react-hot-toast';
import { Skeleton } from '../components/Skeleton';

export const CustomerEvents: React.FC = () => {
  const [selectedCustomer, setSelectedCustomer] = useState<string>('');
  const { data: customers = [] } = useCustomersQuery();
  const { data: customerData, isLoading } = useCustomerEventsQuery(selectedCustomer);
  const saveMutation = useCustomerEventsMutation();




  const handleSave = async () => {
    if (!selectedCustomer || !customerData) return;

    try {
      const success = await saveMutation.mutateAsync({ customerId: selectedCustomer, data: customerData });
      if (success) {
        toast.success(`Customer events for '${selectedCustomer}' saved successfully!`);
      } else {
        toast.error('Failed to save customer events');
      }
    } catch (error) {
      toast.error('Error saving customer events');
    }
  };

  return (
    <>
      <h1 className="mb-4">
        <i className="fas fa-calendar-alt me-2"></i>Customer Events Management
      </h1>

      <div className="card mb-4">
        <div className="card-header">
          <h3>Select Customer</h3>
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

      {selectedCustomer && (
        <div className="card">
          <div className="card-header">
            <h3>Customer Events for {selectedCustomer}</h3>
          </div>
          <div className="card-body">
            <Skeleton loading={isLoading} rows={5} height="40px">
              {customerData ? (
              <div>
                <pre className="bg-light p-3 rounded">
                  {JSON.stringify(customerData, null, 2)}
                </pre>
                <button
                  type="button"
                  className="btn btn-primary mt-3"
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
                      <i className="fas fa-save me-1"></i>
                      Save Changes
                    </>
                  )}
                </button>
              </div>
              ) : (
                <p className="text-muted">No customer events data found.</p>
              )}
            </Skeleton>
          </div>
        </div>
      )}
    </>
  );
};