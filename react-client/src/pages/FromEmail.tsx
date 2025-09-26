import React, { useState, useEffect } from 'react';
import { SaveButton, LoadingSpinner } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { useFromEmailQuery, useFromEmailMutation } from '../hooks/useFromEmailQuery';
import { useErrorHandler } from '../hooks/useErrorHandler';

export const FromEmail: React.FC = () => {
  const [email, setEmail] = useState('');
  const { data, isLoading } = useFromEmailQuery();
  const mutation = useFromEmailMutation();
  const { handleError } = useErrorHandler({ context: 'FromEmail' });

  useEffect(() => {
    if (data !== undefined) {
      setEmail(data);
    }
  }, [data]);

  const handleSave = async () => {
    try {
      await mutation.mutateAsync(email);
    } catch (error) {
      handleError(error, 'Failed to save from email');
    }
  };

  return (
    <PageErrorBoundary pageName="From Email">
      <h1 className="mb-4">
        <i className="fas fa-envelope me-2"></i>From Email Management
      </h1>

      <div className="card">
        <div className="card-header d-flex justify-content-between align-items-center">
          <h3><i className="fas fa-edit me-2"></i>From Email Settings</h3>
          <SaveButton onClick={handleSave} loading={mutation.isPending} text="Save Changes" disabled={!email.trim()} />
        </div>
        <div className="card-body">
          {isLoading ? (
            <LoadingSpinner text="Loading from email settings..." />
          ) : (
            <ComponentErrorBoundary componentName="From Email Editor">
              <div className="mb-3">
                <label htmlFor="fromEmail" className="form-label">From Email Address</label>
                <input
                  type="email"
                  className="form-control"
                  id="fromEmail"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="Enter from email address"
                />
              </div>
            </ComponentErrorBoundary>
          )}
        </div>
      </div>
    </PageErrorBoundary>
  );
};