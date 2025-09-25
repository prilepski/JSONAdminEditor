import React, { useState, useEffect } from 'react';
import { useFormState } from '../hooks/useFormState';
import { useErrorHandler } from '../hooks/useErrorHandler';
import {
  useCustomersQuery,
  useCustomerEventQuery,
  useCustomerEventMutation,
} from '../hooks/useCustomerQuery';
import { CustomerEventMapping } from '../types';
import { EventField, TemplateField, ContentVariable } from '../types/components';
import {
  PageHeader,
  CustomerSelector,
  SaveButton,
  TabNavigation,
  LoadingSpinner,
  TableSkeleton,
} from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { CustomerEventSelector } from '../components/events/CustomerEventSelector';
import { CustomerEventDataTable } from '../components/events/CustomerEventDataTable';
import { CustomerTemplateTable } from '../components/events/CustomerTemplateTable';
import { CustomerContentVariablesTable } from '../components/events/CustomerContentVariablesTable';
import { CustomerContentVariablesOverridesTable } from '../components/events/CustomerContentVariablesOverridesTable';

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
  const [contentVariables, setContentVariables] = useState<Record<string, ContentVariable>>({});
  const [contentVariablesOverrides, setContentVariablesOverrides] = useState<Record<string, Record<string, Record<string, string>>>>({});

  const { selectedCustomer, selectedEvent, selectedOrderType, activeTab } = state;

  const { data: customers = [], isLoading: customersLoading } = useCustomersQuery();
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

    const vars: Record<string, ContentVariable> = {};
    if (specificEventData?.contentVariables) {
      Object.entries(specificEventData.contentVariables).forEach(([key, value]) => {
        vars[key] = { name: key, value: String(value), isRedefined: true };
      });
    }
    setContentVariables(vars);

    setContentVariablesOverrides(specificEventData?.contentVariablesOverrides || {});
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

    const vars: Record<string, string> = {};
    Object.entries(contentVariables).forEach(([key, data]) => {
      if (data.isRedefined || !data.globalValue) vars[key] = data.value;
    });
    if (Object.keys(vars).length > 0) saveData.contentVariables = vars;

    if (Object.keys(contentVariablesOverrides).length > 0) {
      saveData.contentVariablesOverrides = contentVariablesOverrides;
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

  const addContentVariable = () => {
    const newKey = `new_var_${Date.now()}`;
    setContentVariables(prev => ({ ...prev, [newKey]: { name: newKey, value: '', isRedefined: true } }));
  };

  const updateContentVariable = (oldKey: string, newKey: string, value: string) => {
    setContentVariables(prev => {
      const newVars = { ...prev };
      if (oldKey !== newKey) delete newVars[oldKey];
      newVars[newKey] = { ...prev[oldKey], name: newKey, value };
      return newVars;
    });
  };

  const removeContentVariable = (key: string) => {
    setContentVariables(prev => {
      const { [key]: _, ...rest } = prev;
      return rest;
    });
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
                customerEvents={customerEventData}
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
                    <CustomerContentVariablesTable
                      contentVariables={contentVariables}
                      selectedEvent={selectedEvent}
                      selectedOrderType={selectedOrderType}
                      onAdd={addContentVariable}
                      onUpdate={updateContentVariable}
                      onRemove={removeContentVariable}
                      onToggleRedefined={(key, isRedefined) => {
                        if (isRedefined) {
                          const existingVar = contentVariables[key];
                          if (!existingVar) {
                            setContentVariables(prev => ({
                              ...prev,
                              [key]: { name: key, value: '', isRedefined: true }
                            }));
                          }
                        } else {
                          removeContentVariable(key);
                        }
                      }}
                    />
                  </ComponentErrorBoundary>
                )}

                {activeTab === 'content-variables-overrides' && (
                  <ComponentErrorBoundary componentName="Content Variables Overrides">
                    <CustomerContentVariablesOverridesTable
                      contentVariablesOverrides={contentVariablesOverrides}
                      onAdd={addContentVariableOverride}
                      onUpdate={updateContentVariableOverride}
                      onRemove={removeContentVariableOverride}
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
