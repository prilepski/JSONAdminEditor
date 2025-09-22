import React from 'react';
import { ToggleButtonGroup } from '../common';
import { EventTrigger } from '../../types';

interface CustomerEventSelectorProps {
  selectedCustomer: string;
  selectedEvent: string;
  selectedOrderType: string;
  eventTriggers: EventTrigger[];
  onEventChange: (event: string) => void;
  onOrderTypeChange: (orderType: string) => void;
}

export const CustomerEventSelector: React.FC<CustomerEventSelectorProps> = ({
  selectedCustomer,
  selectedEvent,
  selectedOrderType,
  eventTriggers,
  onEventChange,
  onOrderTypeChange,
}) => (
  <div className="card mb-4">
    <div className="card-header">
      <h3>
        <i className="fas fa-calendar-alt me-2"></i>Select Event for {selectedCustomer}
      </h3>
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
        <div className="col-md-6">
          <ToggleButtonGroup
            options={[
              { value: 'Delivery', label: 'Delivery' },
              { value: 'Pickup', label: 'Pickup' }
            ]}
            selected={selectedOrderType}
            onChange={onOrderTypeChange}
            size="sm"
          />
        </div>
      </div>
    </div>
  </div>
);
