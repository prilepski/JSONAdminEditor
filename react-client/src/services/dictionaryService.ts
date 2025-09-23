import axios from 'axios';
import { FileType, Template, EventTrigger, EventChannel, OrderType, Customer, LogoUrl, TableData, DictionaryData, ApiResponse, FileUploadRequest } from '../types';
import { createApiError, createServiceError } from '../utils/errorHandler';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
});

const getEndpoint = (fileType: FileType): string => {
  const endpoints = {
    [FileType.Templates]: 'templates',
    [FileType.EventTriggers]: 'event-triggers', 
    [FileType.EventChannels]: 'event-channels',
    [FileType.OrderTypes]: 'order-types',
    [FileType.Customers]: 'customers',
    [FileType.LogoUrls]: 'logo-url'
  };
  const endpoint = endpoints[fileType];
  if (!endpoint) {
    throw new Error(`Unsupported dictionary type: ${fileType}`);
  }
  return endpoint;
};

export const dictionaryService = {
  getAvailableDictionaries: () => {
    return [
      { type: FileType.Templates, name: 'Notification Templates', endpoint: 'templates' },
      { type: FileType.EventTriggers, name: 'Event Triggers', endpoint: 'event-triggers' },
      { type: FileType.EventChannels, name: 'Event Channels', endpoint: 'event-channels' },
      { type: FileType.OrderTypes, name: 'Order Types', endpoint: 'order-types' },
      { type: FileType.Customers, name: 'Customers', endpoint: 'customers' },
      { type: FileType.LogoUrls, name: 'Logo URLs', endpoint: 'logo-url' },
    ];
  },

  getData: async (fileType: FileType): Promise<ApiResponse<DictionaryData>> => {
    try {
      const endpoint = getEndpoint(fileType);
      const response = await api.get<Template[] | EventTrigger[] | EventChannel[] | OrderType[] | Customer[] | LogoUrl[]>(`/dictionaries/${endpoint}`);
      const tableData = response.data || [];
      const rawColumnNames = tableData.length > 0 ? Object.keys(tableData[0]) : [];
      const columnNames = rawColumnNames.map(col => {
        // Convert camelCase to "Title Case"
        return col.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());
      });
      const columnTypes = rawColumnNames.reduce((acc, col, index) => {
        const isActiveCol = col.toLowerCase() === 'isactive';
        const displayName = columnNames[index];
        return { ...acc, [displayName]: isActiveCol ? 'boolean' : 'string' };
      }, {});
      
      // Map API data to display format
      const displayTableData = tableData.map(row => {
        const displayRow: any = {};
        Object.entries(row).forEach(([apiField, value]) => {
          const displayName = apiField.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());
          displayRow[displayName] = value;
        });
        return displayRow;
      });
      
      return { 
        success: true, 
        data: {
          filePath: `${endpoint}.json`,
          fileName: `${endpoint}.json`,
          columnNames,
          columnTypes,
          tableData: displayTableData,
          isValidJson: true
        }
      };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/dictionaries/${getEndpoint(fileType)}`,
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load dictionary data',
        'dictionaryService',
        'getData'
      );
    }
  },

  save: async (
    _filePath: string,
    jsonData: TableData[],
    fileType: FileType
  ): Promise<ApiResponse> => {
    try {
      const endpoint = getEndpoint(fileType);
      // Convert display names back to API field names
      const apiData = jsonData.map(row => {
        const apiRow: any = {};
        Object.entries(row).forEach(([displayName, value]) => {
          // Convert "Title Case" back to camelCase
          const apiFieldName = displayName.replace(/\s+/g, '').replace(/^./, str => str.toLowerCase());
          apiRow[apiFieldName] = value;
        });
        return apiRow;
      });
      await api.put<void>(`/dictionaries/${endpoint}`, apiData);
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          `/dictionaries/${getEndpoint(fileType)}`,
          'PUT'
        );
      }
      throw createServiceError(
        'Failed to save dictionary data',
        'dictionaryService',
        'save'
      );
    }
  },

  upload: async (upload: FileUploadRequest): Promise<ApiResponse> => {
    try {
      const formData = new FormData();
      formData.append('FileType', upload.fileType.toString());
      formData.append('CustomerName', upload.customerName || '');
      if (upload.jsonFile) {
        formData.append('JsonFile', upload.jsonFile);
      }
      await api.post('/dictionaries/upload', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      return { success: true };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          '/dictionaries/upload',
          'POST'
        );
      }
      throw createServiceError(
        'Failed to upload file',
        'dictionaryService',
        'upload'
      );
    }
  },

  getChannelOptions: async (): Promise<string[]> => {
    try {
      const response = await api.get<EventChannel[]>('/dictionaries/event-channels');
      if (response.data) {
        return response.data
          .filter((channel) => channel.isActive)
          .map((channel) => channel.channelName)
          .filter((name): name is string => name != null && name.trim() !== '');
      }
      return ['Email', 'Sms', 'Voice'];
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw createApiError(
          error.response?.data?.message || error.message,
          error.response?.status || 500,
          '/dictionaries/event-channels',
          'GET'
        );
      }
      throw createServiceError(
        'Failed to load channel options',
        'dictionaryService',
        'getChannelOptions'
      );
    }
  },
};
