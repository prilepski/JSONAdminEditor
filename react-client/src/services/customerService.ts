import axios from 'axios';
import { EventMapping, CustomerNotificationMapping, ContentVariableItem } from '../types';
import { createApiError, createServiceError } from '../utils/errorHandler';

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

  getCustomerContentVariables: async (customerId: string): Promise<Record<string, string>> => {
    try {
      const response = await api.get<ContentVariableItem[]>(`/config/customers/${customerId}/content-variables`);
      const data = response.data || [];
      return data.reduce((acc, item) => ({ ...acc, [item.key]: item.value }), {});
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/content-variables`,
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load customer content variables',
        'customerService',
        'getCustomerContentVariables'
      );
    }
  },

  saveCustomerContentVariables: async (customerId: string, data: Record<string, string>): Promise<{ success: boolean }> => {
    try {
      await api.put<void>(`/config/customers/${customerId}/content-variables`, data);
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/content-variables`,
          'PUT'
        );
      }
      throw createServiceError(
        'Failed to save customer content variables',
        'customerService',
        'saveCustomerContentVariables'
      );
    }
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
