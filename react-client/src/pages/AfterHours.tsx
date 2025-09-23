import React from 'react';
import { useAfterHoursQuery, useAfterHoursMutation } from '../hooks/useAfterHoursQuery';
import { useEventTriggersQuery, useOrderTypesQuery } from '../hooks/useEventTriggersQuery';
import { useAfterHoursForm } from '../hooks/useAfterHoursForm';
import { useErrorHandler } from '../hooks/useErrorHandler';
import { AfterHours2 } from '../types';
import { SaveButton, LoadingSpinner } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { TimeInputs } from '../components/afterhours/TimeInputs';
import { EventRow } from '../components/afterhours/EventRow';

export const AfterHours: React.FC = () => {
  const { data, isLoading } = useAfterHoursQuery();
  const { data: eventTriggers = [] } = useEventTriggersQuery();
  const { data: orderTypes = [] } = useOrderTypesQuery();
  const saveMutation = useAfterHoursMutation();
  const { formData, updateTime, addEvent, removeEvent, updateEvent } = useAfterHoursForm(data);
  const { handleError } = useErrorHandler({ context: 'AfterHours' });

  const handleSave = async () => {
    try {
      await saveMutation.mutateAsync(formData as AfterHours2);
    } catch (error) {
      handleError(error, 'Failed to save after hours settings');
    }
  };

  return (
    <PageErrorBoundary pageName="After Hours">
      <h1 className="mb-4">
        <i className="fas fa-clock me-2"></i>After Hours Management
      </h1>

      <div className="card">
        <div className="card-header d-flex justify-content-between align-items-center">
          <h3><i className="fas fa-edit me-2"></i>After Hours Settings</h3>
          <SaveButton onClick={handleSave} loading={saveMutation.isPending} text="Save Changes" />
        </div>
        <div className="card-body">
          {isLoading ? (
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
    </PageErrorBoundary>
  );
};