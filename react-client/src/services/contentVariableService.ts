import axios from 'axios';
import { ApiResponse } from '../types/api';

const api = axios.create({
  baseURL: '/api/content-variables',
  headers: { 'Content-Type': 'application/json' },
});

export const contentVariableService = {
  getContentVariables: async (): Promise<Record<string, any>[]> => {
    const response = await api.get('/');
    return response.data;
  },

  saveContentVariables: async (data: Record<string, any>[]): Promise<ApiResponse> => {
    const response = await api.post('/save', { data });
    return response.data;
  },
};