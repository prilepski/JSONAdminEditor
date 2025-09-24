import React, { useMemo } from 'react';
import { EventMapping } from '../../types';

interface CustomerEventSelectorProps {
  selectedCustomer: string;
  selectedEvent: string;
  selectedOrderType: string;
  customerEvents: EventMapping[];
  onEventChange: (event: string) => void;
  onOrderTypeChange: (orderType: string) => void;
}

export const CustomerEventSelector: React.FC<CustomerEventSelectorProps> = ({
  selectedCustomer,
  selectedEvent,
  selectedOrderType,
  customerEvents,
  onEventChange,
  onOrderTypeChange,
}) => {
  const eventOptions = useMemo(() => {
    return customerEvents.map(event => ({
      value: `${event.event}|${event.orderType}`,
      label: `${event.event} - ${event.orderType}`,
      event: event.event,
      orderType: event.orderType
    }));
  }, [customerEvents]);
  
  const selectedValue = selectedEvent && selectedOrderType ? `${selectedEvent}|${selectedOrderType}` : '';
  
  const handleChange = (value: string) => {
    if (!value) {
      onEventChange('');
      onOrderTypeChange('Delivery');
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
      </div>
    </div>
  );
};
