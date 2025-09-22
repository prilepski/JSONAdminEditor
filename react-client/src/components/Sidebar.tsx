import React, { useState } from 'react';
import { useLocation, Link } from 'react-router-dom';

export const Sidebar: React.FC = () => {
  const [globalSettingsOpen, setGlobalSettingsOpen] = useState(true);
  const [customerOverridesOpen, setCustomerOverridesOpen] = useState(true);
  const location = useLocation();

  const isActive = (path: string) => location.pathname === path;
  const isParentActive = (paths: string[]) => paths.some((path) => location.pathname === path);

  return (
    <nav className="col-md-2 col-lg-2 d-md-block bg-light sidebar">
      <div className="position-sticky pt-3">
        <ul className="nav flex-column">
          <li className="nav-item">
            <span
              className={`nav-link nav-header ${isParentActive(['/preferred-communication', '/content-variables', '/events', '/after-hours']) ? 'parent-active' : ''}`}
              onClick={() => setGlobalSettingsOpen(!globalSettingsOpen)}
            >
              <i className="fas fa-globe me-2"></i>Global Settings
              <i className={`fas fa-chevron-${globalSettingsOpen ? 'down' : 'right'} ms-auto`}></i>
            </span>
            <ul className={`nav flex-column submenu ${globalSettingsOpen ? 'show' : ''}`}>
              <li className="nav-item">
                <Link
                  className={`nav-link submenu-link ${isActive('/preferred-communication') ? 'active' : ''}`}
                  to="/preferred-communication"
                >
                  <i className="fas fa-comments me-2"></i>Preferred Communication
                </Link>
              </li>
              <li className="nav-item">
                <Link
                  className={`nav-link submenu-link ${isActive('/content-variables') ? 'active' : ''}`}
                  to="/content-variables"
                >
                  <i className="fas fa-tags me-2"></i>Content Variables
                </Link>
              </li>
              <li className="nav-item">
                <Link
                  className={`nav-link submenu-link ${isActive('/events') ? 'active' : ''}`}
                  to="/events"
                >
                  <i className="fas fa-calendar-alt me-2"></i>Events
                </Link>
              </li>
              <li className="nav-item">
                <Link
                  className={`nav-link submenu-link ${isActive('/after-hours') ? 'active' : ''}`}
                  to="/after-hours"
                >
                  <i className="fas fa-clock me-2"></i>After Hours
                </Link>
              </li>
            </ul>
          </li>
          <li className="nav-item">
            <span
              className={`nav-link nav-header ${isParentActive(['/customer-settings', '/customer-events']) ? 'parent-active' : ''}`}
              onClick={() => setCustomerOverridesOpen(!customerOverridesOpen)}
            >
              <i className="fas fa-users me-2"></i>Customer Overrides
              <i
                className={`fas fa-chevron-${customerOverridesOpen ? 'down' : 'right'} ms-auto`}
              ></i>
            </span>
            <ul className={`nav flex-column submenu ${customerOverridesOpen ? 'show' : ''}`}>
              <li className="nav-item">
                <Link
                  className={`nav-link submenu-link ${isActive('/customer-settings') ? 'active' : ''}`}
                  to="/customer-settings"
                >
                  <i className="fas fa-code me-2"></i>Customer Content Variables
                </Link>
              </li>
              <li className="nav-item">
                <Link
                  className={`nav-link submenu-link ${isActive('/customer-events') ? 'active' : ''}`}
                  to="/customer-events"
                >
                  <i className="fas fa-calendar-alt me-2"></i>Customer Events
                </Link>
              </li>
            </ul>
          </li>
          <li className="nav-item">
            <Link
              className={`nav-link ${isActive('/dictionaries') ? 'active' : ''}`}
              to="/dictionaries"
            >
              <i className="fas fa-cog me-2"></i>Dictionaries
            </Link>
          </li>
        </ul>
      </div>
    </nav>
  );
};
