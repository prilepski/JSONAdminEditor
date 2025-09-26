import axios from 'axios';
import { EventMapping, CustomerNotificationMapping, CustomerEventMapping, AfterHours2, PreferredCommunication, ApiResponse } from '../types';
import type { paths } from '../generated/api';

// Generated API types
type CustomerContentVariablesResponse = paths['/api/config/customers/{customerId}/content-variables']['get']['responses']['200']['content']['application/json'];
type CustomerContentVariablesRequest = paths['/api/config/customers/{customerId}/content-variables']['put']['requestBody']['content']['application/json'];
import { createApiError, createServiceError } from '../utils/errorHandler';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const customerService = {
  getCustomers: async (): Promise<Array<{ customerId: string; companyName: string }>> => {
    try {
      const response = await api.get<Array<{ customerId: string; companyName: string }>>('/dictionaries/customers');
      return response.data;
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

  getCustomerEvent: async (customerId: string, eventName: string, orderType: string): Promise<CustomerEventMapping | null> => {
    try {
      const response = await api.get<CustomerEventMapping>(`/config/customers/${customerId}/events/${eventName}/order-types/${orderType}`);
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

  getCustomerContentVariables: async (customerId: string): Promise<CustomerContentVariablesResponse> => {
    try {
      const response = await api.get<CustomerContentVariablesResponse>(`/config/customers/${customerId}/content-variables`);
      return response.data || {};
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

  saveCustomerContentVariables: async (customerId: string, data: CustomerContentVariablesRequest): Promise<{ success: boolean }> => {
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

  getCustomerAfterHours: async (customerId: string): Promise<AfterHours2> => {
    try {
      const response = await api.get<AfterHours2>(`/config/customers/${customerId}/after-hours`);
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/after-hours`,
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load customer after hours settings',
        'customerService',
        'getCustomerAfterHours'
      );
    }
  },

  saveCustomerAfterHours: async (customerId: string, data: AfterHours2): Promise<{ success: boolean }> => {
    try {
      await api.put<void>(`/config/customers/${customerId}/after-hours`, data);
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/after-hours`,
          'PUT'
        );
      }
      throw createServiceError(
        'Failed to save customer after hours settings',
        'customerService',
        'saveCustomerAfterHours'
      );
    }
  },

  getCustomerPreferredCommunication: async (customerId: string): Promise<PreferredCommunication[]> => {
    try {
      const response = await api.get<PreferredCommunication[]>(`/config/customers/${customerId}/preferred-communication`);
      return response.data || [];
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/preferred-communication`,
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load customer preferred communication',
        'customerService',
        'getCustomerPreferredCommunication'
      );
    }
  },

  saveCustomerPreferredCommunication: async (customerId: string, data: PreferredCommunication[]): Promise<{ success: boolean }> => {
    try {
      await api.put<void>(`/config/customers/${customerId}/preferred-communication`, data);
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/preferred-communication`,
          'PUT'
        );
      }
      throw createServiceError(
        'Failed to save customer preferred communication',
        'customerService',
        'saveCustomerPreferredCommunication'
      );
    }
  },

  getCustomerFromEmail: async (customerId: string): Promise<string> => {
    try {
      const response = await api.get<string>(`/config/customers/${customerId}/from-email`);
      return response.data || '';
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/from-email`,
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load customer from email',
        'customerService',
        'getCustomerFromEmail'
      );
    }
  },

  saveCustomerFromEmail: async (customerId: string, email: string): Promise<ApiResponse> => {
    try {
      await api.put<void>(`/config/customers/${customerId}/from-email`, email);
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/config/customers/${customerId}/from-email`,
          'PUT'
        );
      }
      throw createServiceError(
        'Failed to save customer from email',
        'customerService',
        'saveCustomerFromEmail'
      );
    }
  },
};
