import React from 'react';
import { useFormState } from '../hooks/useFormState';
import { JsonEditor } from '../components/JsonEditor';
import {
  usePreferredCommunicationQuery,
  usePreferredCommunicationMutation,
} from '../hooks/usePreferredCommunicationQuery';
import { useDictionaryQuery } from '../hooks/useDictionaryQuery';
import { ValidationError } from '../types/api';
import toast from 'react-hot-toast';
import { TableSkeleton } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

export const PreferredCommunication: React.FC = () => {
  const { data = [], isLoading } = usePreferredCommunicationQuery();
  const { data: eventChannelsData } = useDictionaryQuery(6); // FileType.EventChannels = 6
  const saveMutation = usePreferredCommunicationMutation();
  
  const channelOptions = eventChannelsData?.data?.tableData?.map((row: any) => {
    // Try different possible field names
    return row['Channel Name'] || row['channelName'] || row['Channel'] || row['channel'];
  }).filter(Boolean) || ['Email', 'SMS', 'Voice'];
  
  console.log('Event Channels Data:', eventChannelsData);
  console.log('Extracted Channel Options:', channelOptions);

  const { state, updateField } = useFormState({
    validationErrors: [] as ValidationError[],
  });

  const { validationErrors } = state;

  const columnNames = ['Channel', 'Priority'];
  const columnTypes = {
    Channel: 'select',
    Priority: 'number',
  };

  const handleSave = async (tableData: Record<string, any>[]) => {
    updateField('validationErrors', []);

    try {
      const result = await saveMutation.mutateAsync(tableData);
      if (result.success) {
        toast.success('Preferred Communication settings saved successfully!');
        return { success: true, message: result.message };
      } else {
        toast.error(result.error || 'Failed to save preferred communication');
        if (result.validationErrors) {
          updateField('validationErrors', result.validationErrors);
        }
        return { success: false, error: result.error };
      }
    } catch (error) {
      const errorMessage = 'Error saving preferred communication';
      toast.error(errorMessage);
      return { success: false, error: errorMessage };
    }
  };

  const dictionaryData = {
    columnNames,
    columnTypes,
    tableData: data,
    filePath: 'preferred-communication',
    fileName: 'preferred-communication.json',
    isValidJson: true,
  };

  return (
    <PageErrorBoundary pageName="Preferred Communication">
      <h1 className="mb-4">
        <i className="fas fa-comments me-2"></i>Preferred Communication Management
      </h1>

      <p className="text-muted mb-4">
        Manage communication channel priorities. Channel values must be unique and exist in the Event Channels dictionary.
      </p>

      <div className="card">
        <div className="card-header">
          <h3>
            <i className="fas fa-edit me-2"></i>Preferred Communication Editor
          </h3>
        </div>
        <div className="card-body">
          {isLoading && !data.length ? (
            <TableSkeleton rows={5} columns={5} />
          ) : (
            <ComponentErrorBoundary componentName="Preferred Communication Editor">
              <JsonEditor
                dictionaryData={dictionaryData}
                selectedFileType={1}
                validationErrors={validationErrors}
                onSave={handleSave}
                onClearValidationErrors={() => updateField('validationErrors', [])}
                channelOptions={channelOptions}
              />
            </ComponentErrorBoundary>
          )}
        </div>
      </div>
    </PageErrorBoundary>
  );
};
