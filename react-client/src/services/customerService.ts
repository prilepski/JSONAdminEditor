import axios from 'axios';
import { EventMapping, CustomerNotificationMapping, CustomerEventMapping } from '../types';
import { createApiError, createServiceError } from '../utils/errorHandler';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const customerService = {
  getCustomers: async (): Promise<string[]> => {
    try {
      const response = await api.get<Array<{ customerId: string }>>('/dictionaries/customers');
      return response.data.map(customer => customer.customerId);
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          '/dictionaries/customers',
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load customers',
        'customerService',
        'getCustomers'
      );
    }
  },

  getCustomerEvents: async (customerId: string): Promise<EventMapping[]> => {
    try {
      const response = await api.get<EventMapping[]>(`/config/customers/${customerId}/events`);
      return response.data || [];
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/events`,
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load customer events',
        'customerService',
        'getCustomerEvents'
      );
    }
  },

  getCustomerEvent: async (customerId: string, eventName: string, orderType: string): Promise<EventMapping | null> => {
    try {
      const response = await api.get<EventMapping>(`/config/customers/${customerId}/events/${eventName}/order-types/${orderType}`);
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 404) {
        return null; // Event not found, return null instead of throwing
      }
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/events/${eventName}/order-types/${orderType}`,
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load customer event',
        'customerService',
        'getCustomerEvent'
      );
    }
  },

  saveCustomerEvent: async (customerId: string, eventName: string, orderType: string, data: CustomerEventMapping): Promise<{ success: boolean }> => {
    try {
      await api.put<void>(`/config/customers/${customerId}/events/${eventName}/order-types/${orderType}`, data);
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/events/${eventName}/order-types/${orderType}`,
          'PUT'
        );
      }
      throw createServiceError(
        'Failed to save customer event',
        'customerService',
        'saveCustomerEvent'
      );
    }
  },

  deleteCustomerEvent: async (customerId: string, eventName: string, orderType: string): Promise<{ success: boolean }> => {
    try {
      await api.delete<void>(`/config/customers/${customerId}/events/${eventName}/order-types/${orderType}`);
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/events/${eventName}/order-types/${orderType}`,
          'DELETE'
        );
      }
      throw createServiceError(
        'Failed to delete customer event',
        'customerService',
        'deleteCustomerEvent'
      );
    }
  },

  getCustomerContentVariables: async (customerId: string): Promise<Record<string, string>> => {
    try {
      const response = await api.get<Array<{ key: string; value: string }>>(`/config/customers/${customerId}/content-variables`);
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
