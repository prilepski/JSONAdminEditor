import axios from 'axios';
import { Template, EventTrigger, OrderType } from '../types';

const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_BASE_URL || '/api'}/dictionaries`,
  headers: { 'Content-Type': 'application/json' },
});

export const eventService = {
  getActiveEventTriggers: async (): Promise<EventTrigger[]> => {
    const response = await api.get('/event-triggers');
    return response.data;
  },

  getAvailableOrderTypes: async (): Promise<OrderType[]> => {
    const response = await api.get('/order-types');
    return response.data;
  },

  getAvailableTemplates: async (orderType?: string): Promise<Template[]> => {
    const response = await api.get('/templates');
    return response.data;
  },

  checkEventSupportsByOrderType: async (eventName: string): Promise<boolean> => {
    const response = await api.get(`/supports-order-type?eventName=${eventName}`);
    return response.data;
  },

  getEventByNameAndOrderType: async (eventName: string, orderType: string): Promise<any> => {
    const response = await api.get(`/data?eventName=${eventName}&orderType=${orderType}`);
    return response.data;
  },

  getEventTemplate: async (): Promise<any> => {
    const response = await api.get('/template');
    return response.data;
  },

  saveEvent: async (
    eventName: string,
    orderType: string,
    eventData: any,
    isNew: boolean
  ): Promise<boolean> => {
    const response = await api.post('/save', {
      eventName,
      orderType,
      eventData,
      isNew,
    });
    return response.data.success;
  },
};
