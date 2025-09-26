import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import { customerService } from '../services';
import { CustomerNotificationMapping, CustomerEventMapping, AfterHours2, PreferredCommunication } from '../types';
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

export const useCustomerAfterHoursQuery = (customerId: string) => {
  return useQuery({
    queryKey: ['customerAfterHours', customerId],
    queryFn: () => customerService.getCustomerAfterHours(customerId),
    enabled: !!customerId,
  });
};

export const useCustomerAfterHoursMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'CustomerAfterHours' });

  return useMutation({
    mutationFn: ({ customerId, data }: { customerId: string; data: AfterHours2 }) =>
      customerService.saveCustomerAfterHours(customerId, data),
    onSuccess: (_, { customerId }) => {
      queryClient.invalidateQueries({ queryKey: ['customerAfterHours', customerId] });
      toast.success(`Customer after hours settings saved successfully`);
    },
    onError: (error) => {
      handleError(error, 'Failed to save customer after hours settings');
    },
  });
};

export const useCustomerPreferredCommunicationQuery = (customerId: string) => {
  return useQuery({
    queryKey: ['customerPreferredCommunication', customerId],
    queryFn: () => customerService.getCustomerPreferredCommunication(customerId),
    enabled: !!customerId,
  });
};

export const useCustomerPreferredCommunicationMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'CustomerPreferredCommunication' });

  return useMutation({
    mutationFn: ({ customerId, data }: { customerId: string; data: PreferredCommunication[] }) =>
      customerService.saveCustomerPreferredCommunication(customerId, data),
    onSuccess: (_, { customerId }) => {
      queryClient.invalidateQueries({ queryKey: ['customerPreferredCommunication', customerId] });
      toast.success(`Customer preferred communication settings saved successfully`);
    },
    onError: (error) => {
      handleError(error, 'Failed to save customer preferred communication settings');
    },
  });
};

export const useCustomerFromEmailQuery = (customerId: string) => {
  return useQuery({
    queryKey: ['customerFromEmail', customerId],
    queryFn: () => customerService.getCustomerFromEmail(customerId),
    enabled: !!customerId,
  });
};

export const useCustomerFromEmailMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'CustomerFromEmail' });

  return useMutation({
    mutationFn: ({ customerId, email }: { customerId: string; email: string }) =>
      customerService.saveCustomerFromEmail(customerId, email),
    onSuccess: (_, { customerId }) => {
      queryClient.invalidateQueries({ queryKey: ['customerFromEmail', customerId] });
      toast.success(`Customer from email saved successfully`);
    },
    onError: (error) => {
      handleError(error, 'Failed to save customer from email');
    },
  });
};

export const useCustomerContentVariablesOverridesQuery = (customerId: string) => {
  return useQuery({
    queryKey: ['customerContentVariablesOverrides', customerId],
    queryFn: () => customerService.getCustomerContentVariablesOverrides(customerId),
    enabled: !!customerId,
  });
};

export const useCustomerContentVariablesOverridesMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'CustomerContentVariablesOverrides' });

  return useMutation({
    mutationFn: ({ customerId, data }: { customerId: string; data: Record<string, Record<string, Record<string, string>>> }) =>
      customerService.saveCustomerContentVariablesOverrides(customerId, data),
    onSuccess: (_, { customerId }) => {
      queryClient.invalidateQueries({ queryKey: ['customerContentVariablesOverrides', customerId] });
      toast.success(`Customer content variables overrides saved successfully`);
    },
    onError: (error) => {
      handleError(error, 'Failed to save customer content variables overrides');
    },
  });
};
