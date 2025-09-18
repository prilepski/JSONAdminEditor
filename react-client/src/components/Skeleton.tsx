import React from 'react';

interface SkeletonProps {
  loading: boolean;
  children: React.ReactNode;
  rows?: number;
  height?: string;
}

export const Skeleton: React.FC<SkeletonProps> = ({ 
  loading, 
  children, 
  rows = 3, 
  height = '20px' 
}) => {
  if (!loading) return <>{children}</>;

  return (
    <div className="skeleton-container">
      {Array.from({ length: rows }).map((_, i) => (
        <div 
          key={i}
          className="loading-skeleton mb-2 rounded"
          style={{ height }}
        />
      ))}
    </div>
  );
};