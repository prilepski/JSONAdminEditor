import React, { useState } from 'react';
import { useErrorHandler } from '../hooks/useErrorHandler';
import {
  useContentVariablesQuery,
  useContentVariablesMutation,
} from '../hooks/useContentVariableQuery';

import { PageHeader, SaveButton, LoadingSpinner, ContentVariablesTable, getRedefinedVariables } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

export const ContentVariables: React.FC = () => {
  const { data = {}, isLoading } = useContentVariablesQuery();
  const saveMutation = useContentVariablesMutation();
  const { handleError } = useErrorHandler({ context: 'ContentVariables' });
  
  const [contentVariables, setContentVariables] = useState<Record<string, string>>({});
  const [redefinedStates, setRedefinedStates] = useState<Record<string, boolean>>({});

  React.useEffect(() => {
    setContentVariables(data);
  }, [data]);

  const handleSave = async () => {
    try {
      // For global content variables, save all variables (not just redefined ones)
      await saveMutation.mutateAsync(contentVariables);
    } catch (error) {
      handleError(error, 'Failed to save content variables');
    }
  };

  return (
    <PageErrorBoundary pageName="Content Variables">
      <PageHeader
        icon="fa-tags"
        title="Content Variables Management"
        description="Manage global content variables that can be used across all notification templates and events."
      />

      {isLoading ? (
        <LoadingSpinner text="Loading content variables..." />
      ) : (
        <div className="card">
          <div className="card-header">
            <h3>
              <i className="fas fa-edit me-2"></i>Global Content Variables
            </h3>
          </div>
          <div className="card-body">
            <ComponentErrorBoundary componentName="Content Variables Editor">
              <ContentVariablesTable
                contentVariables={contentVariables}
                onUpdate={setContentVariables}
                onRedefinedStatesChange={setRedefinedStates}
                title="Global Content Variables"
                showAddButton={true}
                saveButton={
                  <SaveButton
                    onClick={handleSave}
                    loading={saveMutation.isPending}
                    text="Save Variables"
                  />
                }
              />
            </ComponentErrorBoundary>
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};
