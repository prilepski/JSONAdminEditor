import React from 'react';
import { useOktaAuth } from '@okta/okta-react';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {

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