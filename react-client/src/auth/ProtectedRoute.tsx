import React from 'react';
import { useOktaAuth } from '@okta/okta-react';
import { isAuthEnabled } from './config';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  if (!isAuthEnabled()) {
    // Return children directly when auth is disabled
    return <>{children}</>;
  }

  const { oktaAuth, authState } = useOktaAuth();

  if (!authState) {
    return <div>Loading...</div>;
  }

  if (!authState.isAuthenticated) {
    oktaAuth.signInWithRedirect();
    return <div>Redirecting to login...</div>;
  }

  return <>{children}</>;
};

export default ProtectedRoute;