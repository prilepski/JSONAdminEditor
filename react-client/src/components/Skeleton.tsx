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
          className="skeleton-item mb-2"
          style={{ 
            height,
            backgroundColor: '#e9ecef',
            borderRadius: '4px',
            animation: 'skeleton-pulse 1.5s ease-in-out infinite'
          }}
        />
      ))}
      <style>{`
        @keyframes skeleton-pulse {
          0% { opacity: 1; }
          50% { opacity: 0.4; }
          100% { opacity: 1; }
        }
      `}</style>
    </div>
  );
};