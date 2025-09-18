import axios from 'axios';

const api = axios.create({
  baseURL: '/api/events',
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

  getAvailableTemplates: async (): Promise<Array<{templateId: string, templateName: string}>> => {
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

  addNewEvent: async (eventName: string, eventData: any): Promise<boolean> => {
    const response = await api.post('/add', { eventName, eventData });
    return response.data.success;
  },

  updateEventByOrderType: async (eventName: string, orderType: string, eventData: any): Promise<boolean> => {
    const response = await api.post('/update', { eventName, orderType, eventData });
    return response.data.success;
  },
};