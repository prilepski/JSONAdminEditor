export interface ValidationError {
  field: string;
  message: string;
  code?: string;
}

export interface ApiError {
  message: string;
  status?: number;
  code?: string;
}

export interface ServiceError extends Error {
  status?: number;
  code?: string;
}