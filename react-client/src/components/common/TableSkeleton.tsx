import React from 'react';

interface TableSkeletonProps {
  rows?: number;
  columns?: number;
}

export const TableSkeleton: React.FC<TableSkeletonProps> = ({ 
  rows = 5, 
  columns = 4 
}) => (
  <div className="table-responsive">
    <table className="table">
      <thead>
        <tr>
          {Array.from({ length: columns }).map((_, i) => (
            <th key={`header-${i}`}>
              <div className="placeholder-glow">
                <span className="placeholder col-8"></span>
              </div>
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {Array.from({ length: rows }).map((_, rowIndex) => (
          <tr key={`row-${rowIndex}`}>
            {Array.from({ length: columns }).map((_, colIndex) => (
              <td key={`cell-${rowIndex}-${colIndex}`}>
                <div className="placeholder-glow">
                  <span className="placeholder col-10"></span>
                </div>
              </td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  </div>
);