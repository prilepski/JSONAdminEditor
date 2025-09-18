import React, { useState } from 'react';
import { FileType } from '../types';
import { useUploadMutation } from '../hooks/useDictionaryQuery';

interface FileUploadProps {
  selectedFileType: FileType;
  onUploadSuccess: (message: string) => void;
  onUploadError: (error: string) => void;
}

const dictionaryNames: Record<FileType, string> = {
  [FileType.None]: '',
  [FileType.Templates]: 'Notification Templates',
  [FileType.EventTriggers]: 'Event Triggers',
  [FileType.EventChannels]: 'Event Channels',
  [FileType.OrderTypes]: 'Order Types',
  [FileType.Customers]: 'Customers',
};

export const FileUpload: React.FC<FileUploadProps> = ({
  selectedFileType,
  onUploadSuccess,
  onUploadError,
}) => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [showConfirmation, setShowConfirmation] = useState(false);
  const uploadMutation = useUploadMutation();

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file && selectedFileType !== FileType.None) {
      setSelectedFile(file);
      setShowConfirmation(true);
    }
  };

  const handleConfirmUpload = async () => {
    if (!selectedFile) return;

    setShowConfirmation(false);

    try {
      const response = await uploadMutation.mutateAsync({
        fileType: selectedFileType,
        jsonFile: selectedFile,
      });

      if (response.success) {
        onUploadSuccess(response.message || 'Dictionary uploaded successfully!');
      } else {
        onUploadError(response.error || 'Upload failed');
      }
    } catch (error) {
      onUploadError('Error uploading file');
    } finally {
      setSelectedFile(null);
      // Reset file input
      const fileInput = document.getElementById('fileInput') as HTMLInputElement;
      if (fileInput) fileInput.value = '';
    }
  };

  const handleCancelUpload = () => {
    setShowConfirmation(false);
    setSelectedFile(null);
    // Reset file input
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) fileInput.value = '';
  };

  if (selectedFileType === FileType.None) {
    return null;
  }

  return (
    <>
      <div className="input-group">
        <label className="input-group-text" htmlFor="fileInput">
          <i className="fas fa-upload me-1"></i>Replace Dictionary
        </label>
        <input
          type="file"
          className="form-control"
          accept=".json"
          id="fileInput"
          onChange={handleFileSelect}
          disabled={uploadMutation.isPending}
        />
      </div>

      {/* Confirmation Modal */}
      {showConfirmation && (
        <div
          className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
          style={{ backgroundColor: 'rgba(0,0,0,0.5)', zIndex: 1050 }}
          onClick={(e) => e.target === e.currentTarget && handleCancelUpload()}
        >
          <div className="card" style={{ maxWidth: '500px', width: '90%' }}>
            <div className="card-header bg-warning text-dark">
              <h5 className="mb-0">
                <i className="fas fa-exclamation-triangle me-2"></i>Replace Dictionary
              </h5>
            </div>
            <div className="card-body">
              <p className="mb-3">
                <strong>Warning:</strong> The{' '}
                <span className="text-primary">{dictionaryNames[selectedFileType]}</span> dictionary
                will be completely replaced with the content of the selected file.
              </p>
              <p className="mb-3 text-muted">
                This action cannot be undone. All existing data in this dictionary will be lost.
              </p>
              <div className="d-flex justify-content-end gap-2">
                <button
                  type="button"
                  className="btn btn-secondary"
                  onClick={handleCancelUpload}
                  disabled={uploadMutation.isPending}
                >
                  Cancel
                </button>
                <button
                  type="button"
                  className="btn btn-warning"
                  onClick={handleConfirmUpload}
                  disabled={uploadMutation.isPending}
                >
                  {uploadMutation.isPending ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-1" role="status"></span>
                      Uploading...
                    </>
                  ) : (
                    <>
                      <i className="fas fa-upload me-1"></i>Replace Dictionary
                    </>
                  )}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </>
  );
};
