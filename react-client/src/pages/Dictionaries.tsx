import React from 'react';
import { FileType } from '../types';
import { useDictionaryQuery } from '../hooks/useDictionaryQuery';
import { useDictionaryState } from '../hooks/useDictionaryState';
import { useDictionarySave } from '../hooks/useDictionarySave';
import { DictionarySelector } from '../components/DictionarySelector';
import { DictionaryEditor } from '../components/dictionaries/DictionaryEditor';
import { PageHeader } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

export const Dictionaries: React.FC = () => {
  const {
    selectedFileType,
    validationErrors,
    handleFileTypeChange,
    clearValidationErrors,
    setValidationErrors,
  } = useDictionaryState();

  const { data: dictionaryResponse, isLoading, error } = useDictionaryQuery(selectedFileType);
  const { handleSave } = useDictionarySave(selectedFileType, setValidationErrors);

  const dictionaryData = dictionaryResponse?.success ? dictionaryResponse.data : null;

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
        <DictionaryEditor
          selectedFileType={selectedFileType}
          dictionaryData={dictionaryData}
          validationErrors={validationErrors}
          isLoading={isLoading}
          onSave={(tableData) => handleSave(tableData, dictionaryData)}
          onClearValidationErrors={clearValidationErrors}
        />
      )}
    </PageErrorBoundary>
  );
};
