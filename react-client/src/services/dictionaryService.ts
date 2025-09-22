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
    const columnNames = tableData.length > 0 ? Object.keys(tableData[0]) : [];
    const columnTypes = columnNames.reduce((acc, col) => ({ ...acc, [col]: 'string' }), {});
    
    return { 
      success: true, 
      data: {
        filePath: `${endpoint}.json`,
        fileName: `${endpoint}.json`,
        columnNames,
        columnTypes,
        tableData,
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
    await api.put<void>(`/dictionaries/${endpoint}`, jsonData);
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
