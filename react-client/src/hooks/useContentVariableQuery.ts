import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import { contentVariableService } from '../services';
import { useErrorHandler } from './useErrorHandler';

export const useContentVariablesQuery = () => {
  return useQuery({
    queryKey: ['contentVariables'],
    queryFn: contentVariableService.getContentVariables,
  });
};

export const useContentVariablesMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'ContentVariables' });

  return useMutation({
    mutationFn: contentVariableService.saveContentVariables,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contentVariables'] });
      toast.success('Content variables saved successfully');
    },
    onError: (error) => {
      handleError(error, 'Failed to save content variables');
    },
  });
};
