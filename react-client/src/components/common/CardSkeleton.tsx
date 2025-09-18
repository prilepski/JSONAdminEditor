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
        {Array.from({ length: lines }).map((_, i) => (
          <div key={i} className="mb-2">
            <span className={`placeholder col-${Math.floor(Math.random() * 4) + 8}`}></span>
          </div>
        ))}
      </div>
    </div>
  </div>
);