import axios from 'axios';
import { createApiError, createServiceError } from '../utils/errorHandler';
import { ApiResponse } from '../types';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const agentsService = {
  getAgents: async (): Promise<Record<string, boolean>> => {
    try {
      const response = await api.get<Record<string, boolean>>('/config/agents');
      return response.data || {};
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          '/config/agents',
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load agents',
        'agentsService',
        'getAgents'
      );
    }
  },

  saveAgents: async (agents: Record<string, boolean>): Promise<ApiResponse> => {
    try {
      await api.put<void>('/config/agents', agents);
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          '/config/agents',
          'PUT'
        );
      }
      throw createServiceError(
        'Failed to save agents',
        'agentsService',
        'saveAgents'
      );
    }
  },
};