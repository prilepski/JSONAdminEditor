import React, { useState, useEffect } from 'react';
import { useFormState } from '../hooks/useFormState';
import {
  useEventTriggersQuery,
  useOrderTypesQuery,
  useTemplatesQuery,
  useEventQuery,
  useEventSupportQuery,
  useEventMutation,
} from '../hooks/useEventQuery';
import { EventTrigger, OrderType, Template } from '../types';
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
  const { state: pageState, updateField } = useFormState({
    selectedEvent: '',
    selectedOrderType: 'Delivery',
    activeTab: 'event-data',
    isNewEvent: false,
  });

  const [eventData, setEventData] = useState<EventData | null>(null);
  const { selectedEvent, selectedOrderType, activeTab, isNewEvent } = pageState;
  const { data: activeEventTriggers = [] as EventTrigger[], isLoading: triggersLoading } = useEventTriggersQuery();
  const { data: availableOrderTypes = [] as OrderType[], isLoading: orderTypesLoading } = useOrderTypesQuery();
  const { data: availableTemplates = [] as Template[], isLoading: templatesLoading } = useTemplatesQuery();

  const { data: eventSupports = false, isLoading: supportsLoading } =
    useEventSupportQuery(selectedEvent);
  const { data: eventInfo, isLoading: eventLoading } = useEventQuery(
    selectedEvent,
    selectedOrderType
  );
  const eventMutation = useEventMutation();

  useEffect(() => {
    if (eventInfo && Object.keys(eventInfo).length > 0) {
      setEventData(eventInfo);
      updateField('isNewEvent', false);
    } else if (selectedEvent) {
      updateField('isNewEvent', true);
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
      updateField('isNewEvent', false);
    }
  }, [selectedEvent, selectedOrderType, eventSupports, eventInfo, updateField]);

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
        updateField('isNewEvent', false);
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
            eventSupportsByOrderType={eventSupports}
            onEventChange={(event) => updateField('selectedEvent', event)}
            onOrderTypeChange={(orderType) => updateField('selectedOrderType', orderType)}
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
                <TabNavigation
                  tabs={tabs}
                  activeTab={activeTab}
                  onTabChange={(tab) => updateField('activeTab', tab)}
                />

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
