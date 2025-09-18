import React, { useState } from 'react';
import { JsonEditor } from '../components/JsonEditor';
import { useContentVariablesQuery, useContentVariablesMutation } from '../hooks/useContentVariableQuery';
import { ValidationError, TableData } from '../types';
import toast from 'react-hot-toast';
import { Skeleton } from '../components/Skeleton';

export const ContentVariables: React.FC = () => {
  const { data = [], isLoading } = useContentVariablesQuery();
  const saveMutation = useContentVariablesMutation();

  const [validationErrors, setValidationErrors] = useState<ValidationError[]>([]);

  const columnNames = ['Variable Name', 'Variable Value', 'Description'];
  const columnTypes = {
    'Variable Name': 'text',
    'Variable Value': 'text',
    'Description': 'text'
  };





  const handleSave = async (tableData: TableData[]) => {
    setValidationErrors([]);
    
    try {
      const result = await saveMutation.mutateAsync(tableData);
      if (result.success) {
        toast.success('Content Variables saved successfully!');
        return { success: true, message: result.message };
      } else {
        toast.error(result.error || 'Failed to save content variables');
        if (result.validationErrors) {
          setValidationErrors(result.validationErrors);
        }
        return { success: false, error: result.error };
      }
    } catch (error) {
      const errorMessage = 'Error saving content variables';
      toast.error(errorMessage);
      return { success: false, error: errorMessage };
    }
  };

  const dictionaryData = {
    columnNames,
    columnTypes,
    tableData: data,
    filePath: 'content-variables',
    fileName: 'content-variables.json',
    isValidJson: true
  };

  return (
    <>
      <h1 className="mb-4">
        <i className="fas fa-tags me-2"></i>Content Variables Management
      </h1>

      <p className="text-muted mb-4">
        Manage global content variables that can be used across all notification templates and events.
      </p>

      <div className="card">
        <div className="card-header">
          <h3>
            <i className="fas fa-edit me-2"></i>Content Variables Editor
          </h3>
        </div>
        <div className="card-body">
          <Skeleton loading={isLoading && !data.length} rows={5} height="40px">
            <JsonEditor
              dictionaryData={dictionaryData}
              selectedFileType={1}
              validationErrors={validationErrors}
              onSave={handleSave}
              onClearValidationErrors={() => setValidationErrors([])}
            />
          </Skeleton>
        </div>
      </div>
    </>
  );
};