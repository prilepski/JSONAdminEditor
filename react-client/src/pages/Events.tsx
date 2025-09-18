import React, { useState, useEffect } from 'react';
import {
  useEventTriggersQuery,
  useOrderTypesQuery,
  useTemplatesQuery,
  useEventQuery,
  useEventSupportQuery,
  useEventMutation,
} from '../hooks/useEventQuery';
import toast from 'react-hot-toast';
import { PageHeader, TabNavigation, SaveButton, LoadingSpinner } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { EventSelector } from '../components/events/EventSelector';
import { EventDataForm } from '../components/events/EventDataForm';
import { TemplateSelectionForm } from '../components/events/TemplateSelectionForm';
import { ContentVariablesForm } from '../components/events/ContentVariablesForm';

interface EventData {
  Event?: string;
  OrderType?: string;
  Phone?: string;
  Email?: string;
  Logo?: string;
  IsSuppressed?: boolean;
  Templates?: {
    Email?: string;
    Sms?: string;
    Voice?: string;
  };
  ContentVariables?: Record<string, string>;
}

export const Events: React.FC = () => {
  const [selectedEvent, setSelectedEvent] = useState<string>('');
  const [selectedOrderType, setSelectedOrderType] = useState<string>('Delivery');
  const [activeTab, setActiveTab] = useState<string>('event-data');
  const [isNewEvent, setIsNewEvent] = useState(false);
  const [eventData, setEventData] = useState<EventData | null>(null);
  const { data: activeEventTriggers = [], isLoading: triggersLoading } = useEventTriggersQuery();
  const { data: availableOrderTypes = [], isLoading: orderTypesLoading } = useOrderTypesQuery();
  const { data: availableTemplates = [], isLoading: templatesLoading } = useTemplatesQuery();
  const [eventSupportsByOrderType, setEventSupportsByOrderType] = useState(false);

  const { data: eventSupports = false, isLoading: supportsLoading } = useEventSupportQuery(selectedEvent);
  const { data: eventInfo, isLoading: eventLoading } = useEventQuery(selectedEvent, selectedOrderType);
  const eventMutation = useEventMutation();

  useEffect(() => {
    setEventSupportsByOrderType(eventSupports);
    if (eventInfo && Object.keys(eventInfo).length > 0) {
      setEventData(eventInfo);
      setIsNewEvent(false);
    } else if (selectedEvent) {
      setIsNewEvent(true);
      setEventData({
        Event: selectedEvent,
        OrderType: selectedOrderType,
        Phone: '$consigneeContact.phone$',
        Email: '$consigneeContact.email$',
        Logo: 'base64',
        IsSuppressed: false,
        Templates: { Email: '', Sms: '', Voice: '' },
        ContentVariables: {},
      });
    } else {
      setEventData(null);
      setIsNewEvent(false);
    }
  }, [selectedEvent, selectedOrderType, eventSupports, eventInfo]);

  const handleSaveEventData = async () => {
    if (!eventData || !selectedEvent) return;

    try {
      const success = await eventMutation.mutateAsync({
        eventName: selectedEvent,
        orderType: selectedOrderType,
        eventData,
        isNew: isNewEvent,
      });

      if (success) {
        toast.success(`Event '${selectedEvent}' ${isNewEvent ? 'added' : 'updated'} successfully!`);
        setIsNewEvent(false);
      } else {
        toast.error('Failed to save event data');
      }
    } catch (error) {
      toast.error('Error saving event data');
    }
  };

  const updateEventField = (field: string, value: any) => {
    setEventData((prev) => (prev ? { ...prev, [field]: value } : null));
  };

  const updateTemplateField = (channel: string, value: string) => {
    setEventData((prev) =>
      prev
        ? {
            ...prev,
            Templates: { ...prev.Templates, [channel]: value },
          }
        : null
    );
  };

  const updateContentVariables = (variables: Record<string, string>) => {
    setEventData((prev) => (prev ? { ...prev, ContentVariables: variables } : null));
  };

  const tabs = [
    { id: 'event-data', label: 'Event Data', icon: 'fa-cog' },
    { id: 'templates', label: 'Templates', icon: 'fa-file-alt' },
    { id: 'content-variables', label: 'Content Variables', icon: 'fa-code' },
  ];

  const isInitialLoading = triggersLoading || orderTypesLoading || templatesLoading;
  const isEventDataLoading = selectedEvent && (supportsLoading || eventLoading);

  return (
    <PageErrorBoundary pageName="Events">
      <PageHeader icon="fa-calendar-alt" title="Event Management" />

      {isInitialLoading ? (
        <LoadingSpinner text="Loading event data..." />
      ) : (
        <ComponentErrorBoundary componentName="Event Selector">
          <EventSelector
            selectedEvent={selectedEvent}
            selectedOrderType={selectedOrderType}
            eventTriggers={activeEventTriggers}
            orderTypes={availableOrderTypes}
            eventSupportsByOrderType={eventSupportsByOrderType}
            onEventChange={setSelectedEvent}
            onOrderTypeChange={setSelectedOrderType}
          />
        </ComponentErrorBoundary>
      )}

      {selectedEvent && (
        <div className="card">
          <div className="card-header">
            <div className="d-flex justify-content-between align-items-center">
              <h3 className="mb-0">
                <i className="fas fa-edit me-2"></i>Edit Event: {selectedEvent}
              </h3>
              {eventData && (
                <SaveButton
                  onClick={handleSaveEventData}
                  loading={eventMutation.isPending}
                  text="Save Changes"
                />
              )}
            </div>
          </div>
          <div className="card-body">
            {isEventDataLoading ? (
              <LoadingSpinner text="Loading event configuration..." />
            ) : eventData ? (
              <>
                <TabNavigation tabs={tabs} activeTab={activeTab} onTabChange={setActiveTab} />

                {activeTab === 'event-data' && (
                  <ComponentErrorBoundary componentName="Event Data Form">
                    <EventDataForm eventData={eventData} onUpdate={updateEventField} />
                  </ComponentErrorBoundary>
                )}

                {activeTab === 'templates' && (
                  <ComponentErrorBoundary componentName="Template Selection">
                    <TemplateSelectionForm
                      templates={eventData.Templates || {}}
                      availableTemplates={availableTemplates}
                      onUpdate={updateTemplateField}
                    />
                  </ComponentErrorBoundary>
                )}

                {activeTab === 'content-variables' && (
                  <ComponentErrorBoundary componentName="Content Variables">
                    <ContentVariablesForm
                      contentVariables={eventData.ContentVariables || {}}
                      onUpdate={updateContentVariables}
                    />
                  </ComponentErrorBoundary>
                )}
              </>
            ) : (
              <div className="text-center py-4">
                <i className="fas fa-info-circle fa-2x text-muted mb-3"></i>
                <p className="text-muted">Select an event to start editing</p>
              </div>
            )}
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};
