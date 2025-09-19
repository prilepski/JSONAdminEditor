import { ServiceError } from '../types/errors';

export const handleApiError = (error: unknown): ServiceError => {
  if (error instanceof Error) {
    return {
      ...error,
      message: error.message,
      status: (error as any).response?.status,
      code: (error as any).code
    };
  }
  
  return {
    name: 'UnknownError',
    message: 'An unknown error occurred',
    status: 500
  };
};

export const sanitizeErrorMessage = (message: string): string => {
  return message.replace(/[\r\n\t]/g, ' ').trim();
};