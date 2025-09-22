import React, { useState, useEffect } from 'react';
import { useAfterHoursQuery, useAfterHoursMutation } from '../hooks/useAfterHoursQuery';
import { useQuery } from '@tanstack/react-query';
import { components } from '../generated/api';
import axios from 'axios';
import toast from 'react-hot-toast';
import { SaveButton, LoadingSpinner } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

type AfterHours2 = components['schemas']['AfterHours2'];

export const AfterHours: React.FC = () => {
  const { data, isLoading } = useAfterHoursQuery();
  const { data: eventTriggers = [] } = useQuery({
    queryKey: ['eventTriggers'],
    queryFn: async () => {
      const response = await axios.get('/api/dictionaries/event-triggers');
      return response.data;
    }
  });
  const { data: orderTypes = [] } = useQuery({
    queryKey: ['orderTypes'],
    queryFn: async () => {
      const response = await axios.get('/api/dictionaries/order-types');
      return response.data;
    }
  });
  const saveMutation = useAfterHoursMutation();
  const [formData, setFormData] = useState({
    restrictedHoursPeriod: { start: '', end: '' },
    exceptionOfValidation: { events: [] as any[] }
  });
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (data) {
      setFormData({
        restrictedHoursPeriod: data.restrictedHoursPeriod || { start: '', end: '' },
        exceptionOfValidation: data.exceptionOfValidation || { events: [] }
      });
    }
  }, [data]);

  const handleSave = async () => {
    setSaving(true);
    try {
      await saveMutation.mutateAsync(formData as AfterHours2);
      toast.success('After Hours settings saved successfully!');
    } catch (error) {
      toast.error('Error saving after hours settings');
    }
    setSaving(false);
  };

  const addEvent = () => {
    setFormData(prev => ({
      ...prev,
      exceptionOfValidation: {
        ...prev.exceptionOfValidation,
        events: [...prev.exceptionOfValidation.events, { name: '', type: '' }]
      }
    }));
  };

  const removeEvent = (index: number) => {
    setFormData(prev => ({
      ...prev,
      exceptionOfValidation: {
        ...prev.exceptionOfValidation,
        events: prev.exceptionOfValidation.events.filter((_, i) => i !== index)
      }
    }));
  };

  const updateEvent = (index: number, field: string, value: string) => {
    setFormData(prev => ({
      ...prev,
      exceptionOfValidation: {
        ...prev.exceptionOfValidation,
        events: prev.exceptionOfValidation.events.map((event, i) => 
          i === index ? { ...event, [field]: value } : event
        )
      }
    }));
  };

  return (
    <PageErrorBoundary pageName="After Hours">
      <h1 className="mb-4">
        <i className="fas fa-clock me-2"></i>After Hours Management
      </h1>

      <div className="card">
        <div className="card-header d-flex justify-content-between align-items-center">
          <h3><i className="fas fa-edit me-2"></i>After Hours Settings</h3>
          <SaveButton onClick={handleSave} loading={saving} text="Save Changes" />
        </div>
        <div className="card-body">
          {isLoading ? (
            <LoadingSpinner text="Loading after hours settings..." />
          ) : (
            <ComponentErrorBoundary componentName="After Hours Editor">
              <div className="row mb-4">
                <div className="col-md-6">
                  <label className="form-label">Start Time</label>
                  <input
                    type="time"
                    className="form-control"
                    value={formData.restrictedHoursPeriod.start}
                    onChange={(e) => setFormData(prev => ({
                      ...prev,
                      restrictedHoursPeriod: { ...prev.restrictedHoursPeriod, start: e.target.value }
                    }))}
                  />
                </div>
                <div className="col-md-6">
                  <label className="form-label">End Time</label>
                  <input
                    type="time"
                    className="form-control"
                    value={formData.restrictedHoursPeriod.end}
                    onChange={(e) => setFormData(prev => ({
                      ...prev,
                      restrictedHoursPeriod: { ...prev.restrictedHoursPeriod, end: e.target.value }
                    }))}
                  />
                </div>
              </div>

              <div className="d-flex justify-content-between align-items-center mb-3">
                <h5>Exception Events</h5>
                <button className="btn btn-success btn-sm" onClick={addEvent}>
                  <i className="fas fa-plus me-1"></i>Add Event
                </button>
              </div>

              <div className="table-responsive">
                <table className="table table-striped">
                  <thead>
                    <tr>
                      <th>Event Name</th>
                      <th>Event Type</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {formData.exceptionOfValidation.events.map((event, index) => (
                      <tr key={index}>
                        <td>
                          <select
                            className="form-select form-select-sm"
                            value={event.name || ''}
                            onChange={(e) => updateEvent(index, 'name', e.target.value)}
                          >
                            <option value="">Select Event</option>
                            {event.name && !eventTriggers.find((t: any) => t.eventName === event.name) && (
                              <option key={event.name} value={event.name}>
                                {event.name}
                              </option>
                            )}
                            {eventTriggers.map((trigger: any) => (
                              <option key={trigger.eventName} value={trigger.eventName}>
                                {trigger.eventName}
                              </option>
                            ))}
                          </select>
                        </td>
                        <td>
                          <select
                            className="form-select form-select-sm"
                            value={event.type || ''}
                            onChange={(e) => updateEvent(index, 'type', e.target.value)}
                          >
                            <option value="">Select Type</option>
                            {event.type && !orderTypes.find((t: any) => t.name === event.type) && (
                              <option key={event.type} value={event.type}>
                                {event.type}
                              </option>
                            )}
                            {orderTypes.map((orderType: any) => (
                              <option key={orderType.name} value={orderType.name}>
                                {orderType.name}
                              </option>
                            ))}
                          </select>
                        </td>
                        <td>
                          <button
                            className="btn btn-danger btn-sm"
                            onClick={() => removeEvent(index)}
                          >
                            <i className="fas fa-trash"></i>
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </ComponentErrorBoundary>
          )}
        </div>
      </div>
    </PageErrorBoundary>
  );
};