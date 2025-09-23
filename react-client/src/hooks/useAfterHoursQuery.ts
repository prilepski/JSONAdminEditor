import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import { afterHoursService } from '../services/afterHoursService';
import { useErrorHandler } from './useErrorHandler';

export const useAfterHoursQuery = () => {
  return useQuery({
    queryKey: ['afterHours'],
    queryFn: afterHoursService.getAfterHours,
  });
};

export const useAfterHoursMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'AfterHours' });

  return useMutation({
    mutationFn: afterHoursService.saveAfterHours,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['afterHours'] });
      toast.success('After hours settings saved successfully');
    },
    onError: (error) => {
      handleError(error, 'Failed to save after hours settings');
    },
  });
};