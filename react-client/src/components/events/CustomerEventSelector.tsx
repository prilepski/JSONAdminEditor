import React from 'react';

interface CustomerEventSelectorProps {
  selectedCustomer: string;
  selectedEvent: string;
  selectedOrderType: string;
  eventTriggers: string[];
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
              <option key={trigger} value={trigger}>
                {trigger}
              </option>
            ))}
          </select>
        </div>
        <div className="col-md-6">
          <div className="d-flex align-items-center">
            <span className="me-2 text-muted">Delivery</span>
            <div className="form-check form-switch">
              <input
                className="form-check-input"
                type="checkbox"
                checked={selectedOrderType === 'Pickup'}
                onChange={(e) => onOrderTypeChange(e.target.checked ? 'Pickup' : 'Delivery')}
              />
            </div>
            <span className="ms-2 text-muted">Pickup</span>
          </div>
        </div>
      </div>
    </div>
  </div>
);
