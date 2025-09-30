import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import { optOutService } from '../services/optOutService';
import { useErrorHandler } from './useErrorHandler';

export const useOptOutQuery = () => {
  return useQuery({
    queryKey: ['optOut'],
    queryFn: optOutService.getOptOutConfig,
  });
};

export const useOptOutMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'OptOut' });

  return useMutation({
    mutationFn: optOutService.saveOptOutConfig,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['optOut'] });
      toast.success('Opt-out configuration saved successfully');
    },
    onError: (error) => {
      handleError(error, 'Failed to save opt-out configuration');
    },
  });
};