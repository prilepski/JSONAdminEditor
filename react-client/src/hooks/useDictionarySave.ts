import { useErrorHandler } from './useErrorHandler';
import { useDictionaryMutation } from './useDictionaryQuery';
import { FileType, TableData, ValidationError } from '../types';

export const useDictionarySave = (
  selectedFileType: FileType,
  setValidationErrors: (errors: ValidationError[]) => void
) => {
  const { handleError } = useErrorHandler({ context: 'Dictionaries' });
  const saveMutation = useDictionaryMutation();

  const handleSave = async (tableData: TableData[], dictionaryData: any) => {
    if (!dictionaryData) {
      return { success: false, error: 'No dictionary data available' };
    }

    try {
      const result = await saveMutation.mutateAsync({
        filePath: dictionaryData.filePath,
        jsonData: tableData,
        fileType: selectedFileType,
      });

      if (result.success) {
        return { success: true, message: result.message };
      } else {
        if (result.validationErrors) {
          setValidationErrors(result.validationErrors);
        }
        return { success: false, error: result.error };
      }
    } catch (error) {
      handleError(error);
      return { success: false, error: 'Error saving dictionary' };
    }
  };

  return { handleSave, isLoading: saveMutation.isPending };
};