import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { preferredCommunicationService } from '../services';
import { components } from '../generated/api';

type PreferredCommunication = components['schemas']['PreferredCommunication'];

export const usePreferredCommunicationQuery = () => {
  return useQuery({
    queryKey: ['preferredCommunication'],
    queryFn: preferredCommunicationService.getPreferredCommunication,
  });
};

export const usePreferredCommunicationMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: preferredCommunicationService.savePreferredCommunication,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['preferredCommunication'] });
    },
  });
};
