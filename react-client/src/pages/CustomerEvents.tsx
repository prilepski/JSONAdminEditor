import React, { useState, useEffect } from 'react';
import { useCustomersQuery, useCustomerEventsQuery, useCustomerEventsMutation } from '../hooks/useCustomerQuery';
import { useEventTriggersQuery, useTemplatesQuery } from '../hooks/useEventQuery';
import toast from 'react-hot-toast';
import { Template } from '../types/template';
import { CustomerEventData, EventField, TemplateField, ContentVariable } from '../types/customerEvent';
import { PageHeader, CustomerSelector, SaveButton, TabNavigation } from '../components/common';

export const CustomerEvents: React.FC = () => {
  const [selectedCustomer, setSelectedCustomer] = useState<string>('');
  const [selectedEvent, setSelectedEvent] = useState<string>('');
  const [selectedOrderType, setSelectedOrderType] = useState<string>('Delivery');
  const [activeTab, setActiveTab] = useState<string>('event-data');
  const [eventFields, setEventFields] = useState<EventField[]>([]);
  const [templateFields, setTemplateFields] = useState<TemplateField[]>([]);
  const [contentVariables, setContentVariables] = useState<Record<string, ContentVariable>>({});
  
  const { data: customers = [] } = useCustomersQuery();
  const { data: activeEventTriggers = [] } = useEventTriggersQuery();
  const { data: availableTemplates = [] } = useTemplatesQuery();
  const { data: customerEventData } = useCustomerEventsQuery(selectedCustomer);
  const saveMutation = useCustomerEventsMutation();

  useEffect(() => {
    if (selectedEvent && selectedCustomer) {
      // Find existing customer event data
      const existingEvent = Array.isArray(customerEventData) 
        ? customerEventData.find((event: any) => 
            event.Event === selectedEvent && event.OrderType === selectedOrderType
          )
        : customerEventData;

      // Initialize event fields with existing data or defaults
      const fields: EventField[] = [
        { 
          name: 'Phone', 
          value: existingEvent?.Phone || '$consigneeContact.phone$', 
          isRedefined: !!existingEvent?.Phone, 
          type: 'text',
          globalValue: '$consigneeContact.phone$'
        },
        { 
          name: 'Email', 
          value: existingEvent?.Email || '$consigneeContact.email$', 
          isRedefined: !!existingEvent?.Email, 
          type: 'text',
          globalValue: '$consigneeContact.email$'
        },
        { 
          name: 'Logo', 
          value: existingEvent?.Logo || 'base64', 
          isRedefined: !!existingEvent?.Logo, 
          type: 'text',
          globalValue: 'base64'
        },
        { 
          name: 'IsSuppressed', 
          value: existingEvent?.IsSuppressed ? 'true' : 'false', 
          isRedefined: existingEvent?.IsSuppressed !== undefined, 
          type: 'checkbox',
          globalValue: 'false'
        },
      ];
      setEventFields(fields);

      // Initialize template fields with existing data
      const templates: TemplateField[] = [
        { 
          channel: 'Email', 
          value: existingEvent?.Templates?.Email || '', 
          isRedefined: !!existingEvent?.Templates?.Email,
          globalValue: ''
        },
        { 
          channel: 'Sms', 
          value: existingEvent?.Templates?.Sms || '', 
          isRedefined: !!existingEvent?.Templates?.Sms,
          globalValue: ''
        },
        { 
          channel: 'Voice', 
          value: existingEvent?.Templates?.Voice || '', 
          isRedefined: !!existingEvent?.Templates?.Voice,
          globalValue: ''
        },
      ];
      setTemplateFields(templates);

      // Initialize content variables with existing data
      const vars: Record<string, ContentVariable> = {};
      if (existingEvent?.ContentVariables) {
        Object.entries(existingEvent.ContentVariables).forEach(([key, value]) => {
          vars[key] = {
            name: key,
            value: String(value),
            isRedefined: true
          };
        });
      }
      setContentVariables(vars);
    }
  }, [selectedEvent, selectedCustomer, selectedOrderType, customerEventData]);

  const handleSave = async () => {
    if (!selectedCustomer || !selectedEvent) return;

    const saveData: CustomerEventData = {
      Event: selectedEvent,
      OrderType: selectedOrderType,
    };

    // Add redefined event fields
    eventFields.forEach(field => {
      if (field.isRedefined) {
        if (field.type === 'checkbox') {
          (saveData as any)[field.name] = field.value === 'true';
        } else {
          (saveData as any)[field.name] = field.value;
        }
      }
    });

    // Add redefined templates
    const templates: Record<string, string> = {};
    templateFields.forEach(template => {
      if (template.isRedefined) {
        templates[template.channel] = template.value;
      }
    });
    if (Object.keys(templates).length > 0) {
      saveData.Templates = templates;
    }

    // Add content variables
    const vars: Record<string, string> = {};
    Object.entries(contentVariables).forEach(([key, data]) => {
      if (data.isRedefined || !data.globalValue) {
        vars[key] = data.value;
      }
    });
    if (Object.keys(vars).length > 0) {
      saveData.ContentVariables = vars;
    }

    try {
      const success = await saveMutation.mutateAsync({ customerId: selectedCustomer, data: saveData });
      if (success) {
        toast.success(`Customer event '${selectedEvent}' saved successfully!`);
      } else {
        toast.error('Failed to save customer event');
      }
    } catch (error) {
      toast.error('Error saving customer event');
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

  const updateTemplateField = (channel: 'Email' | 'Sms' | 'Voice', value: string) => {
    setTemplateFields(prev => prev.map(template => 
      template.channel === channel ? { ...template, value } : template
    ));
  };

  const toggleTemplateRedefined = (channel: 'Email' | 'Sms' | 'Voice', isRedefined: boolean) => {
    setTemplateFields(prev => prev.map(template => 
      template.channel === channel ? { ...template, isRedefined } : template
    ));
  };

  const addContentVariable = () => {
    const newKey = `new_var_${Date.now()}`;
    setContentVariables(prev => ({
      ...prev,
      [newKey]: { name: newKey, value: '', isRedefined: true }
    }));
  };

  const updateContentVariable = (oldKey: string, newKey: string, value: string) => {
    setContentVariables(prev => {
      const newVars = { ...prev };
      if (oldKey !== newKey) {
        delete newVars[oldKey];
      }
      newVars[newKey] = { ...prev[oldKey], name: newKey, value };
      return newVars;
    });
  };

  const removeContentVariable = (key: string) => {
    setContentVariables(prev => {
      const newVars = { ...prev };
      delete newVars[key];
      return newVars;
    });
  };

  const getTemplatesByChannel = (channel: 'Email' | 'Sms' | 'Voice'): Template[] => {
    return availableTemplates.filter(t => t.channelType === channel);
  };

  const tabs = [
    { id: 'event-data', label: 'Event Data', icon: 'fa-cog' },
    { id: 'templates', label: 'Templates', icon: 'fa-file-alt' },
    { id: 'content-variables', label: 'Content Variables', icon: 'fa-code' }
  ];

  return (
    <>
      <PageHeader icon="fa-calendar-alt" title="Customer Events Management" />

      <CustomerSelector
        customers={customers}
        selectedCustomer={selectedCustomer}
        onCustomerChange={setSelectedCustomer}
      />

      {/* Event Selection */}
      {selectedCustomer && (
        <div className="card mb-4">
          <div className="card-header">
            <h3><i className="fas fa-calendar-alt me-2"></i>Select Event for {selectedCustomer}</h3>
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
              <div className="col-md-6">
                <div className="d-flex align-items-center">
                  <span className="me-2 text-muted">Delivery</span>
                  <div className="form-check form-switch">
                    <input 
                      className="form-check-input" 
                      type="checkbox" 
                      checked={selectedOrderType === 'Pickup'}
                      onChange={(e) => setSelectedOrderType(e.target.checked ? 'Pickup' : 'Delivery')}
                    />
                  </div>
                  <span className="ms-2 text-muted">Pickup</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Event Editor */}
      {selectedCustomer && selectedEvent && (
        <div className="card">
          <div className="card-header">
            <div className="d-flex justify-content-between align-items-center">
              <h3 className="mb-0">
                <i className="fas fa-edit me-2"></i>Edit Event: {selectedEvent} ({selectedOrderType})
              </h3>
              <SaveButton
                onClick={handleSave}
                loading={saveMutation.isPending}
                text="Save Event"
              />
            </div>
          </div>
          <div className="card-body">
            <TabNavigation
              tabs={tabs}
              activeTab={activeTab}
              onTabChange={setActiveTab}
            />

            {/* Event Data Tab */}
            {activeTab === 'event-data' && (
              <div>
                <div className="alert alert-info">
                  <i className="fas fa-info-circle me-2"></i>
                  <strong>Event Data Management:</strong> Check "Is Redefined" to override global settings for this customer.
                </div>
                <div className="table-responsive">
                  <table className="table table-bordered">
                    <thead className="table-light">
                      <tr>
                        <th>Field</th>
                        <th>Value</th>
                        <th>Is Redefined</th>
                        <th>Global Value</th>
                      </tr>
                    </thead>
                    <tbody>
                      {eventFields.map(field => (
                        <tr key={field.name}>
                          <td><strong>{field.name}</strong></td>
                          <td>
                            {field.type === 'checkbox' ? (
                              <div className="form-check">
                                <input
                                  className="form-check-input"
                                  type="checkbox"
                                  checked={field.value === 'true'}
                                  disabled={!field.isRedefined}
                                  onChange={(e) => updateEventField(field.name, String(e.target.checked))}
                                />
                              </div>
                            ) : (
                              <input
                                type="text"
                                className="form-control"
                                value={field.value}
                                readOnly={!field.isRedefined}
                                onChange={(e) => updateEventField(field.name, e.target.value)}
                              />
                            )}
                          </td>
                          <td className="text-center">
                            <div className="form-check">
                              <input
                                className="form-check-input"
                                type="checkbox"
                                checked={field.isRedefined}
                                onChange={(e) => toggleEventFieldRedefined(field.name, e.target.checked)}
                              />
                            </div>
                          </td>
                          <td>
                            <span className="text-muted">{field.globalValue || '—'}</span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}

            {/* Templates Tab */}
            {activeTab === 'templates' && (
              <div>
                <div className="alert alert-info">
                  <i className="fas fa-info-circle me-2"></i>
                  <strong>Template Management:</strong> Check "Is Redefined" to override global templates for this customer.
                </div>
                <div className="table-responsive">
                  <table className="table table-bordered">
                    <thead className="table-light">
                      <tr>
                        <th>Channel</th>
                        <th>Template</th>
                        <th>Is Redefined</th>
                        <th>Global Value</th>
                      </tr>
                    </thead>
                    <tbody>
                      {templateFields.map(template => (
                        <tr key={template.channel}>
                          <td><strong>{template.channel}</strong></td>
                          <td>
                            <select
                              className="form-select"
                              value={template.value}
                              disabled={!template.isRedefined}
                              onChange={(e) => updateTemplateField(template.channel, e.target.value)}
                            >
                              <option value="">Select template...</option>
                              {getTemplatesByChannel(template.channel).map(t => (
                                <option key={t.templateId} value={t.templateId}>
                                  {t.templateName} ({t.templateId})
                                </option>
                              ))}
                            </select>
                          </td>
                          <td className="text-center">
                            <div className="form-check">
                              <input
                                className="form-check-input"
                                type="checkbox"
                                checked={template.isRedefined}
                                onChange={(e) => toggleTemplateRedefined(template.channel, e.target.checked)}
                              />
                            </div>
                          </td>
                          <td>
                            <span className="text-muted">{template.globalValue || '—'}</span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}

            {/* Content Variables Tab */}
            {activeTab === 'content-variables' && (
              <div>
                <div className="d-flex justify-content-between align-items-center mb-3">
                  <h5><i className="fas fa-code me-2"></i>Content Variables</h5>
                  <button
                    type="button"
                    className="btn btn-outline-primary"
                    onClick={addContentVariable}
                  >
                    <i className="fas fa-plus me-1"></i>Add Variable
                  </button>
                </div>
                <div className="table-responsive">
                  <table className="table table-bordered">
                    <thead className="table-light">
                      <tr>
                        <th>Variable Name</th>
                        <th>Value</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {Object.entries(contentVariables).map(([key, data]) => (
                        <tr key={key}>
                          <td>
                            <input
                              type="text"
                              className="form-control"
                              value={key}
                              onChange={(e) => updateContentVariable(key, e.target.value, data.value)}
                            />
                          </td>
                          <td>
                            <input
                              type="text"
                              className="form-control"
                              value={data.value}
                              onChange={(e) => updateContentVariable(key, key, e.target.value)}
                            />
                          </td>
                          <td className="text-center">
                            <button
                              type="button"
                              className="btn btn-outline-danger btn-sm"
                              onClick={() => removeContentVariable(key)}
                            >
                              <i className="fas fa-trash"></i>
                            </button>
                          </td>
                        </tr>
                      ))}
                      {Object.keys(contentVariables).length === 0 && (
                        <tr>
                          <td colSpan={3} className="text-center text-muted py-4">
                            No content variables. Click "Add Variable" to create one.
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </>
  );
};