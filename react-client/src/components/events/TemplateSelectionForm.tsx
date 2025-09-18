import React from 'react';
import { Template } from '../../types/template';

interface TemplateSelectionFormProps {
  templates: {
    Email?: string;
    Sms?: string;
    Voice?: string;
  };
  availableTemplates: Template[];
  onUpdate: (channel: string, value: string) => void;
}

export const TemplateSelectionForm: React.FC<TemplateSelectionFormProps> = ({
  templates,
  availableTemplates,
  onUpdate,
}) => (
  <div>
    {['Email', 'Sms', 'Voice'].map((channel) => (
      <div key={channel} className="mb-3">
        <label className="form-label">{channel} Template</label>
        <select
          className="form-select"
          value={templates[channel as keyof typeof templates] || ''}
          onChange={(e) => onUpdate(channel, e.target.value)}
        >
          <option value="">Select template...</option>
          {availableTemplates
            .filter((t) => t.channelType === channel)
            .map((template) => (
              <option key={template.templateId} value={template.templateId}>
                {template.templateName}
              </option>
            ))}
        </select>
      </div>
    ))}
  </div>
);
