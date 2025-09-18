import axios from 'axios';
import { ApiResponse } from '../types/api';

const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_BASE_URL || '/api'}/preferred-communication`,
  headers: { 'Content-Type': 'application/json' },
});

export const preferredCommunicationService = {
  getPreferredCommunication: async (): Promise<Record<string, any>[]> => {
    const response = await api.get('/');
    return response.data;
  },

  savePreferredCommunication: async (data: Record<string, any>[]): Promise<ApiResponse> => {
    const response = await api.post('/save', { data });
    return response.data;
  },
};
