import React, { useState, useEffect } from 'react';
import { useEventTriggersQuery, useOrderTypesQuery, useTemplatesQuery, useEventQuery, useEventSupportQuery, useEventMutation } from '../hooks/useEventQuery';
import toast from 'react-hot-toast';

interface EventData {
  Event?: string;
  OrderType?: string;
  Phone?: string;
  Email?: string;
  Logo?: string;
  IsSuppressed?: boolean;
  Templates?: {
    Email?: string;
    Sms?: string;
    Voice?: string;
  };
  ContentVariables?: Record<string, string>;
}

export const Events: React.FC = () => {
  const [selectedEvent, setSelectedEvent] = useState<string>('');
  const [selectedOrderType, setSelectedOrderType] = useState<string>('Delivery');
  const [activeTab, setActiveTab] = useState<string>('event-data');
  const [isNewEvent, setIsNewEvent] = useState(false);
  const [eventData, setEventData] = useState<EventData | null>(null);
  const { data: activeEventTriggers = [] } = useEventTriggersQuery();
  const { data: availableOrderTypes = [] } = useOrderTypesQuery();
  const { data: availableTemplates = [] } = useTemplatesQuery();
  const [eventSupportsByOrderType, setEventSupportsByOrderType] = useState(false);








  const { data: eventSupports = false } = useEventSupportQuery(selectedEvent);
  const { data: eventInfo, isLoading: eventLoading } = useEventQuery(selectedEvent, selectedOrderType);
  const eventMutation = useEventMutation();
  
  useEffect(() => {
    setEventSupportsByOrderType(eventSupports);
    if (eventInfo) {
      setEventData(eventInfo);
      setIsNewEvent(false);
    } else if (selectedEvent && eventSupports) {
      setIsNewEvent(true);
      setEventData({
        Event: selectedEvent,
        OrderType: selectedOrderType,
        Phone: '$consigneeContact.phone$',
        Email: '$consigneeContact.email$',
        Logo: 'base64',
        IsSuppressed: false,
        Templates: { Email: '', Sms: '', Voice: '' },
        ContentVariables: {}
      });
    }
  }, [selectedEvent, selectedOrderType, eventSupports, eventInfo]);

  const handleSaveEventData = async () => {
    if (!eventData || !selectedEvent) return;

    try {
      const success = await eventMutation.mutateAsync({
        eventName: selectedEvent,
        orderType: selectedOrderType,
        eventData,
        isNew: isNewEvent
      });

      if (success) {
        toast.success(`Event '${selectedEvent}' ${isNewEvent ? 'added' : 'updated'} successfully!`);
        setIsNewEvent(false);
      } else {
        toast.error('Failed to save event data');
      }
    } catch (error) {
      toast.error('Error saving event data');
    }
  };

  const updateEventField = (field: string, value: any) => {
    setEventData(prev => prev ? { ...prev, [field]: value } : null);
  };

  const updateTemplateField = (channel: string, value: string) => {
    setEventData(prev => prev ? {
      ...prev,
      Templates: { ...prev.Templates, [channel]: value }
    } : null);
  };

  return (
    <>
      <h1 className="mb-4">
        <i className="fas fa-calendar-alt me-2"></i>Event Management
      </h1>

      {/* Event Selection */}
      <div className="card mb-4">
        <div className="card-header">
          <h3>Select Event</h3>
        </div>
        <div className="card-body">
          <div className="row">
            <div className="col-md-6">
              <select
                className="form-select"
                value={selectedEvent}
                onChange={(e) => setSelectedEvent(e.target.value)}
              >
                <option value="">Select an event...</option>
                {activeEventTriggers.map(trigger => (
                  <option key={trigger} value={trigger}>{trigger}</option>
                ))}
              </select>
            </div>
            {eventSupportsByOrderType && (
              <div className="col-md-6">
                <select
                  className="form-select"
                  value={selectedOrderType}
                  onChange={(e) => setSelectedOrderType(e.target.value)}
                >
                  {availableOrderTypes.map(type => (
                    <option key={type} value={type}>{type}</option>
                  ))}
                </select>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Event Editor */}
      {selectedEvent && eventData && (
        <div className="card">
          <div className="card-header">
            <ul className="nav nav-tabs card-header-tabs">
              <li className="nav-item">
                <button
                  className={`nav-link ${activeTab === 'event-data' ? 'active' : ''}`}
                  onClick={() => setActiveTab('event-data')}
                >
                  Event Data
                </button>
              </li>
              <li className="nav-item">
                <button
                  className={`nav-link ${activeTab === 'templates' ? 'active' : ''}`}
                  onClick={() => setActiveTab('templates')}
                >
                  Templates
                </button>
              </li>
              <li className="nav-item">
                <button
                  className={`nav-link ${activeTab === 'content-variables' ? 'active' : ''}`}
                  onClick={() => setActiveTab('content-variables')}
                >
                  Content Variables
                </button>
              </li>
            </ul>
          </div>
          <div className="card-body">
            {activeTab === 'event-data' && (
              <div>
                <div className="row mb-3">
                  <div className="col-md-6">
                    <label className="form-label">Order Type</label>
                    <input
                      type="text"
                      className="form-control"
                      value={eventData.OrderType || ''}
                      onChange={(e) => updateEventField('OrderType', e.target.value)}
                    />
                  </div>
                  <div className="col-md-6">
                    <label className="form-label">Phone</label>
                    <input
                      type="text"
                      className="form-control"
                      value={eventData.Phone || ''}
                      onChange={(e) => updateEventField('Phone', e.target.value)}
                    />
                  </div>
                </div>
                <div className="row mb-3">
                  <div className="col-md-6">
                    <label className="form-label">Email</label>
                    <input
                      type="text"
                      className="form-control"
                      value={eventData.Email || ''}
                      onChange={(e) => updateEventField('Email', e.target.value)}
                    />
                  </div>
                  <div className="col-md-6">
                    <label className="form-label">Logo</label>
                    <input
                      type="text"
                      className="form-control"
                      value={eventData.Logo || ''}
                      onChange={(e) => updateEventField('Logo', e.target.value)}
                    />
                  </div>
                </div>
                <div className="mb-3">
                  <div className="form-check">
                    <input
                      type="checkbox"
                      className="form-check-input"
                      checked={eventData.IsSuppressed || false}
                      onChange={(e) => updateEventField('IsSuppressed', e.target.checked)}
                    />
                    <label className="form-check-label">Is Suppressed</label>
                  </div>
                </div>
              </div>
            )}

            {activeTab === 'templates' && (
              <div>
                <div className="mb-3">
                  <label className="form-label">Email Template</label>
                  <select
                    className="form-select"
                    value={eventData.Templates?.Email || ''}
                    onChange={(e) => updateTemplateField('Email', e.target.value)}
                  >
                    <option value="">Select template...</option>
                    {availableTemplates.filter(t => t.templateId.includes('Email')).map(template => (
                      <option key={template.templateId} value={template.templateId}>
                        {template.templateName}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="mb-3">
                  <label className="form-label">SMS Template</label>
                  <select
                    className="form-select"
                    value={eventData.Templates?.Sms || ''}
                    onChange={(e) => updateTemplateField('Sms', e.target.value)}
                  >
                    <option value="">Select template...</option>
                    {availableTemplates.filter(t => t.templateId.includes('Sms')).map(template => (
                      <option key={template.templateId} value={template.templateId}>
                        {template.templateName}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="mb-3">
                  <label className="form-label">Voice Template</label>
                  <select
                    className="form-select"
                    value={eventData.Templates?.Voice || ''}
                    onChange={(e) => updateTemplateField('Voice', e.target.value)}
                  >
                    <option value="">Select template...</option>
                    {availableTemplates.filter(t => t.templateId.includes('Voice')).map(template => (
                      <option key={template.templateId} value={template.templateId}>
                        {template.templateName}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            )}

            {activeTab === 'content-variables' && (
              <div>
                <p className="text-muted">Content variables for this event...</p>
                {/* Content variables editor would go here */}
              </div>
            )}

            <div className="mt-4">
              <button
                type="button"
                className="btn btn-primary"
                onClick={handleSaveEventData}
                disabled={eventMutation.isPending}
              >
                {eventMutation.isPending ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-1"></span>
                    Saving...
                  </>
                ) : (
                  <>
                    <i className="fas fa-save me-1"></i>
                    Save Changes
                  </>
                )}
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};