import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { fileUploadSchema, FileUploadInput } from '../schemas/validation';
import { FileType } from '../types';

interface FileUploadFormProps {
  selectedFileType: FileType;
  onSubmit: (data: FileUploadInput) => void;
  loading?: boolean;
}

export const FileUploadForm: React.FC<FileUploadFormProps> = ({
  selectedFileType,
  onSubmit,
  loading = false,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<FileUploadInput>({
    resolver: zodResolver(fileUploadSchema),
    defaultValues: {
      fileType: selectedFileType,
    },
  });

  const onFormSubmit = (data: FileUploadInput) => {
    onSubmit(data);
    reset();
  };

  return (
    <form onSubmit={handleSubmit(onFormSubmit)} className="d-flex align-items-center gap-2">
      <div className="flex-grow-1">
        <input
          {...register('jsonFile')}
          type="file"
          className={`form-control form-control-sm ${errors.jsonFile ? 'is-invalid' : ''}`}
          accept=".json"
          disabled={loading}
        />
        {errors.jsonFile && (
          <div className="invalid-feedback">{errors.jsonFile.message}</div>
        )}
      </div>
      
      <button
        type="submit"
        className="btn btn-outline-primary btn-sm"
        disabled={loading}
      >
        {loading ? (
          <>
            <span className="spinner-border spinner-border-sm me-1" />
            Uploading...
          </>
        ) : (
          <>
            <i className="fas fa-upload me-1" />
            Upload
          </>
        )}
      </button>
    </form>
  );
};