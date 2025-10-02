import { FileType, DictionaryData, ApiResponse } from '../types';
import { AVAILABLE_DICTIONARIES, getEndpoint } from './dictionary/dictionaryConfig';
import { transformApiToDisplay, transformDisplayToApi } from './dictionary/dataTransformers';
import { fetchDictionaryData, saveDictionaryData, uploadDictionaryFile, fetchChannelOptions } from './dictionary/dictionaryApi';

export const dictionaryService = {
  getAvailableDictionaries: () => AVAILABLE_DICTIONARIES,

  getData: async (fileType: FileType): Promise<ApiResponse<DictionaryData>> => {
    const endpoint = getEndpoint(fileType);
    const apiData = await fetchDictionaryData(fileType);
    const { columnNames, columnTypes, tableData } = transformApiToDisplay(apiData, fileType);
    
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

  save: async (_filePath: string, jsonData: any[], fileType: FileType): Promise<ApiResponse> => {
    const apiData = transformDisplayToApi(jsonData, fileType);
    return await saveDictionaryData(apiData, fileType);
  },

  upload: uploadDictionaryFile,

  getChannelOptions: fetchChannelOptions,
};
