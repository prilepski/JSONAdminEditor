import React from 'react';
import { LoginCallback as OktaLoginCallback } from '@okta/okta-react';
import { isAuthEnabled } from './config';

const LoginCallback: React.FC = () => {
  if (!isAuthEnabled()) {
    // Redirect to home if auth is disabled
    window.location.replace('/');
    return null;
  }

  return <OktaLoginCallback />;
};

export default LoginCallback;