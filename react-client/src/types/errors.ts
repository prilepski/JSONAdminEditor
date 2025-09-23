// Base error interface
export interface BaseError extends Error {
  code?: string;
  status?: number;
  timestamp?: string;
}

// API-specific errors
export interface ApiError extends BaseError {
  status: number;
  endpoint?: string;
  method?: string;
}

// Service layer errors
export interface ServiceError extends BaseError {
  service: string;
  operation: string;
}

// Validation errors
export interface ValidationError extends BaseError {
  field?: string;
  value?: unknown;
  constraints?: string[];
}

// Network errors
export interface NetworkError extends BaseError {
  isNetworkError: true;
  timeout?: boolean;
}

// Authentication errors
export interface AuthError extends BaseError {
  isAuthError: true;
  requiresLogin?: boolean;
}

// Error types enum
export enum ErrorType {
  API = 'API_ERROR',
  SERVICE = 'SERVICE_ERROR',
  VALIDATION = 'VALIDATION_ERROR',
  NETWORK = 'NETWORK_ERROR',
  AUTH = 'AUTH_ERROR',
  UNKNOWN = 'UNKNOWN_ERROR'
}

// Error severity levels
export enum ErrorSeverity {
  LOW = 'low',
  MEDIUM = 'medium',
  HIGH = 'high',
  CRITICAL = 'critical'
}

// Standardized error response
export interface ErrorResponse {
  type: ErrorType;
  severity: ErrorSeverity;
  message: string;
  code?: string;
  status?: number;
  details?: Record<string, unknown>;
  timestamp: string;
}