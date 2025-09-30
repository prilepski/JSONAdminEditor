import React, { useState, useEffect } from 'react';
import { useFormState } from '../hooks/useFormState';
import { useErrorHandler } from '../hooks/useErrorHandler';
import {
  useCustomersQuery,
  useCustomerEventQuery,
  useCustomerEventMutation,
} from '../hooks/useCustomerQuery';
import { CustomerEventMapping } from '../types';
import { EventField, TemplateField } from '../types/components';
import { useContentVariablesQuery } from '../hooks/useContentVariableQuery';
import { useEventQuery } from '../hooks/useEventQuery';
import {
  PageHeader,
  CustomerSelector,
  SaveButton,
  TabNavigation,
  LoadingSpinner,
  TableSkeleton,
  ContentVariablesTable,
  getRedefinedVariables,
} from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { CustomerEventSelector } from '../components/events/CustomerEventSelector';
import { CustomerEventDataTable } from '../components/events/CustomerEventDataTable';
import { CustomerTemplateTable } from '../components/events/CustomerTemplateTable';

import { CustomerContentVariablesOverridesTable } from '../components/events/CustomerContentVariablesOverridesTable';
import { TriggerConditionsForm } from '../components/events/TriggerConditionsForm';

export const CustomerEvents: React.FC = () => {
  const { state, updateField } = useFormState({
    selectedCustomer: '',
    selectedEvent: '',
    selectedOrderType: 'Delivery',
    activeTab: 'event-data',
  });
  const { handleError } = useErrorHandler({ context: 'CustomerEvents' });

  const [eventFields, setEventFields] = useState<EventField[]>([]);
  const [templateFields, setTemplateFields] = useState<TemplateField[]>([]);
  const [contentVariables, setContentVariables] = useState<Record<string, string>>({});
  const [contentVariablesOverrides, setContentVariablesOverrides] = useState<Record<string, Record<string, Record<string, string>>>>({});
  const [contentVariableRedefinedStates, setContentVariableRedefinedStates] = useState<Record<string, boolean>>({});
  const [triggerConditions, setTriggerConditions] = useState<Record<string, boolean>>({});

  const { selectedCustomer, selectedEvent, selectedOrderType, activeTab } = state;

  const { data: customers = [], isLoading: customersLoading } = useCustomersQuery();
  const { data: globalContentVariables = {} } = useContentVariablesQuery();
  const { data: eventData } = useEventQuery(selectedEvent, selectedOrderType);
  const saveMutation = useCustomerEventMutation();
  const { data: specificEventData, isLoading: specificEventLoading } = useCustomerEventQuery(
    selectedCustomer,
    selectedEvent,
    selectedOrderType
  );

  useEffect(() => {
    if (!selectedEvent || !selectedOrderType) return;

    const fields: EventField[] = [
      {
        name: 'Phone',
        value: specificEventData?.phone || '$consigneeContact.phone$',
        isRedefined: !!specificEventData?.phone,
        type: 'text',
        globalValue: '$consigneeContact.phone$',
      },
      {
        name: 'Email',
        value: specificEventData?.email || '$consigneeContact.email$',
        isRedefined: !!specificEventData?.email,
        type: 'text',
        globalValue: '$consigneeContact.email$',
      },
      {
        name: 'IsSuppressed',
        value: specificEventData?.isSuppressed ? 'true' : 'false',
        isRedefined: specificEventData?.isSuppressed !== undefined,
        type: 'checkbox',
        globalValue: 'false',
      },
    ];
    setEventFields(fields);

    const templates: TemplateField[] = ['Email', 'Sms', 'Voice'].map(channel => ({
      channel,
      value: specificEventData?.templates?.[channel] || '',
      isRedefined: !!specificEventData?.templates?.[channel],
      globalValue: '',
    }));
    setTemplateFields(templates);

    const vars: Record<string, string> = {};
    if (specificEventData?.contentVariables) {
      Object.entries(specificEventData.contentVariables).forEach(([key, value]) => {
        vars[key] = String(value);
      });
    }
    setContentVariables(vars);

    setContentVariablesOverrides(specificEventData?.contentVariablesOverrides || {});
    setTriggerConditions(specificEventData?.triggerConditions || {});
  }, [selectedEvent, selectedOrderType, specificEventData]);

  const handleSave = async () => {
    if (!selectedCustomer || !selectedEvent) return;

    const saveData: CustomerEventMapping = { event: selectedEvent, orderType: selectedOrderType };

    eventFields.forEach((field) => {
      if (field.isRedefined) {
        if (field.name === 'Phone') saveData.phone = field.value;
        if (field.name === 'Email') saveData.email = field.value;
        if (field.name === 'IsSuppressed') saveData.isSuppressed = field.value === 'true';
      }
    });

    const templates: Record<string, string> = {};
    templateFields.forEach(t => {
      if (t.isRedefined) templates[t.channel] = t.value;
    });
    if (Object.keys(templates).length > 0) saveData.templates = templates;

    const vars = getRedefinedVariables(contentVariables, contentVariableRedefinedStates);
    if (Object.keys(vars).length > 0) saveData.contentVariables = vars;

    if (Object.keys(contentVariablesOverrides).length > 0) {
      saveData.contentVariablesOverrides = contentVariablesOverrides;
    }

    const validTriggerConditions = Object.fromEntries(
      Object.entries(triggerConditions).filter(([key]) => 
        ['IsSchedulable', 'IsOpen', 'IsScheduled', 'IsCompleted', 'IsReadyForScheduling'].includes(key)
      )
    );
    if (Object.keys(validTriggerConditions).length > 0) {
      saveData.triggerConditions = validTriggerConditions;
    }

    try {
      await saveMutation.mutateAsync({
        customerId: selectedCustomer,
        eventName: selectedEvent,
        orderType: selectedOrderType,
        data: saveData,
      });
    } catch (error) {
      handleError(error, 'Failed to save customer event');
    }
  };

  const updateEventField = (fieldName: string, value: string) => {
    setEventFields(prev => prev.map(field => 
      field.name === fieldName ? { ...field, value } : field
    ));
  };

  const toggleEventFieldRedefined = (fieldName: string, isRedefined: boolean) => {
    setEventFields(prev => prev.map(field => 
      field.name === fieldName ? { ...field, isRedefined } : field
    ));
  };

  const updateTemplateField = (channel: string, value: string) => {
    setTemplateFields(prev => prev.map(template => 
      template.channel === channel ? { ...template, value } : template
    ));
  };

  const toggleTemplateRedefined = (channel: string, isRedefined: boolean) => {
    setTemplateFields(prev => prev.map(template => 
      template.channel === channel ? { ...template, isRedefined } : template
    ));
  };



  const addContentVariableOverride = () => {
    const eventKey = `event_${Date.now()}`;
    const variableKey = `variable_${Date.now()}`;
    const overrideKey = `override_${Date.now()}`;
    setContentVariablesOverrides((prev) => ({
      ...prev,
      [eventKey]: {
        ...prev[eventKey],
        [variableKey]: {
          ...prev[eventKey]?.[variableKey],
          [overrideKey]: ''
        }
      }
    }));
  };

  const updateContentVariableOverride = (eventKey: string, variableKey: string, overrideKey: string, value: string) => {
    setContentVariablesOverrides((prev) => ({
      ...prev,
      [eventKey]: {
        ...prev[eventKey],
        [variableKey]: {
          ...prev[eventKey]?.[variableKey],
          [overrideKey]: value
        }
      }
    }));
  };

  const updateContentVariableOverrideKey = (oldEventKey: string, oldVariableKey: string, oldOverrideKey: string, newEventKey: string, newVariableKey: string, newOverrideKey: string) => {
    setContentVariablesOverrides((prev) => {
      const newOverrides = { ...prev };
      const value = prev[oldEventKey]?.[oldVariableKey]?.[oldOverrideKey] || '';
      
      // Remove old entry
      if (newOverrides[oldEventKey]?.[oldVariableKey]) {
        delete newOverrides[oldEventKey][oldVariableKey][oldOverrideKey];
        if (Object.keys(newOverrides[oldEventKey][oldVariableKey]).length === 0) {
          delete newOverrides[oldEventKey][oldVariableKey];
        }
        if (Object.keys(newOverrides[oldEventKey]).length === 0) {
          delete newOverrides[oldEventKey];
        }
      }
      
      // Add new entry
      if (!newOverrides[newEventKey]) newOverrides[newEventKey] = {};
      if (!newOverrides[newEventKey][newVariableKey]) newOverrides[newEventKey][newVariableKey] = {};
      newOverrides[newEventKey][newVariableKey][newOverrideKey] = value;
      
      return newOverrides;
    });
  };

  const removeContentVariableOverride = (eventKey: string, variableKey: string, overrideKey: string) => {
    setContentVariablesOverrides((prev) => {
      const newOverrides = { ...prev };
      if (newOverrides[eventKey]?.[variableKey]) {
        delete newOverrides[eventKey][variableKey][overrideKey];
        if (Object.keys(newOverrides[eventKey][variableKey]).length === 0) {
          delete newOverrides[eventKey][variableKey];
        }
        if (Object.keys(newOverrides[eventKey]).length === 0) {
          delete newOverrides[eventKey];
        }
      }
      return newOverrides;
    });
  };

  const tabs = [
    { id: 'event-data', label: 'Event Data', icon: 'fa-cog' },
    { id: 'templates', label: 'Templates', icon: 'fa-file-alt' },
    { id: 'content-variables', label: 'Content Variables', icon: 'fa-code' },
    { id: 'content-variables-overrides', label: 'Content Variables Overrides', icon: 'fa-layer-group' },
    { id: 'trigger-conditions', label: 'Trigger Conditions', icon: 'fa-filter' },
  ];

  const isInitialLoading = customersLoading;
  const isEventConfigLoading = selectedCustomer && selectedEvent && specificEventLoading;

  return (
    <PageErrorBoundary pageName="Customer Events">
      <PageHeader icon="fa-calendar-alt" title="Customer Events Management" />

      {isInitialLoading ? (
        <LoadingSpinner text="Loading customer data..." />
      ) : (
        <>
          <ComponentErrorBoundary componentName="Customer Selector">
            <CustomerSelector
              customers={customers}
              selectedCustomer={selectedCustomer}
              onCustomerChange={(customer) => updateField('selectedCustomer', customer)}
            />
          </ComponentErrorBoundary>

          {selectedCustomer && (
            <ComponentErrorBoundary componentName="Event Selector">
              <CustomerEventSelector
                selectedCustomer={selectedCustomer}
                selectedEvent={selectedEvent}
                selectedOrderType={selectedOrderType}
                onEventChange={(event) => updateField('selectedEvent', event)}
                onOrderTypeChange={(orderType) => updateField('selectedOrderType', orderType)}
              />
            </ComponentErrorBoundary>
          )}
        </>
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
            {isEventConfigLoading ? (
              <TableSkeleton rows={4} columns={4} />
            ) : (
              <>
                <TabNavigation
                  tabs={tabs}
                  activeTab={activeTab}
                  onTabChange={(tab) => updateField('activeTab', tab)}
                />

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
                      onUpdateTemplate={updateTemplateField}
                      onToggleRedefined={toggleTemplateRedefined}
                    />
                  </ComponentErrorBoundary>
                )}

                {activeTab === 'content-variables' && (
                  <ComponentErrorBoundary componentName="Content Variables Table">
                    <ContentVariablesTable
                      contentVariables={contentVariables}
                      globalContentVariables={globalContentVariables}
                      eventContentVariables={eventData?.contentVariables}
                      onUpdate={setContentVariables}
                      onRedefinedStatesChange={setContentVariableRedefinedStates}
                    />
                  </ComponentErrorBoundary>
                )}

                {activeTab === 'content-variables-overrides' && (
                  <ComponentErrorBoundary componentName="Content Variables Overrides">
                    <CustomerContentVariablesOverridesTable
                      contentVariablesOverrides={contentVariablesOverrides}
                      onAdd={addContentVariableOverride}
                      onUpdate={updateContentVariableOverride}
                      onUpdateKey={updateContentVariableOverrideKey}
                      onRemove={removeContentVariableOverride}
                    />
                  </ComponentErrorBoundary>
                )}

                {activeTab === 'trigger-conditions' && (
                  <ComponentErrorBoundary componentName="Trigger Conditions">
                    <TriggerConditionsForm
                      triggerConditions={triggerConditions}
                      onUpdate={setTriggerConditions}
                    />
                  </ComponentErrorBoundary>
                )}
              </>
            )}
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};
