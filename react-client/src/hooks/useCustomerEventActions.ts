import { EventField, TemplateField } from '../types/components';

export const useCustomerEventActions = (
  setEventFields: React.Dispatch<React.SetStateAction<EventField[]>>,
  setTemplateFields: React.Dispatch<React.SetStateAction<TemplateField[]>>,
  setContentVariablesOverrides: React.Dispatch<React.SetStateAction<Record<string, Record<string, Record<string, string>>>>>
) => {
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
      
      if (newOverrides[oldEventKey]?.[oldVariableKey]) {
        delete newOverrides[oldEventKey][oldVariableKey][oldOverrideKey];
        if (Object.keys(newOverrides[oldEventKey][oldVariableKey]).length === 0) {
          delete newOverrides[oldEventKey][oldVariableKey];
        }
        if (Object.keys(newOverrides[oldEventKey]).length === 0) {
          delete newOverrides[oldEventKey];
        }
      }
      
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

  return {
    updateEventField,
    toggleEventFieldRedefined,
    updateTemplateField,
    toggleTemplateRedefined,
    addContentVariableOverride,
    updateContentVariableOverride,
    updateContentVariableOverrideKey,
    removeContentVariableOverride,
  };
};