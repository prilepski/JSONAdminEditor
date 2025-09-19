import axios from 'axios';
import { FileType, ApiResponse, DictionaryData, FileUploadRequest, TableData } from '../types';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const dictionaryService = {
  getData: async (fileType: FileType): Promise<ApiResponse<DictionaryData>> => {
    const response = await api.get(`/dictionaries/data?fileType=${fileType}`);
    return response.data;
  },

  save: async (
    filePath: string,
    jsonData: TableData[],
    fileType: FileType
  ): Promise<ApiResponse> => {
    const formData = new FormData();
    formData.append('filePath', filePath);
    formData.append('jsonData', JSON.stringify(jsonData));
    formData.append('fileType', fileType.toString());
    
    const response = await api.post('/dictionaries/save', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  upload: async (upload: FileUploadRequest): Promise<ApiResponse> => {
    const formData = new FormData();
    formData.append('Upload.FileType', upload.fileType.toString());
    formData.append('Upload.CustomerName', upload.customerName || '');
    if (upload.jsonFile) {
      formData.append('Upload.JsonFile', upload.jsonFile);
    }
    const response = await api.post('/dictionaries/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  getChannelOptions: async (): Promise<string[]> => {
    try {
      const response = await api.get('/dictionaries/data?fileType=6');
      if (response.data.success && response.data.data?.tableData) {
        return response.data.data.tableData
          .filter((row: TableData) => row['IsActive'] === true || row['IsActive'] === 'true')
          .map((row: TableData) => row['Channel Name'] as string)
          .filter((name: string) => name && name.trim());
      }
    } catch (error) {
      console.error('Error loading channel options:', error);
    }
    return ['Email', 'Sms', 'Voice'];
  },
};
