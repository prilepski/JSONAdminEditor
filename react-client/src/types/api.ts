import { components } from '../generated/api';

// Generated API types
export type Template = components['schemas']['Template'];
export type EventTrigger = components['schemas']['EventTrigger'];
export type EventChannel = components['schemas']['EventChannel'];
export type OrderType = components['schemas']['OrderType'];
export type Customer = components['schemas']['Customer'];
export type PreferredCommunication = components['schemas']['PreferredCommunication'];
export type Channel = components['schemas']['Channel'];
export type AfterHours2 = components['schemas']['AfterHours2'];
export type EventMapping = components['schemas']['EventMapping'];
export type CustomerNotificationMapping = components['schemas']['CustomerNotificationMapping'];
export type OptOutLocale = components['schemas']['OptOutLocale'];
export type OptOutConfig = { [key: string]: { [key: string]: OptOutLocale } };

// API Response types
export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  error?: string;
  message?: string;
  validationErrors?: ValidationError[];
}

export interface ValidationError {
  rowIndex: number;
  fieldName: string;
  message: string;
}

// File upload types
export interface FileUploadRequest {
  fileType: FileType;
  customerName?: string;
  jsonFile?: File;
}

// Enums
export enum FileType {
  None = 0,
  Templates = 1,
  Customers = 2,
  EventChannels = 3,
  EventTriggers = 5,
  OrderTypes = 7,
}