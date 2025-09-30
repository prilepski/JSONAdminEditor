import axios from 'axios';
import { createApiError, createServiceError } from '../utils/errorHandler';
import { ApiResponse, OptOutConfig } from '../types';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const optOutService = {
  getOptOutConfig: async (): Promise<OptOutConfig> => {
    try {
      const response = await api.get<OptOutConfig>('/config/opt-out');
      return response.data || {};
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          '/config/opt-out',
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load opt-out configuration',
        'optOutService',
        'getOptOutConfig'
      );
    }
  },

  saveOptOutConfig: async (config: OptOutConfig): Promise<ApiResponse> => {
    try {
      await api.put<void>('/config/opt-out', config);
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          '/config/opt-out',
          'PUT'
        );
      }
      throw createServiceError(
        'Failed to save opt-out configuration',
        'optOutService',
        'saveOptOutConfig'
      );
    }
  },
};