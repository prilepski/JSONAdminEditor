import { CustomerEventMapping } from '../types';
import { EventField, TemplateField } from '../types/components';
import { getRedefinedVariables } from '../components/common';

export const buildCustomerEventSaveData = (
  selectedEvent: string,
  selectedOrderType: string,
  eventFields: EventField[],
  templateFields: TemplateField[],
  contentVariables: Record<string, string>,
  contentVariableRedefinedStates: Record<string, boolean>,
  contentVariablesOverrides: Record<string, Record<string, Record<string, string>>>,
  triggerConditions: Record<string, boolean>,
  preferredCommunication: Array<{ channel: string; priority: number }>
): CustomerEventMapping => {
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

  if (preferredCommunication.length > 0) {
    saveData.preferredCommunication = preferredCommunication;
  }

  return saveData;
};