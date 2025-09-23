import React from 'react';
import { JsonEditor } from '../components/JsonEditor';
import { useOptOutQuery, useOptOutMutation } from '../hooks/useOptOutQuery';
import { FileType, ValidationError } from '../types';
import { useFormState } from '../hooks/useFormState';
import toast from 'react-hot-toast';
import { PageHeader, TableSkeleton } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

const OptOutPage: React.FC = () => {
  const { data, isLoading } = useOptOutQuery();
  const saveMutation = useOptOutMutation();
  const { state, updateField } = useFormState({ validationErrors: [] as ValidationError[] });

  const handleSave = async (tableData: any[]) => {
    updateField('validationErrors', []);
    try {
      await saveMutation.mutateAsync(tableData);
      toast.success('Opt-out configuration saved successfully!');
      return { success: true };
    } catch (error) {
      toast.error('Error saving opt-out configuration');
      return { success: false, error: 'Error saving opt-out configuration' };
    }
  };

  return (
    <PageErrorBoundary pageName="Opt-Out Configuration">
      <PageHeader
        icon="fa-ban"
        title="Opt-Out Configuration"
        description="Configure opt-out and opt-in keywords, phrases, and messages by language and country."
      />
      <div className="card">
        <div className="card-header">
          <h3><i className="fas fa-edit me-2"></i>Opt-Out Settings Editor</h3>
        </div>
        <div className="card-body">
          {isLoading ? (
            <TableSkeleton rows={3} columns={4} />
          ) : (
            <ComponentErrorBoundary componentName="Opt-Out Editor">
              <JsonEditor
                dictionaryData={data}
                selectedFileType={FileType.Templates}
                validationErrors={state.validationErrors}
                onSave={handleSave}
                onClearValidationErrors={() => updateField('validationErrors', [])}
              />
            </ComponentErrorBoundary>
          )}
        </div>
      </div>
    </PageErrorBoundary>
  );
};

export { OptOutPage as OptOut };