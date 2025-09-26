import { useMemo, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, Link } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { Toaster } from 'react-hot-toast';
import NiceModal from '@ebay/nice-modal-react';
import { PageErrorBoundary } from './components/common';
import { Sidebar } from './components/Sidebar';
import { Dictionaries } from './pages/Dictionaries';
import { Events } from './pages/Events';
import { ContentVariables } from './pages/ContentVariables';
import { CustomerEvents } from './pages/CustomerEvents';
import { CustomerSettings } from './pages/CustomerSettings';
import { PreferredCommunication } from './pages/PreferredCommunication';
import { AfterHours } from './pages/AfterHours';
import { CustomerAfterHours } from './pages/CustomerAfterHours';
import { CustomerPreferredCommunication } from './pages/CustomerPreferredCommunication';
import { FromEmail } from './pages/FromEmail';
import { CustomerFromEmail } from './pages/CustomerFromEmail';
import { OptOut } from './pages/OptOut';
import { AuthProvider, ProtectedRoute, LoginCallback } from './auth';
import { setupGlobalErrorHandling, cleanupGlobalErrorHandling } from './utils/globalErrorHandler';

import './App.css';

function App() {
  const queryClient = useMemo(() => new QueryClient({
    defaultOptions: {
      queries: {
        retry: (failureCount, error) => {
          // Don't retry on auth errors
          if ((error as any)?.status === 401 || (error as any)?.status === 403) {
            return false;
          }
          // Retry up to 2 times for other errors
          return failureCount < 2;
        },
        staleTime: 5 * 60 * 1000, // 5 minutes
      },
      mutations: {
        retry: false, // Don't retry mutations
      },
    },
  }), []);

  useEffect(() => {
    setupGlobalErrorHandling();
    return cleanupGlobalErrorHandling;
  }, []);

  return (
    <PageErrorBoundary pageName="Application">
      <AuthProvider>
        <QueryClientProvider client={queryClient}>
          <NiceModal.Provider>
            <Router>
              <div className="App">
                <Routes>
                  <Route path="/login/callback" element={<LoginCallback />} />
                  <Route
                    path="/*"
                    element={
                      <ProtectedRoute>
                        <header>
                          <nav className="navbar navbar-dark bg-primary border-bottom box-shadow mb-3">
                            <div className="container-fluid">
                              <Link className="navbar-brand" to="/">
                                <i className="fas fa-table me-2"></i>
                                {import.meta.env.VITE_APP_NAME || 'JSON Admin Editor'}
                              </Link>
                            </div>
                          </nav>
                        </header>

                        <div className="container-fluid">
                          <div className="row">
                            <Sidebar />

                            <main className="col-md-10 ms-sm-auto col-lg-10 px-md-4">
                              <div className="pt-3 pb-3">
                                <Routes>
                                  <Route path="/" element={<Navigate to="/dictionaries" replace />} />
                                  <Route path="/dictionaries" element={<Dictionaries />} />
                                  <Route path="/events" element={<Events />} />
                                  <Route path="/content-variables" element={<ContentVariables />} />
                                  <Route path="/customer-events" element={<CustomerEvents />} />
                                  <Route path="/customer-settings" element={<CustomerSettings />} />
                                  <Route
                                    path="/preferred-communication"
                                    element={<PreferredCommunication />}
                                  />
                                  <Route path="/after-hours" element={<AfterHours />} />
                                  <Route path="/from-email" element={<FromEmail />} />
                                  <Route path="/customer-after-hours" element={<CustomerAfterHours />} />
                                  <Route path="/customer-preferred-communication" element={<CustomerPreferredCommunication />} />
                                  <Route path="/customer-from-email" element={<CustomerFromEmail />} />
                                  <Route path="/opt-out" element={<OptOut />} />
                                </Routes>
                              </div>
                            </main>
                          </div>
                        </div>
                      </ProtectedRoute>
                    }
                  />
                </Routes>
              </div>
            </Router>
            <Toaster position="top-right" />
          </NiceModal.Provider>
          {import.meta.env.VITE_ENABLE_DEVTOOLS === 'true' && (
            <ReactQueryDevtools initialIsOpen={false} />
          )}
        </QueryClientProvider>
      </AuthProvider>
    </PageErrorBoundary>
  );
}

export default App;
