export const config = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL || '/api',
  appEnv: import.meta.env.VITE_APP_ENV || 'development',
  appName: import.meta.env.VITE_APP_NAME || 'JSON Admin Editor',
  enableDevtools: import.meta.env.VITE_ENABLE_DEVTOOLS === 'true',
  logLevel: import.meta.env.VITE_LOG_LEVEL || 'info',
  isDevelopment: import.meta.env.DEV,
  isProduction: import.meta.env.PROD,
  isTest: import.meta.env.VITE_APP_ENV === 'test',
} as const;
