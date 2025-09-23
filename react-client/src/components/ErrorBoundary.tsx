import React, { Component, ReactNode } from 'react';
import { handleError, getUserFriendlyMessage } from '../utils/errorHandler';
import { ErrorSeverity } from '../types/errors';

interface Props {
  children: ReactNode;
  fallback?: ReactNode;
  onError?: (error: Error, errorInfo: React.ErrorInfo) => void;
}

interface State {
  hasError: boolean;
  errorId?: string;
  userMessage?: string;
}

export class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error: Error): State {
    const errorResponse = handleError(error, 'ErrorBoundary');
    const errorId = `ERR-${Date.now()}`;
    
    return { 
      hasError: true, 
      errorId,
      userMessage: getUserFriendlyMessage(errorResponse)
    };
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    const errorResponse = handleError(error, 'ErrorBoundary');
    
    // Log error with context
    console.error('Error caught by boundary:', {
      error: errorResponse,
      errorInfo,
      componentStack: errorInfo.componentStack
    });

    // Call custom error handler if provided
    this.props.onError?.(error, errorInfo);
  }

  private handleRetry = () => {
    this.setState({ hasError: false, errorId: undefined, userMessage: undefined });
  };

  render() {
    if (this.state.hasError) {
      return (
        this.props.fallback || (
          <div className="container mt-5">
            <div className="alert alert-danger">
              <h4><i className="bi bi-exclamation-triangle"></i> Something went wrong</h4>
              <p>{this.state.userMessage}</p>
              {this.state.errorId && (
                <small className="text-muted d-block mb-3">
                  Error ID: {this.state.errorId}
                </small>
              )}
              <div className="d-flex gap-2">
                <button 
                  className="btn btn-primary" 
                  onClick={this.handleRetry}
                >
                  Try Again
                </button>
                <button 
                  className="btn btn-outline-secondary" 
                  onClick={() => window.location.reload()}
                >
                  Refresh Page
                </button>
              </div>
            </div>
          </div>
        )
      );
    }

    return this.props.children;
  }
}
