import React from 'react';
import { ComponentErrorBoundary, ContentVariablesTable, getRedefinedVariables } from '../common';
import { CustomerEventDataTable } from './CustomerEventDataTable';
import { CustomerTemplateTable } from './CustomerTemplateTable';
import { CustomerContentVariablesOverridesTable } from './CustomerContentVariablesOverridesTable';
import { PreferredCommunicationForm } from './PreferredCommunicationForm';
import { TriggerConditionsForm } from './TriggerConditionsForm';
import { EventField, TemplateField } from '../../types/components';

interface CustomerEventTabsProps {
  activeTab: string;
  eventFields: EventField[];
  templateFields: TemplateField[];
  contentVariables: Record<string, string>;
  globalContentVariables: Record<string, string>;
  eventData: any;
  contentVariablesOverrides: Record<string, Record<string, Record<string, string>>>;
  preferredCommunication: Array<{ channel: string; priority: number }>;
  triggerConditions: Record<string, boolean>;
  onUpdateEventField: (fieldName: string, value: string) => void;
  onToggleEventFieldRedefined: (fieldName: string, isRedefined: boolean) => void;
  onUpdateTemplateField: (channel: string, value: string) => void;
  onToggleTemplateRedefined: (channel: string, isRedefined: boolean) => void;
  onUpdateContentVariables: (variables: Record<string, string>) => void;
  onContentVariableRedefinedStatesChange: (states: Record<string, boolean>) => void;
  onAddContentVariableOverride: () => void;
  onUpdateContentVariableOverride: (eventKey: string, variableKey: string, overrideKey: string, value: string) => void;
  onUpdateContentVariableOverrideKey: (oldEventKey: string, oldVariableKey: string, oldOverrideKey: string, newEventKey: string, newVariableKey: string, newOverrideKey: string) => void;
  onRemoveContentVariableOverride: (eventKey: string, variableKey: string, overrideKey: string) => void;
  onUpdatePreferredCommunication: (communication: Array<{ channel: string; priority: number }>) => void;
  onUpdateTriggerConditions: (conditions: Record<string, boolean>) => void;
}

export const CustomerEventTabs: React.FC<CustomerEventTabsProps> = ({
  activeTab,
  eventFields,
  templateFields,
  contentVariables,
  globalContentVariables,
  eventData,
  contentVariablesOverrides,
  preferredCommunication,
  triggerConditions,
  onUpdateEventField,
  onToggleEventFieldRedefined,
  onUpdateTemplateField,
  onToggleTemplateRedefined,
  onUpdateContentVariables,
  onContentVariableRedefinedStatesChange,
  onAddContentVariableOverride,
  onUpdateContentVariableOverride,
  onUpdateContentVariableOverrideKey,
  onRemoveContentVariableOverride,
  onUpdatePreferredCommunication,
  onUpdateTriggerConditions,
}) => {
  return (
    <>
      {activeTab === 'event-data' && (
        <ComponentErrorBoundary componentName="Event Data Table">
          <CustomerEventDataTable
            eventFields={eventFields}
            onUpdateField={onUpdateEventField}
            onToggleRedefined={onToggleEventFieldRedefined}
          />
        </ComponentErrorBoundary>
      )}

      {activeTab === 'templates' && (
        <ComponentErrorBoundary componentName="Template Table">
          <CustomerTemplateTable
            templateFields={templateFields}
            onUpdateTemplate={onUpdateTemplateField}
            onToggleRedefined={onToggleTemplateRedefined}
          />
        </ComponentErrorBoundary>
      )}

      {activeTab === 'content-variables' && (
        <ComponentErrorBoundary componentName="Content Variables Table">
          <ContentVariablesTable
            contentVariables={contentVariables}
            globalContentVariables={globalContentVariables}
            eventContentVariables={eventData?.contentVariables}
            onUpdate={onUpdateContentVariables}
            onRedefinedStatesChange={onContentVariableRedefinedStatesChange}
          />
        </ComponentErrorBoundary>
      )}

      {activeTab === 'content-variables-overrides' && (
        <ComponentErrorBoundary componentName="Content Variables Overrides">
          <CustomerContentVariablesOverridesTable
            contentVariablesOverrides={contentVariablesOverrides}
            onAdd={onAddContentVariableOverride}
            onUpdate={onUpdateContentVariableOverride}
            onUpdateKey={onUpdateContentVariableOverrideKey}
            onRemove={onRemoveContentVariableOverride}
          />
        </ComponentErrorBoundary>
      )}

      {activeTab === 'preferred-communication' && (
        <ComponentErrorBoundary componentName="Preferred Communication">
          <PreferredCommunicationForm
            preferredCommunication={preferredCommunication}
            onUpdate={onUpdatePreferredCommunication}
          />
        </ComponentErrorBoundary>
      )}

      {activeTab === 'trigger-conditions' && (
        <ComponentErrorBoundary componentName="Trigger Conditions">
          <TriggerConditionsForm
            triggerConditions={triggerConditions}
            onUpdate={onUpdateTriggerConditions}
          />
        </ComponentErrorBoundary>
      )}
    </>
  );
};