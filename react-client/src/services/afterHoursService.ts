import axios from 'axios';
import { AfterHours2 } from '../types';

const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_BASE_URL || '/api'}/config`,
  headers: { 'Content-Type': 'application/json' },
});

export const afterHoursService = {
  getAfterHours: async (): Promise<AfterHours2> => {
    const response = await api.get('/after-hours');
    return response.data;
  },

  saveAfterHours: async (data: AfterHours2): Promise<void> => {
    await api.put('/after-hours', data);
  },
};