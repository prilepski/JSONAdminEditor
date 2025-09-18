import React from 'react';

interface SkeletonProps {
  loading: boolean;
  rows?: number;
  height?: string;
  children: React.ReactNode;
}

export const Skeleton: React.FC<SkeletonProps> = ({ 
  loading, 
  rows = 3, 
  height = '20px', 
  children 
}) => {
  if (!loading) return <>{children}</>;

  return (
    <div data-testid="skeleton">
      {Array.from({ length: rows }).map((_, index) => (
        <div
          key={index}
          className="placeholder-glow mb-2"
          style={{ height }}
        >
          <div className="placeholder col-12"></div>
        </div>
      ))}
    </div>
  );
};