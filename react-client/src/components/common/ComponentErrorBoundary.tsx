import React from 'react';
import { ErrorBoundary } from 'react-error-boundary';

interface ComponentErrorBoundaryProps {
  children: React.ReactNode;
  fallback?: React.ComponentType<any>;
  componentName?: string;
}

export const ComponentErrorBoundary: React.FC<ComponentErrorBoundaryProps> = ({ 
  children, 
  fallback: Fallback,
  componentName = 'Component'
}) => (
  <ErrorBoundary
    fallback={Fallback ? <Fallback /> : (
      <div className="alert alert-warning">
        <i className="fas fa-exclamation-circle me-2"></i>
        {componentName} failed to load
      </div>
    )}
    onError={(error) => {
      console.error(`Error in ${componentName}:`, error);
    }}
  >
    {children}
  </ErrorBoundary>
);