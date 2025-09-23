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

export const useTemplatesQuery = () => {
  return useQuery({
    queryKey: ['templates'],
    queryFn: eventService.getAvailableTemplates,
  });
};

export const useAllEventsQuery = () => {
  return useQuery({
    queryKey: ['allEvents'],
    queryFn: eventService.getAllEvents,
  });
};

export const useEventQuery = (eventName: string, orderType: string) => {
  return useQuery({
    queryKey: ['event', eventName, orderType],
    queryFn: () => eventService.getEventByNameAndOrderType(eventName, orderType),
    enabled: !!eventName && !!orderType,
  });
};

export const useEventMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ eventName, orderType, eventData }: {
      eventName: string;
      orderType: string;
      eventData: any;
    }) => eventService.saveEvent(eventName, orderType, eventData),
    onSuccess: (_, { eventName, orderType }) => {
      queryClient.invalidateQueries({ queryKey: ['event', eventName, orderType] });
      queryClient.invalidateQueries({ queryKey: ['allEvents'] });
    },
  });
};

export const useEventDeleteMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ eventName, orderType }: { eventName: string; orderType: string }) => 
      eventService.deleteEvent(eventName, orderType),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['allEvents'] });
    },
  });
};
