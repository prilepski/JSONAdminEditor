import axios from 'axios';
import { FileType, ApiResponse, DictionaryData, FileUploadRequest, TableData } from '../types';
import { components } from '../generated/api';

type Template = components['schemas']['Template'];
type EventTrigger = components['schemas']['EventTrigger'];
type EventChannel = components['schemas']['EventChannel'];
type OrderType = components['schemas']['OrderType'];
type Customer = components['schemas']['Customer'];

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
    [FileType.Customers]: 'customers'
  };
  return endpoints[fileType] || 'templates';
};

export const dictionaryService = {
  getData: async (fileType: FileType): Promise<ApiResponse<DictionaryData>> => {
    const endpoint = getEndpoint(fileType);
    const response = await api.get<Template[] | EventTrigger[] | EventChannel[] | OrderType[] | Customer[]>(`/dictionaries/${endpoint}`);
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
  },

  save: async (
    _filePath: string,
    jsonData: TableData[],
    fileType: FileType
  ): Promise<ApiResponse> => {
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
  },

  upload: async (upload: FileUploadRequest): Promise<ApiResponse> => {
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
    } catch (error) {
      console.error('Error loading channel options:', error);
    }
    return ['Email', 'Sms', 'Voice'];
  },
};
