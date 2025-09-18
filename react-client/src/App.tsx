
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

import './App.css';

const queryClient = new QueryClient();

function App() {
  return (
    <PageErrorBoundary pageName="Application">
      <QueryClientProvider client={queryClient}>
        <NiceModal.Provider>
          <Router>
            <div className="App">
              <header>
                <nav className="navbar navbar-dark bg-primary border-bottom box-shadow mb-3">
                  <div className="container-fluid">
                    <Link className="navbar-brand" to="/">
                      <i className="fas fa-table me-2"></i>JSON Admin Editor
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
                        <Route path="/preferred-communication" element={<PreferredCommunication />} />
                      </Routes>
                    </div>
                  </main>
                </div>
              </div>
            </div>
          </Router>
          <Toaster position="top-right" />
        </NiceModal.Provider>
        <ReactQueryDevtools initialIsOpen={false} />
      </QueryClientProvider>
    </PageErrorBoundary>
  );
}

export default App;