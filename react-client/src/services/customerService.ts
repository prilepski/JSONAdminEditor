import axios from 'axios';

const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_BASE_URL || '/api'}/customers`,
  headers: { 'Content-Type': 'application/json' },
});

export const customerService = {
  getCustomers: async (): Promise<string[]> => {
    const response = await api.get('/');
    return response.data;
  },

  getCustomerEvents: async (customerId: string, orderType?: string): Promise<any> => {
    const url = orderType ? `/${customerId}/events?orderType=${orderType}` : `/${customerId}/events`;
    const response = await api.get(url);
    // Return the Events array from customer data, or empty array if not found
    return response.data?.Events || [];
  },

  saveCustomerEvents: async (customerId: string, data: any): Promise<{ success: boolean; data?: any }> => {
    const response = await api.post(`/${customerId}/events`, data);
    return { success: response.data.success, data: response.data.data };
  },

  getCustomerSettings: async (customerId: string): Promise<any> => {
    const response = await api.get(`/${customerId}/settings`);
    return response.data;
  },

  saveCustomerSettings: async (customerId: string, data: any): Promise<{ success: boolean; data?: any }> => {
    const response = await api.post(`/${customerId}/settings`, data);
    return { success: response.data.success, data: response.data.data };
  },
};
