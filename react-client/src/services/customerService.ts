import axios from 'axios';

const api = axios.create({
  baseURL: '/api/customers',
  headers: { 'Content-Type': 'application/json' },
});

export const customerService = {
  getCustomers: async (): Promise<string[]> => {
    const response = await api.get('/');
    return response.data;
  },

  getCustomerEvents: async (customerId: string): Promise<any> => {
    const response = await api.get(`/${customerId}/events`);
    return response.data;
  },

  saveCustomerEvents: async (customerId: string, data: any): Promise<boolean> => {
    const response = await api.post(`/${customerId}/events`, data);
    return response.data.success;
  },

  getCustomerSettings: async (customerId: string): Promise<any> => {
    const response = await api.get(`/${customerId}/settings`);
    return response.data;
  },

  saveCustomerSettings: async (customerId: string, data: any): Promise<boolean> => {
    const response = await api.post(`/${customerId}/settings`, data);
    return response.data.success;
  },
};
