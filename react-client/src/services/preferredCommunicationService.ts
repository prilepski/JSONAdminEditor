import axios from 'axios';
import { components } from '../generated/api';

type PreferredCommunication = components['schemas']['PreferredCommunication'];

const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_BASE_URL || '/api'}/config`,
  headers: { 'Content-Type': 'application/json' },
});

export const preferredCommunicationService = {
  getPreferredCommunication: async (): Promise<PreferredCommunication[]> => {
    const response = await api.get('/preferred-communication');
    return response.data;
  },

  savePreferredCommunication: async (data: PreferredCommunication[]): Promise<void> => {
    await api.put('/preferred-communication', data);
  },
};
