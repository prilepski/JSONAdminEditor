import axios from 'axios';
import { Template, EventTrigger, OrderType, EventMapping } from '../types';

export const eventService = {
  // Dictionary endpoints
  getActiveEventTriggers: async (): Promise<EventTrigger[]> => {
    const response = await axios.get('/api/dictionaries/event-triggers');
    return response.data;
  },

  getAvailableOrderTypes: async (): Promise<OrderType[]> => {
    const response = await axios.get('/api/dictionaries/order-types');
    return response.data;
  },

  getAvailableTemplates: async (): Promise<Template[]> => {
    const response = await axios.get('/api/dictionaries/templates');
    return response.data;
  },

  // Config endpoints
  getAllEvents: async (): Promise<EventMapping[]> => {
    const response = await axios.get('/api/config/events');
    return response.data;
  },

  getEventByNameAndOrderType: async (eventName: string, orderType: string): Promise<EventMapping> => {
    const response = await axios.get(`/api/config/events/${eventName}/order-types/${orderType}`);
    return response.data;
  },

  saveEvent: async (eventName: string, orderType: string, eventData: EventMapping): Promise<void> => {
    await axios.put(`/api/config/events/${eventName}/order-types/${orderType}`, eventData);
  },

  deleteEvent: async (eventName: string, orderType: string): Promise<void> => {
    await axios.delete(`/api/config/events/${eventName}/order-types/${orderType}`);
  },
};
