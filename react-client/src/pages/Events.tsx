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
import { PageHeader, TabNavigation, SaveButton } from '../components/common';
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
  const { data: activeEventTriggers = [] } = useEventTriggersQuery();
  const { data: availableOrderTypes = [] } = useOrderTypesQuery();
  const { data: availableTemplates = [] } = useTemplatesQuery();
  const [eventSupportsByOrderType, setEventSupportsByOrderType] = useState(false);

  const { data: eventSupports = false } = useEventSupportQuery(selectedEvent);
  const { data: eventInfo } = useEventQuery(selectedEvent, selectedOrderType);
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

  return (
    <PageErrorBoundary pageName="Events">
      <PageHeader icon="fa-calendar-alt" title="Event Management" />

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

      {selectedEvent && eventData && (
        <div className="card">
          <div className="card-header">
            <div className="d-flex justify-content-between align-items-center">
              <h3 className="mb-0">
                <i className="fas fa-edit me-2"></i>Edit Event: {selectedEvent}
              </h3>
              <SaveButton
                onClick={handleSaveEventData}
                loading={eventMutation.isPending}
                text="Save Changes"
              />
            </div>
          </div>
          <div className="card-body">
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
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};
