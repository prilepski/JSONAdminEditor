import React from 'react';
import { EventTrigger, OrderType } from '../../types';

interface EventSelectorProps {
  selectedEvent: string;
  selectedOrderType: string;
  eventTriggers: EventTrigger[];
  orderTypes: OrderType[];
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
  onOrderTypeChange,
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
            {eventTriggers.map((trigger) => (
              <option key={trigger.eventName} value={trigger.eventName}>
                {trigger.eventName}
              </option>
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
              {orderTypes.map((type) => (
                <option key={type.name} value={type.name}>
                  {type.name}
                </option>
              ))}
            </select>
          </div>
        )}
      </div>
    </div>
  </div>
);
