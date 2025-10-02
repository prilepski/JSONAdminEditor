import React, { useState } from 'react';
import { useFormState } from '../hooks/useFormState';
import { useErrorHandler } from '../hooks/useErrorHandler';
import { useCustomerEventData } from '../hooks/useCustomerEventData';
import { useCustomerEventActions } from '../hooks/useCustomerEventActions';
import {
  useCustomersQuery,
  useCustomerEventQuery,
  useCustomerEventMutation,
} from '../hooks/useCustomerQuery';
import { useContentVariablesQuery } from '../hooks/useContentVariableQuery';
import { useEventQuery } from '../hooks/useEventQuery';
import {
  PageHeader,
  CustomerSelector,
  SaveButton,
  TabNavigation,
  LoadingSpinner,
  TableSkeleton, 
    PageErrorBoundary,
    ComponentErrorBoundary,
    NestedVariablesEditor
} from '../components/common';
import { CustomerEventSelector } from '../components/events/CustomerEventSelector';
import { CustomerEventTabs } from '../components/events/CustomerEventTabs';
import { buildCustomerEventSaveData } from '../utils/customerEventSaveData';

export const CustomerEvents: React.FC = () => {
  const { state, updateField } = useFormState({
    selectedCustomer: '',
    selectedEvent: '',
    selectedOrderType: 'Delivery',
    activeTab: 'event-data',
  });
  const { handleError } = useErrorHandler({ context: 'CustomerEvents' });
  const [contentVariableRedefinedStates, setContentVariableRedefinedStates] = useState<Record<string, boolean>>({});

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

  const {
    eventFields,
    setEventFields,
    templateFields,
    setTemplateFields,
    contentVariables,
    setContentVariables,
    contentVariablesOverrides,
    setContentVariablesOverrides,
    triggerConditions,
    setTriggerConditions,
    preferredCommunication,
    setPreferredCommunication,
  } = useCustomerEventData(selectedEvent, selectedOrderType, specificEventData, eventData);

  const {
    updateEventField,
    toggleEventFieldRedefined,
    updateTemplateField,
    toggleTemplateRedefined,
    addContentVariableOverride,
    updateContentVariableOverride,
    updateContentVariableOverrideKey,
    removeContentVariableOverride,
  } = useCustomerEventActions(setEventFields, setTemplateFields, setContentVariablesOverrides);

  const handleSaveNestedOverrides = async (category: string, subcategory: string, variables: Record<string, string>) => {
    const updatedOverrides = {
      ...contentVariablesOverrides,
      [category]: {
        ...contentVariablesOverrides[category],
        [subcategory]: variables
      }
    };
    setContentVariablesOverrides(updatedOverrides);
    // No individual save - changes will be saved with main Save Event button
  };

  const handleAddCategory = async (name: string) => {
    if (!name.trim() || Object.keys(contentVariablesOverrides).includes(name)) return;
    const updatedOverrides = { ...contentVariablesOverrides, [name]: {} };
    setContentVariablesOverrides(updatedOverrides);
  };

  const handleAddSubcategory = async (category: string, name: string) => {
    if (!name.trim() || Object.keys(contentVariablesOverrides[category] || {}).includes(name)) return;
    const updatedOverrides = {
      ...contentVariablesOverrides,
      [category]: { ...contentVariablesOverrides[category], [name]: {} }
    };
    setContentVariablesOverrides(updatedOverrides);
  };

  const handleDeleteSubcategory = async (category: string, subcategory: string) => {
    const updatedOverrides = { ...contentVariablesOverrides };
    if (updatedOverrides[category]) {
      delete updatedOverrides[category][subcategory];
      if (Object.keys(updatedOverrides[category]).length === 0) {
        delete updatedOverrides[category];
      }
    }
    setContentVariablesOverrides(updatedOverrides);
  };

  const handleSave = async () => {
    if (!selectedCustomer || !selectedEvent) return;

    const saveData = buildCustomerEventSaveData(
      selectedEvent,
      selectedOrderType,
      eventFields,
      templateFields,
      contentVariables,
      contentVariableRedefinedStates,
      contentVariablesOverrides,
      triggerConditions,
      preferredCommunication
    );

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

  const tabs = [
    { id: 'event-data', label: 'Event Data', icon: 'fa-cog' },
    { id: 'templates', label: 'Templates', icon: 'fa-file-alt' },
    { id: 'content-variables', label: 'Content Variables', icon: 'fa-code' },
    { id: 'content-variables-overrides', label: 'Content Variables Overrides', icon: 'fa-layer-group' },
    { id: 'preferred-communication', label: 'Preferred Communication', icon: 'fa-comments' },
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

                {activeTab === 'content-variables-overrides' ? (
                  <NestedVariablesEditor
                    data={contentVariablesOverrides}
                    onSave={handleSaveNestedOverrides}
                    onAddCategory={handleAddCategory}
                    onAddSubcategory={handleAddSubcategory}
                    onDeleteSubcategory={handleDeleteSubcategory}
                    isLoading={false}
                    isSaving={false}
                    showSaveButton={false}
                  />
                ) : (
                  <CustomerEventTabs
                    activeTab={activeTab}
                    eventFields={eventFields}
                    templateFields={templateFields}
                    contentVariables={contentVariables}
                    globalContentVariables={globalContentVariables}
                    eventData={eventData}
                    contentVariablesOverrides={contentVariablesOverrides}
                    preferredCommunication={preferredCommunication}
                    triggerConditions={triggerConditions}
                    onUpdateEventField={updateEventField}
                    onToggleEventFieldRedefined={toggleEventFieldRedefined}
                    onUpdateTemplateField={updateTemplateField}
                    onToggleTemplateRedefined={toggleTemplateRedefined}
                    onUpdateContentVariables={setContentVariables}
                    onContentVariableRedefinedStatesChange={setContentVariableRedefinedStates}
                    onAddContentVariableOverride={addContentVariableOverride}
                    onUpdateContentVariableOverride={updateContentVariableOverride}
                    onUpdateContentVariableOverrideKey={updateContentVariableOverrideKey}
                    onRemoveContentVariableOverride={removeContentVariableOverride}
                    onUpdatePreferredCommunication={setPreferredCommunication}
                    onUpdateTriggerConditions={setTriggerConditions}
                  />
                )}
              </>
            )}
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};
