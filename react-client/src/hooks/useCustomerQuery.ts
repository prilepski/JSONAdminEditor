import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { customerService } from '../services';

export const useCustomersQuery = () => {
  return useQuery({
    queryKey: ['customers'],
    queryFn: customerService.getCustomers,
  });
};

export const useCustomerEventsQuery = (customerId: string) => {
  return useQuery({
    queryKey: ['customerEvents', customerId],
    queryFn: () => customerService.getCustomerEvents(customerId),
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

export const useCustomerEventsMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ customerId, data }: { customerId: string; data: any }) =>
      customerService.saveCustomerEvents(customerId, data),
    onSuccess: (_, { customerId }) => {
      queryClient.invalidateQueries({ queryKey: ['customerEvents', customerId] });
    },
  });
};

export const useCustomerSettingsMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ customerId, data }: { customerId: string; data: any }) =>
      customerService.saveCustomerSettings(customerId, data),
    onSuccess: (_, { customerId }) => {
      queryClient.invalidateQueries({ queryKey: ['customerSettings', customerId] });
    },
  });
};
