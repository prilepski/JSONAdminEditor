import axios from 'axios';
import { EventMapping, CustomerNotificationMapping } from '../types';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const customerService = {
  getCustomers: async (): Promise<string[]> => {
    const response = await api.get<string[]>('/config/customers');
    return response.data;
  },

  getCustomerEvents: async (customerId: string): Promise<EventMapping[]> => {
    const response = await api.get<EventMapping[]>(`/config/customers/${customerId}/events`);
    return response.data || [];
  },

  saveCustomerEvents: async (customerId: string, eventName: string, orderType: string, data: EventMapping): Promise<{ success: boolean }> => {
    await api.put<void>(`/config/customers/${customerId}/events/${eventName}/order-types/${orderType}`, data);
    return { success: true };
  },

  getCustomerSettings: async (customerId: string): Promise<CustomerNotificationMapping> => {
    const response = await api.get<CustomerNotificationMapping>(`/config/customers/${customerId}`);
    return response.data;
  },

  saveCustomerSettings: async (customerId: string, data: CustomerNotificationMapping): Promise<{ success: boolean }> => {
    await api.put<void>(`/config/customers/${customerId}`, data);
    return { success: true };
  },
};
