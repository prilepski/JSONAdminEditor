import axios from 'axios';

export const optOutService = {
  getOptOutConfig: async () => {
    const response = await axios.get('/api/config/opt-out');
    return response.data;
  },

  saveOptOutConfig: async (data: any) => {
    const response = await axios.post('/api/config/opt-out', data);
    return response.data;
  },
};