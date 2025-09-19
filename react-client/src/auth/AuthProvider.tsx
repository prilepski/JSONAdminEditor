import React from 'react';
import { Security } from '@okta/okta-react';
import { OktaAuth } from '@okta/okta-auth-js';
import { getAuthConfig, isAuthEnabled } from './config';

interface AuthProviderProps {
  children: React.ReactNode;
}

const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  if (!isAuthEnabled()) {
    // Return children directly when auth is disabled (dev environment)
    return <>{children}</>;
  }

  const authConfig = getAuthConfig()!;
  const oktaAuth = new OktaAuth(authConfig);

  return (
    <Security oktaAuth={oktaAuth} restoreOriginalUri={() => window.location.replace('/')}>
      {children}
    </Security>
  );
};

export default AuthProvider;