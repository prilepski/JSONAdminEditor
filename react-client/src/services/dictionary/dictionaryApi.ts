import axios from 'axios';
import { FileType, TableData, ApiResponse, FileUploadRequest, EventChannel } from '../../types';
import { createApiError, createServiceError } from '../../utils/errorHandler';
import { getEndpoint } from './dictionaryConfig';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const fetchDictionaryData = async (fileType: FileType) => {
  try {
    const endpoint = getEndpoint(fileType);
    const response = await api.get(`/dictionaries/${endpoint}`);
    return response.data || [];
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw createApiError(
        error.response?.data?.message || error.message,
        error.response?.status || 500,
        `/dictionaries/${getEndpoint(fileType)}`,
        'GET'
      );
    }
    throw createServiceError('Failed to load dictionary data', 'dictionaryService', 'getData');
  }
};

export const saveDictionaryData = async (jsonData: TableData[], fileType: FileType) => {
  try {
    const endpoint = getEndpoint(fileType);
    await api.put(`/dictionaries/${endpoint}`, jsonData);
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
    throw createServiceError('Failed to save dictionary data', 'dictionaryService', 'save');
  }
};

export const uploadDictionaryFile = async (upload: FileUploadRequest): Promise<ApiResponse> => {
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
    throw createServiceError('Failed to upload file', 'dictionaryService', 'upload');
  }
};

export const fetchChannelOptions = async (): Promise<string[]> => {
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
    throw createServiceError('Failed to load channel options', 'dictionaryService', 'getChannelOptions');
  }
};