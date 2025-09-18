import axios from 'axios';
import { FileType } from '../types';
import { ApiResponse, DictionaryData, FileUploadViewModel } from '../types/api';

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const dictionaryService = {
  getData: async (fileType: FileType): Promise<ApiResponse<DictionaryData>> => {
    const response = await api.get(`/dictionaries/data?fileType=${fileType}`);
    return response.data;
  },

  save: async (filePath: string, jsonData: Record<string, any>[], fileType: FileType): Promise<ApiResponse> => {
    const formData = new FormData();
    formData.append('filePath', filePath);
    formData.append('jsonData', JSON.stringify(jsonData));
    formData.append('fileType', fileType.toString());
    const response = await api.post('/dictionaries/save', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  upload: async (upload: FileUploadViewModel): Promise<ApiResponse> => {
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
          .filter((row: any) => row['IsActive'] === true || row['IsActive'] === 'true')
          .map((row: any) => row['Channel Name'])
          .filter((name: string) => name && name.trim());
      }
    } catch (error) {
      console.error('Error loading channel options:', error);
    }
    return ['Email', 'Sms', 'Voice'];
  },
};