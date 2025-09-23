import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import { FileType } from '../types';
import { dictionaryService } from '../services';
import { useErrorHandler } from './useErrorHandler';

export const useDictionaryQuery = (fileType: FileType) => {
  return useQuery({
    queryKey: ['dictionary', fileType],
    queryFn: () => dictionaryService.getData(fileType),
    enabled: fileType !== FileType.None,
  });
};

export const useDictionaryMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'DictionaryMutation' });

  return useMutation({
    mutationFn: ({
      filePath,
      jsonData,
      fileType,
    }: {
      filePath: string;
      jsonData: Record<string, any>[];
      fileType: FileType;
    }) => dictionaryService.save(filePath, jsonData, fileType),
    onSuccess: (_, { fileType }) => {
      queryClient.invalidateQueries({ queryKey: ['dictionary', fileType] });
      toast.success('Dictionary saved successfully');
    },
    onError: (error) => {
      handleError(error, 'Failed to save dictionary');
    },
  });
};

export const useUploadMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'FileUpload' });

  return useMutation({
    mutationFn: dictionaryService.upload,
    onSuccess: (_, { fileType }) => {
      queryClient.invalidateQueries({ queryKey: ['dictionary', fileType] });
      toast.success('File uploaded successfully');
    },
    onError: (error) => {
      handleError(error, 'Failed to upload file');
    },
  });
};

export const useChannelOptionsQuery = () => {
  return useQuery({
    queryKey: ['channelOptions'],
    queryFn: dictionaryService.getChannelOptions,
  });
};
