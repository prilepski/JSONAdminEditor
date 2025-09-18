// Generated API types - will be replaced by OpenAPI generation
export interface DictionaryData {
  filePath: string;
  fileName: string;
  columnNames: string[];
  columnTypes: Record<string, string>;
  tableData: Record<string, any>[];
  isValidJson: boolean;
}

export interface ApiResponse<T = any> {
  success: boolean;
  message?: string;
  error?: string;
  data?: T;
  validationErrors?: ValidationError[];
}

export interface ValidationError {
  rowIndex?: number;
  fieldName: string;
  message: string;
}

export interface FileUploadViewModel {
  fileType: number;
  customerName?: string;
  jsonFile?: File;
}

export interface EventData {
  Event?: string;
  OrderType?: string;
  Phone?: string;
  Email?: string;
  Logo?: string;
  IsSuppressed?: boolean;
  Templates?: {
    Email?: string;
    Sms?: string;
    Voice?: string;
  };
  ContentVariables?: Record<string, string>;
}

export interface Template {
  templateId: string;
  templateName: string;
}