import { useOktaAuth } from '@okta/okta-react';
import { isAuthEnabled } from './config';

export const useAuth = () => {
  if (!isAuthEnabled()) {
    return {
      isAuthenticated: true,
      user: null,
      login: () => {},
      logout: () => {},
      authState: { isAuthenticated: true, isPending: false },
    };
  }

  const { oktaAuth, authState } = useOktaAuth();

  return {
    isAuthenticated: authState?.isAuthenticated ?? false,
    user: authState?.idToken?.claims ?? null,
    login: () => oktaAuth.signInWithRedirect(),
    logout: () => oktaAuth.signOut(),
    authState,
  };
};