import React from 'react';
import { TemplateField } from '../../types/components';

interface CustomerTemplateTableProps {
  templateFields: TemplateField[];
  onUpdateTemplate: (channel: string, value: string) => void;
  onToggleRedefined: (channel: string, isRedefined: boolean) => void;
}

export const CustomerTemplateTable: React.FC<CustomerTemplateTableProps> = ({
  templateFields,
  onUpdateTemplate,
  onToggleRedefined,
}) => {

  return (
    <div>
      <div className="alert alert-info">
        <i className="fas fa-info-circle me-2"></i>
        <strong>Template Management:</strong> Check "Is Redefined" to override global templates for
        this customer.
      </div>
      <div className="table-responsive">
        <table className="table table-bordered">
          <thead className="table-light">
            <tr>
              <th>Channel Name</th>
              <th>Template Name</th>
              <th>Is Redefined</th>
              <th>Global Value</th>
            </tr>
          </thead>
          <tbody>
            {templateFields.map((template) => (
              <tr key={template.channel}>
                <td>
                  <strong>{template.channel}</strong>
                </td>
                <td>
                  <input
                    type="text"
                    className="form-control"
                    value={template.value}
                    disabled={!template.isRedefined}
                    onChange={(e) => onUpdateTemplate(template.channel, e.target.value)}
                    placeholder="Enter template ID..."
                  />
                </td>
                <td className="text-center">
                  <div className="form-check">
                    <input
                      className="form-check-input"
                      type="checkbox"
                      checked={template.isRedefined}
                      onChange={(e) => onToggleRedefined(template.channel, e.target.checked)}
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
  );
};
