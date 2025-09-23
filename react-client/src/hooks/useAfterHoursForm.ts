import { useState, useEffect, useCallback } from 'react';
import { AfterHours2 } from '../types';

export const useAfterHoursForm = (data?: AfterHours2) => {
  const [formData, setFormData] = useState({
    restrictedHoursPeriod: { start: '', end: '' },
    exceptionOfValidation: { events: [] as any[] }
  });

  useEffect(() => {
    if (data) {
      setFormData({
        restrictedHoursPeriod: data.restrictedHoursPeriod || { start: '', end: '' },
        exceptionOfValidation: { events: data.exceptionOfValidation?.events || [] }
      });
    }
  }, [data]);

  const updateTime = useCallback((field: 'start' | 'end', value: string) => {
    setFormData(prev => ({
      ...prev,
      restrictedHoursPeriod: { ...prev.restrictedHoursPeriod, [field]: value }
    }));
  }, []);

  const addEvent = useCallback(() => {
    setFormData(prev => ({
      ...prev,
      exceptionOfValidation: {
        ...prev.exceptionOfValidation,
        events: [...prev.exceptionOfValidation.events, { name: '', type: '' }]
      }
    }));
  }, []);

  const removeEvent = useCallback((index: number) => {
    setFormData(prev => ({
      ...prev,
      exceptionOfValidation: {
        ...prev.exceptionOfValidation,
        events: prev.exceptionOfValidation.events.filter((_, i) => i !== index)
      }
    }));
  }, []);

  const updateEvent = useCallback((index: number, field: string, value: string) => {
    setFormData(prev => ({
      ...prev,
      exceptionOfValidation: {
        ...prev.exceptionOfValidation,
        events: prev.exceptionOfValidation.events.map((event, i) => 
          i === index ? { ...event, [field]: value } : event
        )
      }
    }));
  }, []);

  return {
    formData,
    updateTime,
    addEvent,
    removeEvent,
    updateEvent
  };
};