import axios from 'axios';
import { Template } from '../types/template';

const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_BASE_URL || '/api'}/events`,
  headers: { 'Content-Type': 'application/json' },
});

export const eventService = {
  getActiveEventTriggers: async (): Promise<string[]> => {
    const response = await api.get('/triggers');
    return response.data;
  },

  getAvailableOrderTypes: async (): Promise<string[]> => {
    const response = await api.get('/order-types');
    return response.data;
  },

  getAvailableTemplates: async (orderType?: string): Promise<Template[]> => {
    const url = orderType ? `/templates?orderType=${orderType}` : '/templates';
    const response = await api.get(url);
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
