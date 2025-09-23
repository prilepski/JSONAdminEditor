import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import { eventService } from '../services';
import { useErrorHandler } from './useErrorHandler';

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
  const { handleError } = useErrorHandler({ context: 'EventMutation' });

  return useMutation({
    mutationFn: ({ eventName, orderType, eventData }: {
      eventName: string;
      orderType: string;
      eventData: any;
    }) => eventService.saveEvent(eventName, orderType, eventData),
    onSuccess: (_, { eventName, orderType }) => {
      queryClient.invalidateQueries({ queryKey: ['event', eventName, orderType] });
      queryClient.invalidateQueries({ queryKey: ['allEvents'] });
      toast.success(`Event '${eventName}' saved successfully`);
    },
    onError: (error) => {
      handleError(error, 'Failed to save event');
    },
  });
};

export const useEventDeleteMutation = () => {
  const queryClient = useQueryClient();
  const { handleError } = useErrorHandler({ context: 'EventDelete' });

  return useMutation({
    mutationFn: ({ eventName, orderType }: { eventName: string; orderType: string }) => 
      eventService.deleteEvent(eventName, orderType),
    onSuccess: (_, { eventName }) => {
      queryClient.invalidateQueries({ queryKey: ['allEvents'] });
      toast.success(`Event '${eventName}' deleted successfully`);
    },
    onError: (error) => {
      handleError(error, 'Failed to delete event');
    },
  });
};
