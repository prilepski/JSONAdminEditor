import React from 'react';

interface NestedVariablesNavigatorProps {
  data: Record<string, Record<string, Record<string, string>>>;
  selectedPath: string;
  expandedCategories: Set<string>;
  onPathSelect: (path: string) => void;
  onToggleCategory: (category: string) => void;
  onAddCategory: () => void;
  onAddSubcategory: (category: string) => void;
}

export const NestedVariablesNavigator: React.FC<NestedVariablesNavigatorProps> = ({
  data,
  selectedPath,
  expandedCategories,
  onPathSelect,
  onToggleCategory,
  onAddCategory,
  onAddSubcategory,
}) => {
  return (
    <div className="card">
      <div className="card-header d-flex justify-content-between align-items-center">
        <h6 className="mb-0">Categories</h6>
        <button 
          className="btn btn-primary btn-sm" 
          onClick={onAddCategory}
          style={{ width: '32px', height: '32px' }}
        >
          <i className="fas fa-plus"></i>
        </button>
      </div>
      <div className="card-body p-0">
        <div className="list-group list-group-flush">
          {Object.keys(data).map((category) => (
            <div key={category}>
              <div className="list-group-item d-flex justify-content-between align-items-center py-2 px-3">
                <button
                  className="btn btn-link text-start p-0 text-decoration-none flex-grow-1"
                  onClick={() => onToggleCategory(category)}
                >
                  <i className={`fas fa-chevron-${expandedCategories.has(category) ? 'down' : 'right'} me-2`}></i>
                  {category}
                </button>
                <button
                  className="btn btn-success btn-sm"
                  onClick={() => onAddSubcategory(category)}
                  style={{ width: '32px', height: '32px' }}
                >
                  <i className="fas fa-plus"></i>
                </button>
              </div>
              {expandedCategories.has(category) && Object.keys(data[category] || {}).map((subcategory) => (
                <div
                  key={`${category}/${subcategory}`}
                  className={`list-group-item d-flex justify-content-between align-items-center py-2 ps-5 pe-3 ${selectedPath === `${category}/${subcategory}` ? 'active' : ''}`}
                >
                  <button
                    className="btn btn-link text-start p-0 text-decoration-none flex-grow-1"
                    onClick={() => onPathSelect(`${category}/${subcategory}`)}
                  >
                    <i className="fas fa-file me-2"></i>{subcategory}
                  </button>
                  <div style={{ width: '32px' }}></div>
                </div>
              ))}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};