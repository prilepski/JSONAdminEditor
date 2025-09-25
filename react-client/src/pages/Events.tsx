import React, { useState, useEffect } from 'react';
import { useFormState } from '../hooks/useFormState';
import { useErrorHandler } from '../hooks/useErrorHandler';
import {
  useTemplatesQuery,
  useEventQuery,
  useAllEventsQuery,
  useEventMutation,
} from '../hooks/useEventQuery';
import { useContentVariablesQuery } from '../hooks/useContentVariableQuery';
import { Template } from '../types';
import { PageHeader, TabNavigation, SaveButton, LoadingSpinner } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { EventSelector } from '../components/events/EventSelector';
import { EventDataForm } from '../components/events/EventDataForm';
import { TemplateSelectionForm } from '../components/events/TemplateSelectionForm';
import { ContentVariablesForm } from '../components/events/ContentVariablesForm';
import { PreferredCommunicationForm } from '../components/events/PreferredCommunicationForm';
import { TriggerConditionsForm } from '../components/events/TriggerConditionsForm';

interface EventData {
  event: string;
  orderType: string;
  phone?: string;
  email?: string;
  templates?: Record<string, string>;
  isSuppressed?: boolean;
  preferredCommunication?: Array<{ channel: string; priority: number }>;
  contentVariables?: Record<string, string>;
  triggerConditions?: Record<string, boolean>;
  contentVariablesOverrides?: Record<string, Record<string, Record<string, string>>>;
}

export const Events: React.FC = () => {
  const { state: pageState, updateField } = useFormState({
    selectedEvent: '',
    selectedOrderType: 'Delivery',
    activeTab: 'event-data',
    isNewEvent: false,
  });
  const { handleError } = useErrorHandler({ context: 'Events' });

  const [eventData, setEventData] = useState<EventData | null>(null);
  const { selectedEvent, selectedOrderType, activeTab } = pageState;
  const { data: availableTemplates = [] as Template[], isLoading: templatesLoading } = useTemplatesQuery();
  const { data: allEvents = [] } = useAllEventsQuery();
  const { data: globalContentVariables = {} } = useContentVariablesQuery();
  const { data: eventInfo, isLoading: eventLoading } = useEventQuery(
    selectedEvent,
    selectedOrderType
  );
  const eventMutation = useEventMutation();

  useEffect(() => {
    if (eventInfo) {
      setEventData({
        ...eventInfo,
        preferredCommunication: (eventInfo.preferredCommunication as Array<{ channel: string; priority: number }>) || []
      });
      updateField('isNewEvent', false);
    } else if (selectedEvent && selectedOrderType) {
      updateField('isNewEvent', true);
      setEventData({
        event: selectedEvent,
        orderType: selectedOrderType,
        phone: '$consigneeContact.phone$',
        email: '$consigneeContact.email$',
        templates: {},
        isSuppressed: false,
        preferredCommunication: [] as Array<{ channel: string; priority: number }>,
        contentVariables: {},
        triggerConditions: {},
        contentVariablesOverrides: {},
      });
    } else {
      setEventData(null);
      updateField('isNewEvent', false);
    }
  }, [selectedEvent, selectedOrderType, eventInfo, updateField]);

  const handleSaveEventData = async () => {
    if (!eventData || !selectedEvent) return;

    try {
      await eventMutation.mutateAsync({
        eventName: selectedEvent,
        orderType: selectedOrderType,
        eventData,
      });
      
      updateField('isNewEvent', false);
    } catch (error) {
      handleError(error, 'Failed to save event data');
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
            templates: { ...prev.templates, [channel]: value },
          }
        : null
    );
  };

  const updateContentVariables = (variables: Record<string, string>) => {
    setEventData((prev) => (prev ? { ...prev, contentVariables: variables } : null));
  };

  const updatePreferredCommunication = (communication: Array<{ channel: string; priority: number }>) => {
    setEventData((prev) => (prev ? { ...prev, preferredCommunication: communication } : null));
  };

  const updateTriggerConditions = (conditions: Record<string, boolean>) => {
    setEventData((prev) => (prev ? { ...prev, triggerConditions: conditions } : null));
  };

  const tabs = [
    { id: 'event-data', label: 'Event Data', icon: 'fa-cog' },
    { id: 'templates', label: 'Templates', icon: 'fa-file-alt' },
    { id: 'content-variables', label: 'Content Variables', icon: 'fa-code' },
    { id: 'preferred-communication', label: 'Preferred Communication', icon: 'fa-comments' },
    { id: 'trigger-conditions', label: 'Trigger Conditions', icon: 'fa-filter' },
  ];

  const isInitialLoading = templatesLoading;
  const isEventDataLoading = selectedEvent && eventLoading;

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
            allEvents={allEvents}
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
                    <EventDataForm 
                      eventData={eventData} 
                      onUpdate={updateEventField} 
                    />
                  </ComponentErrorBoundary>
                )}

                {activeTab === 'templates' && (
                  <ComponentErrorBoundary componentName="Template Selection">
                    <TemplateSelectionForm
                      templates={eventData.templates || {}}
                      availableTemplates={availableTemplates}
                      onUpdate={updateTemplateField}
                    />
                  </ComponentErrorBoundary>
                )}

                {activeTab === 'content-variables' && (
                  <ComponentErrorBoundary componentName="Content Variables">
                    <ContentVariablesForm
                      contentVariables={eventData.contentVariables || {}}
                      globalContentVariables={globalContentVariables}
                      onUpdate={updateContentVariables}
                    />
                  </ComponentErrorBoundary>
                )}

                {activeTab === 'preferred-communication' && (
                  <ComponentErrorBoundary componentName="Preferred Communication">
                    <PreferredCommunicationForm
                      preferredCommunication={eventData.preferredCommunication || []}
                      onUpdate={updatePreferredCommunication}
                    />
                  </ComponentErrorBoundary>
                )}

                {activeTab === 'trigger-conditions' && (
                  <ComponentErrorBoundary componentName="Trigger Conditions">
                    <TriggerConditionsForm
                      triggerConditions={eventData.triggerConditions || {}}
                      onUpdate={updateTriggerConditions}
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
