import axios from 'axios';
import { ContentVariables } from '../types';

const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_BASE_URL || '/api'}/config`,
  headers: { 'Content-Type': 'application/json' },
});

export const contentVariableService = {
  getContentVariables: async (): Promise<ContentVariables> => {
    const response = await api.get('/content-variables');
    return response.data;
  },

  saveContentVariables: async (data: ContentVariables): Promise<void> => {
    await api.put('/content-variables', data);
  },
};
