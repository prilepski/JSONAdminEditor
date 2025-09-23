import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import { preferredCommunicationService } from '../services';
import { useErrorHandler } from './useErrorHandler';

export const usePreferredCommunicationQuery = () => {
  return useQuery({
    queryKey: ['preferredCommunication'],
    queryFn: preferredCommunicationService.getPreferredCommunication,
  });
};

export const usePreferredCommunicationMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'PreferredCommunication' });

  return useMutation({
    mutationFn: preferredCommunicationService.savePreferredCommunication,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['preferredCommunication'] });
      toast.success('Preferred communication settings saved successfully');
    },
    onError: (error) => {
      handleError(error, 'Failed to save preferred communication settings');
    },
  });
};
