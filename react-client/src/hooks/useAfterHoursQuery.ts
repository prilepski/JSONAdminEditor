import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { afterHoursService } from '../services/afterHoursService';

export const useAfterHoursQuery = () => {
  return useQuery({
    queryKey: ['afterHours'],
    queryFn: afterHoursService.getAfterHours,
  });
};

export const useAfterHoursMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: afterHoursService.saveAfterHours,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['afterHours'] });
    },
  });
};