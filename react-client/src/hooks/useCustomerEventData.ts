import { useState, useEffect } from 'react';
import { EventField, TemplateField } from '../types/components';

export const useCustomerEventData = (selectedEvent: string, selectedOrderType: string, specificEventData: any) => {
  const [eventFields, setEventFields] = useState<EventField[]>([]);
  const [templateFields, setTemplateFields] = useState<TemplateField[]>([]);
  const [contentVariables, setContentVariables] = useState<Record<string, string>>({});
  const [contentVariablesOverrides, setContentVariablesOverrides] = useState<Record<string, Record<string, Record<string, string>>>>({});
  const [triggerConditions, setTriggerConditions] = useState<Record<string, boolean>>({});
  const [preferredCommunication, setPreferredCommunication] = useState<Array<{ channel: string; priority: number }>>([]);

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
    setPreferredCommunication(specificEventData?.preferredCommunication || []);
  }, [selectedEvent, selectedOrderType, specificEventData]);

  return {
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
  };
};