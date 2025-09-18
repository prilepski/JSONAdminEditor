import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { FileType } from '../types';
import { dictionaryService } from '../services';

export const useDictionaryQuery = (fileType: FileType) => {
  return useQuery({
    queryKey: ['dictionary', fileType],
    queryFn: () => dictionaryService.getData(fileType),
    enabled: fileType !== FileType.None,
  });
};

export const useDictionaryMutation = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ filePath, jsonData, fileType }: { filePath: string; jsonData: Record<string, any>[]; fileType: FileType }) =>
      dictionaryService.save(filePath, jsonData, fileType),
    onSuccess: (_, { fileType }) => {
      queryClient.invalidateQueries({ queryKey: ['dictionary', fileType] });
    },
  });
};

export const useUploadMutation = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: dictionaryService.upload,
    onSuccess: (_, { fileType }) => {
      queryClient.invalidateQueries({ queryKey: ['dictionary', fileType] });
    },
  });
};

export const useChannelOptionsQuery = () => {
  return useQuery({
    queryKey: ['channelOptions'],
    queryFn: dictionaryService.getChannelOptions,
  });
};