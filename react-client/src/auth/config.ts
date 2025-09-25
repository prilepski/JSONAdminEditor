export interface AuthConfig {
  issuer: string;
  clientId: string;
  redirectUri: string;
  scopes: string[];
  pkce: boolean;
}

export const getAuthConfig = (): AuthConfig | null => {
  const env = import.meta.env.VITE_APP_ENV;
  
  // Disable auth for development environment
  if (env === 'local') {
    return null;
  }

  return {
    issuer: import.meta.env.VITE_OKTA_ISSUER || '',
    clientId: import.meta.env.VITE_OKTA_CLIENT_ID || '',
    redirectUri: import.meta.env.VITE_OKTA_REDIRECT_URI || `${window.location.origin}/login/callback`,
    scopes: ['openid', 'profile', 'email'],
    pkce: true,
  };
};

export const isAuthEnabled = (): boolean => {
  return getAuthConfig() !== null;
};