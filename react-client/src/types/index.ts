import { ValidationError } from './api';

export enum FileType {
  None = 0,
  Templates = 1,
  Customers = 2,
  EventTriggers = 5,
  EventChannels = 6,
  OrderTypes = 7
}

export interface JsonFileViewModel {
  fileName?: string;
  filePath?: string;
  jsonContent?: string;
  tableData?: Record<string, any>[];
  columnNames?: string[];
  columnTypes?: Record<string, string>;
  errorMessage?: string;
  isValidJson: boolean;
  validationErrors?: ValidationError[];
}

export interface ValidationResult {
  isValid: boolean;
  errors: ValidationError[];
}