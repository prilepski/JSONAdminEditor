import { ValidationError } from './api';

// Base utility types
export type Nullable<T> = T | null;
export type Optional<T> = T | undefined;

// Channel types
export type ChannelType = 'Email' | 'Sms' | 'Voice';

// Validation result
export interface ValidationResult {
  isValid: boolean;
  errors: ValidationError[];
}
