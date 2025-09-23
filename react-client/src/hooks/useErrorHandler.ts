import { useCallback } from 'react';
import { toast } from 'react-hot-toast';
import { handleError, getUserFriendlyMessage } from '../utils/errorHandler';
import { ErrorResponse, ErrorSeverity } from '../types/errors';

interface UseErrorHandlerOptions {
  showToast?: boolean;
  logErrors?: boolean;
  context?: string;
}

export const useErrorHandler = (options: UseErrorHandlerOptions = {}) => {
  const { showToast = true, logErrors = true, context } = options;

  const handleErrorWithToast = useCallback((error: unknown, customMessage?: string) => {
    const errorResponse = handleError(error, context);
    
    if (logErrors) {
      console.error('Error handled:', errorResponse);
    }

    if (showToast) {
      const message = customMessage || getUserFriendlyMessage(errorResponse);
      
      switch (errorResponse.severity) {
        case ErrorSeverity.CRITICAL:
        case ErrorSeverity.HIGH:
          toast.error(message, { duration: 6000 });
          break;
        case ErrorSeverity.MEDIUM:
          toast.error(message, { duration: 4000 });
          break;
        default:
          toast(message, { duration: 3000 });
      }
    }

    return errorResponse;
  }, [showToast, logErrors, context]);

  const handleErrorSilently = useCallback((error: unknown): ErrorResponse => {
    const errorResponse = handleError(error, context);
    
    if (logErrors) {
      console.error('Error handled silently:', errorResponse);
    }

    return errorResponse;
  }, [logErrors, context]);

  return {
    handleError: handleErrorWithToast,
    handleErrorSilently
  };
};