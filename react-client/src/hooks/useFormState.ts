import { useState, useCallback } from 'react';

export function useFormState<T extends Record<string, any>>(initialState: T) {
  const [state, setState] = useState<T>(initialState);

  const updateField = useCallback((field: keyof T, value: any) => {
    setState(prev => ({ ...prev, [field]: value }));
  }, []);

  const updateFields = useCallback((updates: Partial<T>) => {
    setState(prev => ({ ...prev, ...updates }));
  }, []);

  const resetForm = useCallback(() => {
    setState(initialState);
  }, [initialState]);

  return {
    state,
    updateField,
    updateFields,
    resetForm,
    setState
  };
}