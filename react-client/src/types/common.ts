// Base utility types
export type Nullable<T> = T | null;
export type Optional<T> = T | undefined;

// Channel types
export type ChannelType = 'Email' | 'Sms' | 'Voice';

// File types enum
export enum FileType {
  None = 0,
  Templates = 1,
  Customers = 2,
  EventTriggers = 5,
  EventChannels = 6,
  OrderTypes = 7,
}

// Base validation error interface
export interface ValidationError {
  rowIndex?: number;
  fieldName: string;
  message: string;
}

// Generic API response wrapper
export interface ApiResponse<TData = unknown> {
  success: boolean;
  message?: string;
  error?: string;
  data?: TData;
  validationErrors?: ValidationError[];
}

// Base table data structure
export interface TableData {
  [key: string]: string | number | boolean;
}

// Validation result
export interface ValidationResult {
  isValid: boolean;
  errors: ValidationError[];
}
