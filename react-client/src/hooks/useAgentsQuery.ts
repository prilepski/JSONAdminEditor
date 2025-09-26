import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import { agentsService } from '../services/agentsService';
import { useErrorHandler } from './useErrorHandler';

export const useAgentsQuery = () => {
  return useQuery({
    queryKey: ['agents'],
    queryFn: agentsService.getAgents,
  });
};

export const useAgentsMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'Agents' });

  return useMutation({
    mutationFn: agentsService.saveAgents,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['agents'] });
      toast.success('Agents settings saved successfully');
    },
    onError: (error) => {
      handleError(error, 'Failed to save agents settings');
    },
  });
};