import React from 'react';

interface EventSelectorProps {
  selectedEvent: string;
  selectedOrderType: string;
  eventTriggers: string[];
  orderTypes: string[];
  eventSupportsByOrderType: boolean;
  onEventChange: (event: string) => void;
  onOrderTypeChange: (orderType: string) => void;
}

export const EventSelector: React.FC<EventSelectorProps> = ({
  selectedEvent,
  selectedOrderType,
  eventTriggers,
  orderTypes,
  eventSupportsByOrderType,
  onEventChange,
  onOrderTypeChange
}) => (
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
            {eventTriggers.map(trigger => (
              <option key={trigger} value={trigger}>{trigger}</option>
            ))}
          </select>
        </div>
        {eventSupportsByOrderType && (
          <div className="col-md-6">
            <select
              className="form-select"
              value={selectedOrderType}
              onChange={(e) => onOrderTypeChange(e.target.value)}
            >
              {orderTypes.map(type => (
                <option key={type} value={type}>{type}</option>
              ))}
            </select>
          </div>
        )}
      </div>
    </div>
  </div>
);