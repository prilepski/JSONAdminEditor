import React from 'react';

interface CardSkeletonProps {
  hasHeader?: boolean;
  lines?: number;
}

export const CardSkeleton: React.FC<CardSkeletonProps> = ({ 
  hasHeader = true, 
  lines = 3 
}) => (
  <div className="card">
    {hasHeader && (
      <div className="card-header">
        <div className="placeholder-glow">
          <span className="placeholder col-6"></span>
        </div>
      </div>
    )}
    <div className="card-body">
      <div className="placeholder-glow">
        {Array.from({ length: lines }, (_, i) => {
          const widths = [8, 9, 10, 11];
          const width = widths[i % widths.length];
          return (
            <div key={`line-${i}`} className="mb-2">
              <span className={`placeholder col-${width}`}></span>
            </div>
          );
        })}
      </div>
    </div>
  </div>
);