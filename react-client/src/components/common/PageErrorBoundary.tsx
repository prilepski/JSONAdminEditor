import React from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { ErrorFallback } from './ErrorFallback';

interface PageErrorBoundaryProps {
  children: React.ReactNode;
  pageName: string;
}

export const PageErrorBoundary: React.FC<PageErrorBoundaryProps> = ({ 
  children, 
  pageName 
}) => (
  <ErrorBoundary
    FallbackComponent={({ error, resetErrorBoundary }) => (
      <ErrorFallback 
        error={error} 
        resetError={resetErrorBoundary} 
        componentName={`${pageName} page`} 
      />
    )}
    onError={(error, errorInfo) => {
      console.error(`Error in ${pageName} page:`, error, errorInfo);
    }}
  >
    {children}
  </ErrorBoundary>
);