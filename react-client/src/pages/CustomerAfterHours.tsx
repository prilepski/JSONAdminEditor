import React, { useState } from 'react';
import { useCustomersQuery, useCustomerAfterHoursQuery, useCustomerAfterHoursMutation } from '../hooks/useCustomerQuery';
import { useEventTriggersQuery, useOrderTypesQuery } from '../hooks/useEventTriggersQuery';
import { useAfterHoursForm } from '../hooks/useAfterHoursForm';
import { useErrorHandler } from '../hooks/useErrorHandler';
import { AfterHours2 } from '../types';
import { PageHeader, CustomerSelector, SaveButton, LoadingSpinner } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { TimeInputs } from '../components/afterhours/TimeInputs';
import { EventRow } from '../components/afterhours/EventRow';

export const CustomerAfterHours: React.FC = () => {
  const [selectedCustomer, setSelectedCustomer] = useState<string>('');
  const { handleError } = useErrorHandler({ context: 'CustomerAfterHours' });

  const { data: customers = [], isLoading: customersLoading } = useCustomersQuery();
  const { data: customerAfterHours, isLoading: afterHoursLoading } = useCustomerAfterHoursQuery(selectedCustomer);
  const { data: eventTriggers = [] } = useEventTriggersQuery();
  const { data: orderTypes = [] } = useOrderTypesQuery();
  const saveMutation = useCustomerAfterHoursMutation();
  const { formData, updateTime, addEvent, removeEvent, updateEvent } = useAfterHoursForm(customerAfterHours);

  const handleSave = async () => {
    if (!selectedCustomer) return;

    try {
      await saveMutation.mutateAsync({
        customerId: selectedCustomer,
        data: formData as AfterHours2,
      });
    } catch (error) {
      handleError(error, 'Failed to save customer after hours settings');
    }
  };

  const isInitialLoading = customersLoading;
  const isAfterHoursLoading = selectedCustomer && afterHoursLoading;

  return (
    <PageErrorBoundary pageName="Customer After Hours">
      <PageHeader
        icon="fa-clock"
        title="Customer After Hours Management"
        description="Configure after-hours settings and exception events for specific customers."
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
        <div className="card">
          <div className="card-header">
            <div className="d-flex justify-content-between align-items-center">
              <h3>
                <i className="fas fa-edit me-2"></i>After Hours Settings: {selectedCustomer}
              </h3>
              <SaveButton
                onClick={handleSave}
                loading={saveMutation.isPending}
                text="Save Settings"
              />
            </div>
          </div>
          <div className="card-body">
            {isAfterHoursLoading ? (
              <LoadingSpinner text="Loading after hours settings..." />
            ) : (
              <ComponentErrorBoundary componentName="After Hours Editor">
                <TimeInputs
                  startTime={formData.restrictedHoursPeriod.start}
                  endTime={formData.restrictedHoursPeriod.end}
                  onTimeChange={updateTime}
                />

                <div className="d-flex justify-content-between align-items-center mb-3">
                  <h5>Exception Events</h5>
                  <button className="btn btn-success btn-sm" onClick={addEvent}>
                    <i className="fas fa-plus me-1"></i>Add Event
                  </button>
                </div>

                <div className="table-responsive">
                  <table className="table table-striped">
                    <thead>
                      <tr>
                        <th>Event Name</th>
                        <th>Event Type</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {formData.exceptionOfValidation.events.map((event, index) => (
                        <EventRow
                          key={index}
                          event={event}
                          index={index}
                          eventTriggers={eventTriggers}
                          orderTypes={orderTypes}
                          onUpdate={updateEvent}
                          onRemove={removeEvent}
                        />
                      ))}
                    </tbody>
                  </table>
                </div>
              </ComponentErrorBoundary>
            )}
          </div>
        </div>
      )}

      {!selectedCustomer && !isInitialLoading && (
        <div className="card">
          <div className="card-body text-center py-4">
            <i className="fas fa-clock fa-3x text-muted mb-3"></i>
            <h4 className="text-muted">Select a Customer</h4>
            <p className="text-muted">
              Choose a customer from the search box above to configure their after-hours settings.
            </p>
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};