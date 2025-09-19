import React, { useRef } from 'react';
import { FileType } from '../types';
import { useUploadMutation } from '../hooks/useDictionaryQuery';
import { useConfirm } from '../hooks/useConfirm';
import { ConfirmModal } from './common';

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
  const fileInputRef = useRef<HTMLInputElement>(null);
  const uploadMutation = useUploadMutation();
  const { confirm, isOpen, options, handleConfirm, handleCancel } = useConfirm();

  const handleFileSelect = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file && selectedFileType !== FileType.None) {
      const confirmed = await confirm({
        title: 'Replace Dictionary',
        message: `Warning: The ${dictionaryNames[selectedFileType]} dictionary will be completely replaced with the content of the selected file. This action cannot be undone.`,
        confirmText: 'Replace Dictionary',
        cancelText: 'Cancel',
      });

      if (confirmed) {
        await handleUpload(file);
      } else {
        resetFileInput();
      }
    }
  };

  const handleUpload = async (file: File) => {
    try {
      const response = await uploadMutation.mutateAsync({
        fileType: selectedFileType,
        jsonFile: file,
      });

      if (response.success) {
        onUploadSuccess(response.message || 'Dictionary uploaded successfully!');
      } else {
        onUploadError(response.error || 'Upload failed');
      }
    } catch {
      onUploadError('Error uploading file');
    } finally {
      resetFileInput();
    }
  };

  const resetFileInput = () => {
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  if (selectedFileType === FileType.None) {
    return null;
  }

  return (
    <>
      <div className="input-group">
        <label className="input-group-text">
          <i className="fas fa-upload me-1"></i>Replace Dictionary
        </label>
        <input
          type="file"
          className="form-control"
          accept=".json"
          ref={fileInputRef}
          onChange={handleFileSelect}
          disabled={uploadMutation.isPending}
        />
      </div>

      <ConfirmModal
        isOpen={isOpen}
        title={options.title}
        message={options.message}
        onConfirm={handleConfirm}
        onCancel={handleCancel}
        confirmText={options.confirmText}
        cancelText={options.cancelText}
      />
    </>
  );
};
