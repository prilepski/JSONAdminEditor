import axios from 'axios';
import { ApiResponse, TableData } from '../types';

const api = axios.create({
  baseURL: '/api/content-variables',
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
