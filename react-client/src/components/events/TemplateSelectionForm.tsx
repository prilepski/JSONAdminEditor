import React from 'react';
import { Template } from '../../types';

interface TemplateSelectionFormProps {
  templates: Record<string, string>;
  availableTemplates: Template[];
  onUpdate: (channel: string, value: string) => void;
}

export const TemplateSelectionForm: React.FC<TemplateSelectionFormProps> = ({
  templates,
  availableTemplates,
  onUpdate,
}) => {
  const channels = ['Email', 'Sms', 'Voice'];

  return (
    <div className="table-responsive">
      <table className="table table-bordered">
        <thead className="table-light">
          <tr>
            <th style={{ width: '150px' }}>Channel</th>
            <th>Template</th>
          </tr>
        </thead>
        <tbody>
          {channels.map((channel) => (
            <tr key={channel}>
              <td><strong>{channel}</strong></td>
              <td>
                <select
                  className="form-select"
                  value={templates[channel] || ''}
                  onChange={(e) => onUpdate(channel, e.target.value)}
                >
                  <option value="">Select template...</option>
                  {templates[channel] && !availableTemplates.find(t => t.templateId === templates[channel]) && (
                    <option value={templates[channel]} style={{ color: '#dc3545' }}>
                      Template not found: {templates[channel]}
                    </option>
                  )}
                  {availableTemplates
                    .filter((t) => t.channelType === channel)
                    .map((template) => (
                      <option key={template.templateId} value={template.templateId}>
                        {template.templateName}
                      </option>
                    ))}
                </select>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
