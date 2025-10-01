import React from 'react';
import { FileType, ValidationError } from '../../types';
import { FileUpload } from '../FileUpload';
import { JsonEditor } from '../JsonEditor';
import { TableSkeleton } from '../common';
import { ComponentErrorBoundary } from '../common';

interface DictionaryEditorProps {
  selectedFileType: FileType;
  dictionaryData: any;
  validationErrors: ValidationError[];
  isLoading: boolean;
  onSave: (tableData: any[]) => Promise<any>;
  onClearValidationErrors: () => void;
}

export const DictionaryEditor: React.FC<DictionaryEditorProps> = ({
  selectedFileType,
  dictionaryData,
  validationErrors,
  isLoading,
  onSave,
  onClearValidationErrors,
}) => {
  return (
    <div className="card">
      <div className="card-header">
        <div className="row align-items-center">
          <div className="col-md-6">
            <h3>
              <i className="fas fa-edit me-2"></i>
              Dictionary Editor
            </h3>
          </div>
          <div className="col-md-6">
            <ComponentErrorBoundary componentName="File Upload">
              <FileUpload
                selectedFileType={selectedFileType}
                onUploadSuccess={() => {}}
                onUploadError={() => {}}
              />
            </ComponentErrorBoundary>
          </div>
        </div>
      </div>
      <div className="card-body">
        {isLoading ? (
          <TableSkeleton rows={5} columns={4} />
        ) : (
          <ComponentErrorBoundary componentName="JSON Editor">
            <JsonEditor
              dictionaryData={dictionaryData}
              selectedFileType={selectedFileType}
              validationErrors={validationErrors}
              onSave={onSave}
              onClearValidationErrors={onClearValidationErrors}
            />
          </ComponentErrorBoundary>
        )}
      </div>
    </div>
  );
};