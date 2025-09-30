import React, { useState } from 'react';
import { PageHeader, SaveButton, LoadingSpinner } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { useOptOutQuery, useOptOutMutation } from '../hooks/useOptOutQuery';
import { useErrorHandler } from '../hooks/useErrorHandler';
import { OptOutConfig, OptOutLocale } from '../types';

export const OptOut: React.FC = () => {
  const [config, setConfig] = useState<OptOutConfig>({});
  const { data = {}, isLoading } = useOptOutQuery();
  const mutation = useOptOutMutation();
  const { handleError } = useErrorHandler({ context: 'OptOut' });

  React.useEffect(() => {
    setConfig(data);
  }, [data]);

  const handleSave = async () => {
    try {
      await mutation.mutateAsync(config);
    } catch (error) {
      handleError(error, 'Failed to save opt-out configuration');
    }
  };

  const updateField = (category: string, locale: string, field: keyof OptOutLocale, value: string) => {
    setConfig(prev => ({
      ...prev,
      [category]: {
        ...prev[category],
        [locale]: {
          ...prev[category]?.[locale],
          [field]: value
        }
      }
    }));
  };

  const fieldLabels: Record<keyof OptOutLocale, string> = {
    optOutKeywords: 'Opt-Out Keywords',
    optInKeywords: 'Opt-In Keywords', 
    helpKeywords: 'Help Keywords',
    optOutFooter: 'Opt-Out Footer',
    optOutPhrase: 'Opt-Out Phrase',
    optInPhrase: 'Opt-In Phrase',
    optInMessage: 'Opt-In Message',
    helpPhrase: 'Help Phrase'
  };

  return (
    <PageErrorBoundary pageName="Opt-Out Configuration">
      <PageHeader
        icon="fa-ban"
        title="Opt-Out Configuration"
        description="Configure opt-out settings and messages for SMS notifications."
      />
      
      {isLoading ? (
        <LoadingSpinner text="Loading opt-out configuration..." />
      ) : (
        <div className="card">
          <div className="card-header">
            <div className="d-flex justify-content-between align-items-center">
              <h3>
                <i className="fas fa-edit me-2"></i>Opt-Out Settings
              </h3>
              <SaveButton
                onClick={handleSave}
                loading={mutation.isPending}
                text="Save Changes"
              />
            </div>
          </div>
          <div className="card-body">
            <ComponentErrorBoundary componentName="Opt-Out Editor">
              {Object.keys(config).length === 0 ? (
                <div className="text-center py-4">
                  <i className="fas fa-ban fa-3x text-muted mb-3"></i>
                  <h5 className="text-muted">No Configuration</h5>
                  <p className="text-muted">
                    No opt-out configuration found.
                  </p>
                </div>
              ) : (
                Object.entries(config).map(([category, locales]) => (
                  <div key={category} className="mb-4">
                    <h5 className="border-bottom pb-2 mb-3">
                      <i className="fas fa-folder me-2"></i>{category}
                    </h5>
                    {Object.entries(locales || {}).map(([locale, localeData]) => (
                      <div key={`${category}-${locale}`} className="mb-4">
                        <h6 className="text-primary mb-3">
                          <i className="fas fa-globe me-2"></i>{locale}
                        </h6>
                        <div className="table-responsive">
                          <table className="table table-striped table-hover">
                            <thead className="table-dark">
                              <tr>
                                <th style={{ width: '30%' }}>Field</th>
                                <th style={{ width: '70%' }}>Value</th>
                              </tr>
                            </thead>
                            <tbody>
                              {Object.entries(fieldLabels).map(([field, label]) => (
                                <tr key={field}>
                                  <td>
                                    <input
                                      type="text"
                                      className="form-control form-control-sm bg-light"
                                      value={`${label} (${field})`}
                                      readOnly
                                    />
                                  </td>
                                  <td>
                                    <input
                                      type="text"
                                      className="form-control form-control-sm"
                                      value={localeData?.[field as keyof OptOutLocale] || ''}
                                      onChange={(e) => updateField(category, locale, field as keyof OptOutLocale, e.target.value)}
                                      placeholder={`Enter ${label.toLowerCase()}`}
                                    />
                                  </td>
                                </tr>
                              ))}
                            </tbody>
                          </table>
                        </div>
                      </div>
                    ))}
                  </div>
                ))
              )}
            </ComponentErrorBoundary>
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};