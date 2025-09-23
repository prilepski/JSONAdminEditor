import React, { useMemo } from 'react';
import { EventMapping } from '../../types';

interface EventSelectorProps {
  selectedEvent: string;
  selectedOrderType: string;
  allEvents: EventMapping[];
  onEventChange: (event: string) => void;
  onOrderTypeChange: (orderType: string) => void;
}

export const EventSelector: React.FC<EventSelectorProps> = ({
  selectedEvent,
  selectedOrderType,
  allEvents,
  onEventChange,
  onOrderTypeChange,
}) => {
  const eventOptions = useMemo(() => {
    return allEvents.map(event => ({
      value: `${event.event}|${event.orderType}`,
      label: `${event.event} - ${event.orderType}`,
      event: event.event,
      orderType: event.orderType
    }));
  }, [allEvents]);
  
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
        <h3>Select Event</h3>
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
