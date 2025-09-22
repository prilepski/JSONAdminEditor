import React from 'react';
import { JsonEditor } from '../components/JsonEditor';
import {
  usePreferredCommunicationQuery,
  usePreferredCommunicationMutation,
} from '../hooks/usePreferredCommunicationQuery';
import { useQuery } from '@tanstack/react-query';
import { components } from '../generated/api';
import axios from 'axios';
import toast from 'react-hot-toast';
import { TableSkeleton } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

type PreferredCommunication = components['schemas']['PreferredCommunication'];
type Channel = components['schemas']['Channel'];

export const PreferredCommunication: React.FC = () => {
  const { data = [], isLoading } = usePreferredCommunicationQuery();
  const saveMutation = usePreferredCommunicationMutation();
  
  const { data: eventChannels = [] } = useQuery({
    queryKey: ['eventChannels'],
    queryFn: async () => {
      const response = await axios.get('/api/dictionaries/event-channels');
      return response.data;
    }
  });
  
  const channelOptions = eventChannels.map((ch: any) => ch.channelName).filter(Boolean);

  const columnNames = ['Channel', 'Priority'];
  const columnTypes = {
    Channel: 'select',
    Priority: 'number',
  };

  const handleSave = async (tableData: Record<string, any>[]) => {
    try {
      const preferredCommData: PreferredCommunication[] = tableData.map(row => ({
        channel: row.Channel as Channel,
        priority: Number(row.Priority)
      }));
      
      await saveMutation.mutateAsync(preferredCommData);
      toast.success('Preferred Communication settings saved successfully!');
      return { success: true };
    } catch (error) {
      toast.error('Error saving preferred communication');
      return { success: false, error: 'Error saving preferred communication' };
    }
  };

  const tableData = data.map(item => ({
    Channel: item.channel,
    Priority: item.priority
  }));

  const dictionaryData = {
    columnNames,
    columnTypes,
    tableData,
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
        Manage communication channel priorities. Available channels: Email, SMS, Voice.
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
                validationErrors={[]}
                onSave={handleSave}
                onClearValidationErrors={() => {}}
                channelOptions={channelOptions}
              />
            </ComponentErrorBoundary>
          )}
        </div>
      </div>
    </PageErrorBoundary>
  );
};
