import { useFormState } from './useFormState';
import { FileType, ValidationError } from '../types';

export const useDictionaryState = () => {
  const { state, updateField } = useFormState({
    selectedFileType: FileType.None,
    validationErrors: [] as ValidationError[],
  });

  const handleFileTypeChange = (fileType: FileType) => {
    updateField('selectedFileType', fileType);
    updateField('validationErrors', []);
  };

  const clearValidationErrors = () => {
    updateField('validationErrors', []);
  };

  const setValidationErrors = (errors: ValidationError[]) => {
    updateField('validationErrors', errors);
  };

  return {
    selectedFileType: state.selectedFileType,
    validationErrors: state.validationErrors,
    handleFileTypeChange,
    clearValidationErrors,
    setValidationErrors,
  };
};