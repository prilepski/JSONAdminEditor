import React, { useState, useEffect } from 'react';
import {
  useCustomersQuery,
  useCustomerEventsQuery,
  useCustomerEventsMutation,
} from '../hooks/useCustomerQuery';
import { useEventTriggersQuery, useTemplatesQuery } from '../hooks/useEventQuery';
import toast from 'react-hot-toast';
import {
  CustomerEventData,
  EventField,
  TemplateField,
  ContentVariable,
} from '../types/customerEvent';
import { PageHeader, CustomerSelector, SaveButton, TabNavigation } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { CustomerEventSelector } from '../components/events/CustomerEventSelector';
import { CustomerEventDataTable } from '../components/events/CustomerEventDataTable';
import { CustomerTemplateTable } from '../components/events/CustomerTemplateTable';
import { CustomerContentVariablesTable } from '../components/events/CustomerContentVariablesTable';

export const CustomerEvents: React.FC = () => {
  const [selectedCustomer, setSelectedCustomer] = useState<string>('');
  const [selectedEvent, setSelectedEvent] = useState<string>('');
  const [selectedOrderType, setSelectedOrderType] = useState<string>('Delivery');
  const [activeTab, setActiveTab] = useState<string>('event-data');
  const [eventFields, setEventFields] = useState<EventField[]>([]);
  const [templateFields, setTemplateFields] = useState<TemplateField[]>([]);
  const [contentVariables, setContentVariables] = useState<Record<string, ContentVariable>>({});

  const { data: customers = [] } = useCustomersQuery();
  const { data: activeEventTriggers = [] } = useEventTriggersQuery();
  const { data: availableTemplates = [] } = useTemplatesQuery();
  const { data: customerEventData } = useCustomerEventsQuery(selectedCustomer);
  const saveMutation = useCustomerEventsMutation();

  useEffect(() => {
    if (selectedEvent && selectedCustomer) {
      // Find existing customer event data
      const existingEvent = Array.isArray(customerEventData)
        ? customerEventData.find(
            (event: any) => event.Event === selectedEvent && event.OrderType === selectedOrderType
          )
        : customerEventData;

      // Initialize event fields with existing data or defaults
      const fields: EventField[] = [
        {
          name: 'Phone',
          value: existingEvent?.Phone || '$consigneeContact.phone$',
          isRedefined: !!existingEvent?.Phone,
          type: 'text',
          globalValue: '$consigneeContact.phone$',
        },
        {
          name: 'Email',
          value: existingEvent?.Email || '$consigneeContact.email$',
          isRedefined: !!existingEvent?.Email,
          type: 'text',
          globalValue: '$consigneeContact.email$',
        },
        {
          name: 'Logo',
          value: existingEvent?.Logo || 'base64',
          isRedefined: !!existingEvent?.Logo,
          type: 'text',
          globalValue: 'base64',
        },
        {
          name: 'IsSuppressed',
          value: existingEvent?.IsSuppressed ? 'true' : 'false',
          isRedefined: existingEvent?.IsSuppressed !== undefined,
          type: 'checkbox',
          globalValue: 'false',
        },
      ];
      setEventFields(fields);

      // Initialize template fields with existing data
      const templates: TemplateField[] = [
        {
          channel: 'Email',
          value: existingEvent?.Templates?.Email || '',
          isRedefined: !!existingEvent?.Templates?.Email,
          globalValue: '',
        },
        {
          channel: 'Sms',
          value: existingEvent?.Templates?.Sms || '',
          isRedefined: !!existingEvent?.Templates?.Sms,
          globalValue: '',
        },
        {
          channel: 'Voice',
          value: existingEvent?.Templates?.Voice || '',
          isRedefined: !!existingEvent?.Templates?.Voice,
          globalValue: '',
        },
      ];
      setTemplateFields(templates);

      // Initialize content variables with existing data
      const vars: Record<string, ContentVariable> = {};
      if (existingEvent?.ContentVariables) {
        Object.entries(existingEvent.ContentVariables).forEach(([key, value]) => {
          vars[key] = {
            name: key,
            value: String(value),
            isRedefined: true,
          };
        });
      }
      setContentVariables(vars);
    }
  }, [selectedEvent, selectedCustomer, selectedOrderType, customerEventData]);

  const handleSave = async () => {
    if (!selectedCustomer || !selectedEvent) return;

    const saveData: CustomerEventData = {
      Event: selectedEvent,
      OrderType: selectedOrderType,
    };

    // Add redefined event fields
    eventFields.forEach((field) => {
      if (field.isRedefined) {
        if (field.type === 'checkbox') {
          (saveData as any)[field.name] = field.value === 'true';
        } else {
          (saveData as any)[field.name] = field.value;
        }
      }
    });

    // Add redefined templates
    const templates: Record<string, string> = {};
    templateFields.forEach((template) => {
      if (template.isRedefined) {
        templates[template.channel] = template.value;
      }
    });
    if (Object.keys(templates).length > 0) {
      saveData.Templates = templates;
    }

    // Add content variables
    const vars: Record<string, string> = {};
    Object.entries(contentVariables).forEach(([key, data]) => {
      if (data.isRedefined || !data.globalValue) {
        vars[key] = data.value;
      }
    });
    if (Object.keys(vars).length > 0) {
      saveData.ContentVariables = vars;
    }

    try {
      const success = await saveMutation.mutateAsync({
        customerId: selectedCustomer,
        data: saveData,
      });
      if (success) {
        toast.success(`Customer event '${selectedEvent}' saved successfully!`);
      } else {
        toast.error('Failed to save customer event');
      }
    } catch (error) {
      toast.error('Error saving customer event');
    }
  };

  const updateEventField = (fieldName: string, value: string) => {
    setEventFields((prev) =>
      prev.map((field) => (field.name === fieldName ? { ...field, value } : field))
    );
  };

  const toggleEventFieldRedefined = (fieldName: string, isRedefined: boolean) => {
    setEventFields((prev) =>
      prev.map((field) => (field.name === fieldName ? { ...field, isRedefined } : field))
    );
  };

  const updateTemplateField = (channel: 'Email' | 'Sms' | 'Voice', value: string) => {
    setTemplateFields((prev) =>
      prev.map((template) => (template.channel === channel ? { ...template, value } : template))
    );
  };

  const toggleTemplateRedefined = (channel: 'Email' | 'Sms' | 'Voice', isRedefined: boolean) => {
    setTemplateFields((prev) =>
      prev.map((template) =>
        template.channel === channel ? { ...template, isRedefined } : template
      )
    );
  };

  const addContentVariable = () => {
    const newKey = `new_var_${Date.now()}`;
    setContentVariables((prev) => ({
      ...prev,
      [newKey]: { name: newKey, value: '', isRedefined: true },
    }));
  };

  const updateContentVariable = (oldKey: string, newKey: string, value: string) => {
    setContentVariables((prev) => {
      const newVars = { ...prev };
      if (oldKey !== newKey) {
        delete newVars[oldKey];
      }
      newVars[newKey] = { ...prev[oldKey], name: newKey, value };
      return newVars;
    });
  };

  const removeContentVariable = (key: string) => {
    setContentVariables((prev) => {
      const newVars = { ...prev };
      delete newVars[key];
      return newVars;
    });
  };

  const tabs = [
    { id: 'event-data', label: 'Event Data', icon: 'fa-cog' },
    { id: 'templates', label: 'Templates', icon: 'fa-file-alt' },
    { id: 'content-variables', label: 'Content Variables', icon: 'fa-code' },
  ];

  return (
    <PageErrorBoundary pageName="Customer Events">
      <PageHeader icon="fa-calendar-alt" title="Customer Events Management" />

      <ComponentErrorBoundary componentName="Customer Selector">
        <CustomerSelector
          customers={customers}
          selectedCustomer={selectedCustomer}
          onCustomerChange={setSelectedCustomer}
        />
      </ComponentErrorBoundary>

      {selectedCustomer && (
        <ComponentErrorBoundary componentName="Event Selector">
          <CustomerEventSelector
            selectedCustomer={selectedCustomer}
            selectedEvent={selectedEvent}
            selectedOrderType={selectedOrderType}
            eventTriggers={activeEventTriggers}
            onEventChange={setSelectedEvent}
            onOrderTypeChange={setSelectedOrderType}
          />
        </ComponentErrorBoundary>
      )}

      {/* Event Editor */}
      {selectedCustomer && selectedEvent && (
        <div className="card">
          <div className="card-header">
            <div className="d-flex justify-content-between align-items-center">
              <h3 className="mb-0">
                <i className="fas fa-edit me-2"></i>Edit Event: {selectedEvent} ({selectedOrderType}
                )
              </h3>
              <SaveButton onClick={handleSave} loading={saveMutation.isPending} text="Save Event" />
            </div>
          </div>
          <div className="card-body">
            <TabNavigation tabs={tabs} activeTab={activeTab} onTabChange={setActiveTab} />

            {activeTab === 'event-data' && (
              <ComponentErrorBoundary componentName="Event Data Table">
                <CustomerEventDataTable
                  eventFields={eventFields}
                  onUpdateField={updateEventField}
                  onToggleRedefined={toggleEventFieldRedefined}
                />
              </ComponentErrorBoundary>
            )}

            {activeTab === 'templates' && (
              <ComponentErrorBoundary componentName="Template Table">
                <CustomerTemplateTable
                  templateFields={templateFields}
                  availableTemplates={availableTemplates}
                  onUpdateTemplate={updateTemplateField}
                  onToggleRedefined={toggleTemplateRedefined}
                />
              </ComponentErrorBoundary>
            )}

            {activeTab === 'content-variables' && (
              <ComponentErrorBoundary componentName="Content Variables Table">
                <CustomerContentVariablesTable
                  contentVariables={contentVariables}
                  onAdd={addContentVariable}
                  onUpdate={updateContentVariable}
                  onRemove={removeContentVariable}
                />
              </ComponentErrorBoundary>
            )}
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};
