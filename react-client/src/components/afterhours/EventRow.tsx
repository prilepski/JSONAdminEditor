import React from 'react';
import { EventTrigger, OrderType } from '../../types';

interface EventRowProps {
  event: { name: string; type: string };
  index: number;
  eventTriggers: EventTrigger[];
  orderTypes: OrderType[];
  onUpdate: (index: number, field: string, value: string) => void;
  onRemove: (index: number) => void;
}

export const EventRow: React.FC<EventRowProps> = ({ 
  event, index, eventTriggers, orderTypes, onUpdate, onRemove 
}) => (
  <tr>
    <td>
      <select
        className="form-select form-select-sm"
        value={event.name || ''}
        onChange={(e) => onUpdate(index, 'name', e.target.value)}
      >
        <option value="">Select Event</option>
        {event.name && !eventTriggers.find(t => t.eventName === event.name) && (
          <option key={event.name} value={event.name}>{event.name}</option>
        )}
        {eventTriggers.map(trigger => (
          <option key={trigger.eventName} value={trigger.eventName}>
            {trigger.eventName}
          </option>
        ))}
      </select>
    </td>
    <td>
      <select
        className="form-select form-select-sm"
        value={event.type || ''}
        onChange={(e) => onUpdate(index, 'type', e.target.value)}
      >
        <option value="">Select Type</option>
        {event.type && !orderTypes.find(t => t.name === event.type) && (
          <option key={event.type} value={event.type}>{event.type}</option>
        )}
        {orderTypes.map(orderType => (
          <option key={orderType.name} value={orderType.name}>
            {orderType.name}
          </option>
        ))}
      </select>
    </td>
    <td>
      <button className="btn btn-danger btn-sm" onClick={() => onRemove(index)}>
        <i className="fas fa-trash"></i>
      </button>
    </td>
  </tr>
);