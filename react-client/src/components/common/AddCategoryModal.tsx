import React from 'react';

interface AddCategoryModalProps {
  show: boolean;
  title: string;
  placeholder: string;
  value: string;
  onValueChange: (value: string) => void;
  onConfirm: () => void;
  onCancel: () => void;
  isLoading?: boolean;
}

export const AddCategoryModal: React.FC<AddCategoryModalProps> = ({
  show,
  title,
  placeholder,
  value,
  onValueChange,
  onConfirm,
  onCancel,
  isLoading = false,
}) => {
  if (!show) return null;

  return (
    <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">{title}</h5>
            <button className="btn-close" onClick={onCancel}></button>
          </div>
          <div className="modal-body">
            <input
              type="text"
              className="form-control"
              placeholder={placeholder}
              value={value}
              onChange={(e) => onValueChange(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && onConfirm()}
            />
          </div>
          <div className="modal-footer">
            <button className="btn btn-secondary" onClick={onCancel}>Cancel</button>
            <button 
              className="btn btn-primary" 
              onClick={onConfirm} 
              disabled={!value.trim() || isLoading}
            >
              {isLoading ? 'Adding...' : 'Add'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};