import React from 'react';

interface PageHeaderProps {
  icon: string;
  title: string;
  description?: string;
}

export const PageHeader: React.FC<PageHeaderProps> = ({ icon, title, description }) => (
  <>
    <h1 className="mb-4">
      <i className={`fas ${icon} me-2`}></i>{title}
    </h1>
    {description && <p className="text-muted mb-4">{description}</p>}
  </>
);