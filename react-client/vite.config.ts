import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig(({ mode }) => {
  // Load .env.local for dev mode, otherwise use process.env
  const env = mode === 'dev' ? loadEnv(mode, process.cwd(), 'VITE_') : process.env
  
  return {
    plugins: [react()],
    server: {
      port: parseInt(env.VITE_DEV_PORT as string) || 3000,
      proxy: env.VITE_PROXY_TARGET ? {
        '/api': env.VITE_PROXY_TARGET
      } : undefined
    },
    build: {
      outDir: 'dist',
      sourcemap: env.VITE_SOURCEMAP === 'true'
    },
    test: {
      globals: true,
      environment: 'jsdom',
      setupFiles: ['./src/setupTests.ts']
    }
  }
})