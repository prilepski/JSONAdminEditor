import { handleError } from './errorHandler';

// Global error handler setup
export const setupGlobalErrorHandling = () => {
  // Handle unhandled promise rejections
  window.addEventListener('unhandledrejection', (event) => {
    const errorResponse = handleError(event.reason, 'UnhandledPromiseRejection');
    console.error('Unhandled promise rejection:', errorResponse);
    
    // Prevent the default browser behavior
    event.preventDefault();
  });

  // Handle uncaught errors
  window.addEventListener('error', (event) => {
    const errorResponse = handleError(event.error || event.message, 'UncaughtError');
    console.error('Uncaught error:', errorResponse);
  });

  // Handle React errors in development
  if (import.meta.env.DEV) {
    const originalConsoleError = console.error;
    console.error = (...args) => {
      // Check if this is a React error
      if (args[0]?.includes?.('React') || args[0]?.includes?.('Warning')) {
        originalConsoleError.apply(console, args);
        return;
      }
      
      // Handle other errors
      const errorResponse = handleError(args[0], 'ConsoleError');
      originalConsoleError('Processed error:', errorResponse);
    };
  }
};

// Cleanup function
export const cleanupGlobalErrorHandling = () => {
  window.removeEventListener('unhandledrejection', () => {});
  window.removeEventListener('error', () => {});
};