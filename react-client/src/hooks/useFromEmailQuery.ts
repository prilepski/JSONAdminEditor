import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { fromEmailService } from '../services/fromEmailService';

export const useFromEmailQuery = () => {
  return useQuery({
    queryKey: ['fromEmail'],
    queryFn: fromEmailService.getFromEmail,
  });
};

export const useFromEmailMutation = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: fromEmailService.saveFromEmail,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['fromEmail'] });
    },
  });
};