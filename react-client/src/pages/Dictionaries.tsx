import React from 'react';
import { FileType } from '../types';
import { useDictionaryQuery } from '../hooks/useDictionaryQuery';
import { useDictionaryState } from '../hooks/useDictionaryState';
import { DictionarySelector } from '../components/DictionarySelector';
import { JsonEditor } from '../components/JsonEditor';
import { PageHeader, TableSkeleton } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

export const Dictionaries: React.FC = () => {
  const {
    selectedFileType,
    validationErrors,
    handleFileTypeChange,
    clearValidationErrors,
  } = useDictionaryState();

  const { data: dictionaryResponse, isLoading, error } = useDictionaryQuery(selectedFileType);


  const dictionaryData = dictionaryResponse?.success ? dictionaryResponse.data : null;

  return (
    <PageErrorBoundary pageName="Dictionaries">
      <PageHeader icon="fa-cog" title="Dictionary Management" />
      {error && <div className="alert alert-danger">{String(error)}</div>}

      <ComponentErrorBoundary componentName="Dictionary Selector">
        <DictionarySelector
          selectedFileType={selectedFileType}
          onFileTypeChange={handleFileTypeChange}
        />
      </ComponentErrorBoundary>

      {selectedFileType !== FileType.None && (
        <div className="card">
          <div className="card-header">
            <h3>
              <i className="fas fa-eye me-2"></i>
              Dictionary Viewer (Read-Only)
            </h3>
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
                  onSave={() => Promise.resolve({ success: false, error: 'Read-only mode' })}
                  onClearValidationErrors={clearValidationErrors}
                  readonly={true}
                />
              </ComponentErrorBoundary>
            )}
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};
