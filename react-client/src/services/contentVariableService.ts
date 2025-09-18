import axios from 'axios';
import { ApiResponse, TableData } from '../types';

const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_BASE_URL || '/api'}/content-variables`,
  headers: { 'Content-Type': 'application/json' },
});

export const contentVariableService = {
  getContentVariables: async (): Promise<TableData[]> => {
    const response = await api.get('/');
    return response.data;
  },

  saveContentVariables: async (data: TableData[]): Promise<ApiResponse> => {
    const response = await api.post('/save', { data });
    return response.data;
  },
};
