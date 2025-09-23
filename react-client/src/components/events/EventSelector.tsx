import React, { useMemo } from 'react';
import { EventMapping, OrderType } from '../../types';

interface EventSelectorProps {
  selectedEvent: string;
  selectedOrderType: string;
  allEvents: EventMapping[];
  orderTypes: OrderType[];
  onEventChange: (event: string) => void;
  onOrderTypeChange: (orderType: string) => void;
}

export const EventSelector: React.FC<EventSelectorProps> = ({
  selectedEvent,
  selectedOrderType,
  allEvents,
  orderTypes,
  onEventChange,
  onOrderTypeChange,
}) => {
  const uniqueEvents = useMemo(() => 
    [...new Set(allEvents.map(event => event.event))], 
    [allEvents]
  );
  
  return (
    <div className="card mb-4">
      <div className="card-header">
        <h3>Select Event</h3>
      </div>
      <div className="card-body">
        <div className="row">
          <div className="col-md-6">
            <select
              className="form-select"
              value={selectedEvent}
              onChange={(e) => onEventChange(e.target.value)}
            >
              <option value="">Select an event...</option>
              {uniqueEvents.map((eventName) => (
                <option key={eventName} value={eventName}>
                  {eventName}
                </option>
              ))}
            </select>
          </div>
          <div className="col-md-6">
            <select
              className="form-select"
              value={selectedOrderType}
              onChange={(e) => onOrderTypeChange(e.target.value)}
            >
              {orderTypes.map((type) => (
                <option key={type.name} value={type.name}>
                  {type.name}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>
    </div>
  );
};
