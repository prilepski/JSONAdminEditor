# Error Handling System

## Overview
Standardized error handling system with proper type safety, user-friendly messages, and comprehensive logging.

## Usage

### In Services
```typescript
import { createApiError, createServiceError } from '../utils/errorHandler';

try {
  const response = await api.get('/endpoint');
  return response.data;
} catch (error) {
  if (axios.isAxiosError(error)) {
    throw createApiError(
      error.response?.data?.message || error.message,
      error.response?.status || 500,
      '/endpoint',
      'GET'
    );
  }
  throw createServiceError(
    'Operation failed',
    'serviceName',
    'operationName'
  );
}
```

### In Components/Hooks
```typescript
import { useErrorHandler } from '../hooks/useErrorHandler';

const { handleError } = useErrorHandler({ context: 'ComponentName' });

// In mutation
onError: (error) => {
  handleError(error, 'Custom user message');
}
```

### Error Types
- **ApiError**: HTTP/API related errors
- **ServiceError**: Service layer errors  
- **ValidationError**: Data validation errors
- **NetworkError**: Network connectivity issues
- **AuthError**: Authentication/authorization errors

### Features
- Automatic error classification
- Sanitized error messages (XSS protection)
- User-friendly error messages
- Proper logging with context
- Toast notifications
- Global error handling for unhandled errors