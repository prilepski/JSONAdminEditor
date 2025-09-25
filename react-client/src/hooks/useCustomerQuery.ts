import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import { customerService } from '../services';
import { CustomerNotificationMapping, CustomerEventMapping } from '../types';
import { useErrorHandler } from './useErrorHandler';

export const useCustomersQuery = () => {
  return useQuery({
    queryKey: ['customers'],
    queryFn: customerService.getCustomers,
  });
};

export const useCustomerEventsQuery = (customerId: string, orderType?: string) => {
  return useQuery({
    queryKey: ['customerEvents', customerId, orderType],
    queryFn: () => customerService.getCustomerEvents(customerId),
    enabled: !!customerId,
  });
};

export const useCustomerContentVariablesQuery = (customerId: string) => {
  return useQuery({
    queryKey: ['customerContentVariables', customerId],
    queryFn: () => customerService.getCustomerContentVariables(customerId),
    enabled: !!customerId,
  });
};

export const useCustomerSettingsQuery = (customerId: string) => {
  return useQuery({
    queryKey: ['customerSettings', customerId],
    queryFn: () => customerService.getCustomerSettings(customerId),
    enabled: !!customerId,
  });
};

export const useCustomerEventQuery = (customerId: string, eventName: string, orderType: string) => {
  return useQuery({
    queryKey: ['customerEvent', customerId, eventName, orderType],
    queryFn: () => customerService.getCustomerEvent(customerId, eventName, orderType),
    enabled: !!customerId && !!eventName && !!orderType,
    retry: false, // Don't retry on 404
  });
};

export const useCustomerEventMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'CustomerEvents' });

  return useMutation({
    mutationFn: ({ customerId, eventName, orderType, data }: { customerId: string; eventName: string; orderType: string; data: CustomerEventMapping }) =>
      customerService.saveCustomerEvent(customerId, eventName, orderType, data),
    onSuccess: (_, { customerId, eventName }) => {
      queryClient.invalidateQueries({ queryKey: ['customerEvents', customerId] });
      queryClient.invalidateQueries({ queryKey: ['customerEvent', customerId, eventName] });
      toast.success(`Customer event '${eventName}' saved successfully`);
    },
    onError: (error) => {
      handleError(error, 'Failed to save customer event');
    },
  });
};

export const useCustomerEventDeleteMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'CustomerEvents' });

  return useMutation({
    mutationFn: ({ customerId, eventName, orderType }: { customerId: string; eventName: string; orderType: string }) =>
      customerService.deleteCustomerEvent(customerId, eventName, orderType),
    onSuccess: (_, { customerId, eventName }) => {
      queryClient.invalidateQueries({ queryKey: ['customerEvents', customerId] });
      toast.success(`Customer event '${eventName}' deleted successfully`);
    },
    onError: (error) => {
      handleError(error, 'Failed to delete customer event');
    },
  });
};

export const useCustomerContentVariablesMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'CustomerContentVariables' });

  return useMutation({
    mutationFn: ({ customerId, data }: { customerId: string; data: Record<string, string> }) =>
      customerService.saveCustomerContentVariables(customerId, data),
    onSuccess: (_, { customerId }) => {
      queryClient.invalidateQueries({ queryKey: ['customerContentVariables', customerId] });
      toast.success(`Customer content variables for '${customerId}' saved successfully`);
    },
    onError: (error) => {
      handleError(error, 'Failed to save customer content variables');
    },
  });
};

export const useCustomerSettingsMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'CustomerSettings' });

  return useMutation({
    mutationFn: ({ customerId, data }: { customerId: string; data: CustomerNotificationMapping }) =>
      customerService.saveCustomerSettings(customerId, data),
    onSuccess: (_, { customerId }) => {
      queryClient.invalidateQueries({ queryKey: ['customerSettings', customerId] });
      toast.success(`Customer settings for '${customerId}' saved successfully`);
    },
    onError: (error) => {
      handleError(error, 'Failed to save customer settings');
    },
  });
};
