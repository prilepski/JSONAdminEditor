import { useQuery } from '@tanstack/react-query';
import axios from 'axios';

export const useEventTriggersQuery = () => {
  return useQuery({
    queryKey: ['eventTriggers'],
    queryFn: async () => {
      const response = await axios.get('/api/dictionaries/event-triggers');
      return response.data;
    }
  });
};

export const useOrderTypesQuery = () => {
  return useQuery({
    queryKey: ['orderTypes'],
    queryFn: async () => {
      const response = await axios.get('/api/dictionaries/order-types');
      return response.data;
    }
  });
};