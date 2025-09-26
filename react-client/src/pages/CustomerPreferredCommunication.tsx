import React, { useState } from 'react';
import { JsonEditor } from '../components/JsonEditor';
import { useCustomersQuery, useCustomerPreferredCommunicationQuery, useCustomerPreferredCommunicationMutation } from '../hooks/useCustomerQuery';
import { useQuery } from '@tanstack/react-query';
import { useErrorHandler } from '../hooks/useErrorHandler';
import { FileType, PreferredCommunication, Channel } from '../types';
import axios from 'axios';
import { PageHeader, CustomerSelector, LoadingSpinner, TableSkeleton } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

export const CustomerPreferredCommunication: React.FC = () => {
  const [selectedCustomer, setSelectedCustomer] = useState<string>('');
  const { handleError } = useErrorHandler({ context: 'CustomerPreferredCommunication' });

  const { data: customers = [], isLoading: customersLoading } = useCustomersQuery();
  const { data: customerPrefComm = [], isLoading: prefCommLoading } = useCustomerPreferredCommunicationQuery(selectedCustomer);
  const saveMutation = useCustomerPreferredCommunicationMutation();
  
  const { data: eventChannels = [] } = useQuery({
    queryKey: ['eventChannels'],
    queryFn: async () => {
      const response = await axios.get('/api/dictionaries/event-channels');
      return response.data;
    }
  });
  
  const channelOptions = eventChannels.map((ch: any) => ch.channelName).filter(Boolean);

  const columnNames = ['Channel Name', 'Priority'];
  const columnTypes = {
    'Channel Name': 'select',
    Priority: 'number',
  };

  const handleSave = async (tableData: Record<string, any>[]) => {
    if (!selectedCustomer) return { success: false, error: 'No customer selected' };

    try {
      const preferredCommData: PreferredCommunication[] = tableData.map(row => ({
        channel: row['Channel Name'] as Channel,
        priority: Number(row.Priority)
      }));
      
      await saveMutation.mutateAsync({
        customerId: selectedCustomer,
        data: preferredCommData,
      });
      return { success: true };
    } catch (error) {
      handleError(error);
      return { success: false, error: 'Error saving customer preferred communication' };
    }
  };

  const tableData = customerPrefComm.map(item => ({
    'Channel Name': item.channel,
    Priority: item.priority
  }));

  const dictionaryData = {
    columnNames,
    columnTypes,
    tableData,
    filePath: `customer-preferred-communication-${selectedCustomer}`,
    fileName: `customer-preferred-communication-${selectedCustomer}.json`,
    isValidJson: true,
  };

  const isInitialLoading = customersLoading;
  const isPrefCommLoading = selectedCustomer && prefCommLoading;

  return (
    <PageErrorBoundary pageName="Customer Preferred Communication">
      <PageHeader
        icon="fa-comments"
        title="Customer Preferred Communication"
        description="Configure communication channel priorities for specific customers."
      />

      {isInitialLoading ? (
        <LoadingSpinner text="Loading customer data..." />
      ) : (
        <ComponentErrorBoundary componentName="Customer Selector">
          <CustomerSelector
            customers={customers}
            selectedCustomer={selectedCustomer}
            onCustomerChange={setSelectedCustomer}
          />
        </ComponentErrorBoundary>
      )}

      {selectedCustomer && (
        <div className="card">
          <div className="card-header">
            <h3>
              <i className="fas fa-edit me-2"></i>Preferred Communication: {selectedCustomer}
            </h3>
            <small className="text-muted">
              Manage communication channel priorities. Available channels: Email, SMS, Voice.
            </small>
          </div>
          <div className="card-body">
            {isPrefCommLoading ? (
              <TableSkeleton rows={5} columns={5} />
            ) : (
              <ComponentErrorBoundary componentName="Preferred Communication Editor">
                <JsonEditor
                  dictionaryData={dictionaryData}
                  selectedFileType={FileType.Templates}
                  validationErrors={[]}
                  onSave={handleSave}
                  onClearValidationErrors={() => {}}
                  channelOptions={channelOptions}
                />
              </ComponentErrorBoundary>
            )}
          </div>
        </div>
      )}

      {!selectedCustomer && !isInitialLoading && (
        <div className="card">
          <div className="card-body text-center py-4">
            <i className="fas fa-comments fa-3x text-muted mb-3"></i>
            <h4 className="text-muted">Select a Customer</h4>
            <p className="text-muted">
              Choose a customer from the search box above to configure their preferred communication settings.
            </p>
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};