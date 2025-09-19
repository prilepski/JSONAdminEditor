import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { eventService } from '../services';

export const useEventTriggersQuery = () => {
  return useQuery({
    queryKey: ['eventTriggers'],
    queryFn: eventService.getActiveEventTriggers,
  });
};

export const useOrderTypesQuery = () => {
  return useQuery({
    queryKey: ['orderTypes'],
    queryFn: eventService.getAvailableOrderTypes,
  });
};

export const useTemplatesQuery = (orderType?: string) => {
  return useQuery({
    queryKey: ['templates', orderType],
    queryFn: () => eventService.getAvailableTemplates(orderType),
  });
};

export const useEventQuery = (eventName: string, orderType: string) => {
  return useQuery({
    queryKey: ['event', eventName, orderType],
    queryFn: () => eventService.getEventByNameAndOrderType(eventName, orderType),
    enabled: !!eventName,
  });
};

export const useEventSupportQuery = (eventName: string) => {
  return useQuery({
    queryKey: ['eventSupport', eventName],
    queryFn: () => eventService.checkEventSupportsByOrderType(eventName),
    enabled: !!eventName,
  });
};

export const useEventMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      eventName,
      orderType,
      eventData,
      isNew,
    }: {
      eventName: string;
      orderType: string;
      eventData: any;
      isNew: boolean;
    }) => eventService.saveEvent(eventName, orderType, eventData, isNew),
    onSuccess: (_, { eventName, orderType }) => {
      queryClient.invalidateQueries({ queryKey: ['event', eventName, orderType] });
      queryClient.invalidateQueries({ queryKey: ['eventTriggers'] });
    },
  });
};
