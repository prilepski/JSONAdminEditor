import React from 'react';

interface Tab {
  id: string;
  label: string;
  icon: string;
}

interface TabNavigationProps {
  tabs: Tab[];
  activeTab: string;
  onTabChange: (tabId: string) => void;
}

export const TabNavigation: React.FC<TabNavigationProps> = ({ tabs, activeTab, onTabChange }) => (
  <ul className="nav nav-tabs mb-4">
    {tabs.map((tab) => (
      <li key={tab.id} className="nav-item">
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === tab.id}
          className={`nav-link ${activeTab === tab.id ? 'active' : ''}`}
          onClick={() => onTabChange(tab.id)}
        >
          <i className={`fas ${tab.icon} me-2`}></i>
          {tab.label}
        </button>
      </li>
    ))}
  </ul>
);
