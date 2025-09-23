import { 
  ApiError, 
  ServiceError, 
  ValidationError, 
  NetworkError, 
  AuthError,
  ErrorType,
  ErrorSeverity,
  ErrorResponse 
} from '../types/errors';

// Error classification
export const classifyError = (error: unknown): ErrorType => {
  if (isNetworkError(error)) return ErrorType.NETWORK;
  if (isAuthError(error)) return ErrorType.AUTH;
  if (isValidationError(error)) return ErrorType.VALIDATION;
  if (isApiError(error)) return ErrorType.API;
  return ErrorType.UNKNOWN;
};

// Type guards
export const isApiError = (error: unknown): error is ApiError => {
  return error instanceof Error && 'status' in error && typeof (error as any).status === 'number';
};

export const isNetworkError = (error: unknown): error is NetworkError => {
  return error instanceof Error && (
    error.message.includes('Network Error') ||
    error.message.includes('fetch') ||
    (error as any).code === 'NETWORK_ERROR'
  );
};

export const isAuthError = (error: unknown): error is AuthError => {
  return isApiError(error) && (error.status === 401 || error.status === 403);
};

export const isValidationError = (error: unknown): error is ValidationError => {
  return error instanceof Error && 'field' in error;
};

// Error severity determination
export const getErrorSeverity = (error: unknown): ErrorSeverity => {
  if (isAuthError(error)) return ErrorSeverity.HIGH;
  if (isNetworkError(error)) return ErrorSeverity.MEDIUM;
  if (isApiError(error)) {
    if (error.status >= 500) return ErrorSeverity.HIGH;
    if (error.status >= 400) return ErrorSeverity.MEDIUM;
  }
  return ErrorSeverity.LOW;
};

// Main error handler
export const handleError = (error: unknown, context?: string): ErrorResponse => {
  const type = classifyError(error);
  const severity = getErrorSeverity(error);
  const timestamp = new Date().toISOString();
  
  let message = 'An unexpected error occurred';
  let code: string | undefined;
  let status: number | undefined;
  let details: Record<string, unknown> | undefined;

  if (error instanceof Error) {
    message = sanitizeErrorMessage(error.message);
    
    if (isApiError(error)) {
      status = error.status;
      code = error.code;
      details = {
        endpoint: error.endpoint,
        method: error.method
      };
    } else if (isServiceError(error)) {
      code = error.code;
      details = {
        service: error.service,
        operation: error.operation
      };
    } else if (isValidationError(error)) {
      code = error.code;
      details = {
        field: error.field,
        value: error.value,
        constraints: error.constraints
      };
    }
  }

  if (context) {
    details = { ...details, context };
  }

  return {
    type,
    severity,
    message,
    code,
    status,
    details,
    timestamp
  };
};

// Service error creator
export const createServiceError = (
  message: string,
  service: string,
  operation: string,
  code?: string
): ServiceError => ({
  name: 'ServiceError',
  message: sanitizeErrorMessage(message),
  service,
  operation,
  code,
  timestamp: new Date().toISOString()
});

// API error creator
export const createApiError = (
  message: string,
  status: number,
  endpoint?: string,
  method?: string,
  code?: string
): ApiError => ({
  name: 'ApiError',
  message: sanitizeErrorMessage(message),
  status,
  endpoint,
  method,
  code,
  timestamp: new Date().toISOString()
});

// Validation error creator
export const createValidationError = (
  message: string,
  field?: string,
  value?: unknown,
  constraints?: string[]
): ValidationError => ({
  name: 'ValidationError',
  message: sanitizeErrorMessage(message),
  field,
  value,
  constraints,
  timestamp: new Date().toISOString()
});

// Message sanitization
export const sanitizeErrorMessage = (message: string): string => {
  return message
    .replace(/[\r\n\t]/g, ' ')
    .replace(/[<>"'&]/g, '')
    .trim()
    .substring(0, 500);
};

// Service error type guard
const isServiceError = (error: unknown): error is ServiceError => {
  return error instanceof Error && 'service' in error && 'operation' in error;
};

// User-friendly error messages
export const getUserFriendlyMessage = (error: ErrorResponse): string => {
  switch (error.type) {
    case ErrorType.NETWORK:
      return 'Connection problem. Please check your internet connection and try again.';
    case ErrorType.AUTH:
      return 'Authentication required. Please log in and try again.';
    case ErrorType.VALIDATION:
      return `Invalid data: ${error.message}`;
    case ErrorType.API:
      if (error.status === 404) return 'The requested resource was not found.';
      if (error.status === 500) return 'Server error. Please try again later.';
      return error.message;
    default:
      return 'Something went wrong. Please try again.';
  }
};
