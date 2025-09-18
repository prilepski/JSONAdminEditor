import React from 'react';

interface ErrorFallbackProps {
  error: Error;
  resetError: () => void;
  componentName?: string;
}

export const ErrorFallback: React.FC<ErrorFallbackProps> = ({
  error,
  resetError,
  componentName = 'Component',
}) => (
  <div className="alert alert-danger" role="alert">
    <div className="d-flex align-items-center mb-3">
      <i className="fas fa-exclamation-triangle fa-2x text-danger me-3"></i>
      <div>
        <h4 className="alert-heading mb-1">Something went wrong</h4>
        <p className="mb-0">An error occurred in {componentName}</p>
      </div>
    </div>

    <details className="mb-3">
      <summary className="btn btn-outline-secondary btn-sm">Show error details</summary>
      <pre className="mt-2 p-2 bg-light border rounded small">
        {error.message}
        {error.stack && `\n\n${error.stack}`}
      </pre>
    </details>

    <button className="btn btn-primary" onClick={resetError}>
      <i className="fas fa-redo me-1"></i>Try again
    </button>
  </div>
);
