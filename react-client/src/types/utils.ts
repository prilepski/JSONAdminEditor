// Type utility functions and guards - currently unused but kept for future use

import { FileType, ValidationError } from './api';
import { TableData } from './table';

// Type guards
export const isValidFileType = (value: number): value is FileType => {
  return Object.values(FileType).includes(value);
};

export const isTableData = (value: unknown): value is TableData => {
  return typeof value === 'object' && value !== null && !Array.isArray(value);
};

export const isValidationError = (value: unknown): value is ValidationError => {
  return (
    typeof value === 'object' &&
    value !== null &&
    'fieldName' in value &&
    'message' in value &&
    typeof (value as ValidationError).fieldName === 'string' &&
    typeof (value as ValidationError).message === 'string'
  );
};
