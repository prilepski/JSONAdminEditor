import React from 'react';
import { useEventTriggersQuery, useOrderTypesQuery } from '../../hooks/useDictionaryQuery';

interface CustomerEventSelectorProps {
  selectedCustomer: string;
  selectedEvent: string;
  selectedOrderType: string;
  onEventChange: (event: string) => void;
  onOrderTypeChange: (orderType: string) => void;
}

export const CustomerEventSelector: React.FC<CustomerEventSelectorProps> = ({
  selectedCustomer,
  selectedEvent,
  selectedOrderType,
  onEventChange,
  onOrderTypeChange,
}) => {
  const { data: eventTriggers = [], isLoading: triggersLoading, error: triggersError } = useEventTriggersQuery();
  const { data: orderTypes = [], isLoading: typesLoading, error: typesError } = useOrderTypesQuery();
  
  // Handle loading and errors
  if (triggersLoading || typesLoading) {
    return (
      <div className="card mb-4">
        <div className="card-body text-center">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      </div>
    );
  }
  
  if (triggersError || typesError) {
    return (
      <div className="card mb-4">
        <div className="card-body text-center text-danger">
          Error loading event data
        </div>
      </div>
    );
  }
  
  const activeEvents = Array.isArray(eventTriggers) ? eventTriggers.filter(trigger => trigger?.isActive) : [];
  
  // Create combined options when customer is selected
  const eventOptions = selectedCustomer && Array.isArray(orderTypes) ? activeEvents.flatMap(trigger =>
    orderTypes.map(orderType => ({
      value: `${trigger?.eventName}|${orderType?.name}`,
      label: `${trigger?.eventName} - ${orderType?.name}`,
      event: trigger?.eventName,
      orderType: orderType?.name
    }))
  ) : [];
  
  const selectedValue = selectedEvent && selectedOrderType ? `${selectedEvent}|${selectedOrderType}` : '';
  
  const handleChange = (value: string) => {
    if (!value) {
      onEventChange('');
      onOrderTypeChange('');
      return;
    }
    
    const [event, orderType] = value.split('|');
    onEventChange(event);
    onOrderTypeChange(orderType);
  };
  
  return (
    <div className="card mb-4">
      <div className="card-header">
        <h3>
          <i className="fas fa-calendar-alt me-2"></i>Select Event for {selectedCustomer}
        </h3>
      </div>
      <div className="card-body">
        {selectedCustomer ? (
          <select
            className="form-select"
            value={selectedValue}
            onChange={(e) => handleChange(e.target.value)}
          >
            <option value="">Select an event...</option>
            {eventOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        ) : (
          <div className="text-muted text-center py-3">
            <i className="fas fa-info-circle me-2"></i>
            Please select a customer first to see available events.
          </div>
        )}
      </div>
    </div>
  );
};