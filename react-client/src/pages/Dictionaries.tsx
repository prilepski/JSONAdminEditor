import React from 'react';
import { useFormState } from '../hooks/useFormState';
import { useErrorHandler } from '../hooks/useErrorHandler';
import { FileType, ValidationError, TableData } from '../types';
import { useDictionaryQuery, useDictionaryMutation } from '../hooks/useDictionaryQuery';
import { DictionarySelector } from '../components/DictionarySelector';
import { FileUpload } from '../components/FileUpload';
import { JsonEditor } from '../components/JsonEditor';

import { PageHeader, TableSkeleton } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

export const Dictionaries: React.FC = () => {
  const { state, updateField } = useFormState({
    selectedFileType: FileType.None,
    validationErrors: [] as ValidationError[],
  });
  const { handleError } = useErrorHandler({ context: 'Dictionaries' });

  const { selectedFileType, validationErrors } = state;

  const { data: dictionaryResponse, isLoading, error } = useDictionaryQuery(selectedFileType);
  const saveMutation = useDictionaryMutation();

  const dictionaryData = dictionaryResponse?.success ? dictionaryResponse.data : null;

  const handleFileTypeChange = (fileType: FileType) => {
    updateField('selectedFileType', fileType);
    updateField('validationErrors', []);
  };

  const handleSave = async (tableData: TableData[]) => {
    if (!dictionaryData) {
      return { success: false, error: 'No dictionary data available' };
    }

    try {
      const result = await saveMutation.mutateAsync({
        filePath: dictionaryData.filePath,
        jsonData: tableData,
        fileType: selectedFileType,
      });

      if (result.success) {
        return { success: true, message: result.message };
      } else {
        if (result.validationErrors) {
          updateField('validationErrors', result.validationErrors);
        }
        return { success: false, error: result.error };
      }
    } catch (error) {
      handleError(error);
      return { success: false, error: 'Error saving dictionary' };
    }
  };

  return (
    <PageErrorBoundary pageName="Dictionaries">
      <PageHeader icon="fa-cog" title="Dictionary Management" />
      {error && handleError(error)}

      <ComponentErrorBoundary componentName="Dictionary Selector">
        <DictionarySelector
          selectedFileType={selectedFileType}
          onFileTypeChange={handleFileTypeChange}
        />
      </ComponentErrorBoundary>

      {selectedFileType !== FileType.None && (
        <div className="card">
          <div className="card-header">
            <div className="row align-items-center">
              <div className="col-md-6">
                <h3>
                  <i className="fas fa-edit me-2"></i>
                  Dictionary Editor
                </h3>
              </div>
              <div className="col-md-6">
                <ComponentErrorBoundary componentName="File Upload">
                  <FileUpload
                    selectedFileType={selectedFileType}
                    onUploadSuccess={(text) => toast.success(text)}
                    onUploadError={(text) => toast.error(text)}
                  />
                </ComponentErrorBoundary>
              </div>
            </div>
          </div>
          <div className="card-body">
            {isLoading ? (
              <TableSkeleton rows={5} columns={4} />
            ) : (
              <ComponentErrorBoundary componentName="JSON Editor">
                <JsonEditor
                  dictionaryData={dictionaryData}
                  selectedFileType={selectedFileType}
                  validationErrors={validationErrors}
                  onSave={handleSave}
                  onClearValidationErrors={() => updateField('validationErrors', [])}
                />
              </ComponentErrorBoundary>
            )}
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};
