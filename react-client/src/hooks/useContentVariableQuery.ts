import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { contentVariableService } from '../services';

export const useContentVariablesQuery = () => {
  return useQuery({
    queryKey: ['contentVariables'],
    queryFn: contentVariableService.getContentVariables,
  });
};

export const useContentVariablesMutation = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: contentVariableService.saveContentVariables,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contentVariables'] });
    },
  });
};