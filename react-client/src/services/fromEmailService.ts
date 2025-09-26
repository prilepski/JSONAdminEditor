import axios from 'axios';
import { createApiError, createServiceError } from '../utils/errorHandler';
import { ApiResponse } from '../types';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const fromEmailService = {
  getFromEmail: async (): Promise<string> => {
    try {
      const response = await api.get<string>('/config/from-email');
      return response.data || '';
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          '/config/from-email',
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load from email',
        'fromEmailService',
        'getFromEmail'
      );
    }
  },

  saveFromEmail: async (email: string): Promise<ApiResponse> => {
    try {
      await api.put<void>('/config/from-email', email);
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          '/config/from-email',
          'PUT'
        );
      }
      throw createServiceError(
        'Failed to save from email',
        'fromEmailService',
        'saveFromEmail'
      );
    }
  },
};