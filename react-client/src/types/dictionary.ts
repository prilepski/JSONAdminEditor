import { TableData, ValidationError, FileType } from './common';

// Dictionary-specific types
export interface DictionaryData {
  filePath: string;
  fileName: string;
  columnNames: string[];
  columnTypes: Record<string, string>;
  tableData: TableData[];
  isValidJson: boolean;
}

export interface JsonFileViewModel {
  fileName?: string;
  filePath?: string;
  jsonContent?: string;
  tableData?: TableData[];
  columnNames?: string[];
  columnTypes?: Record<string, string>;
  errorMessage?: string;
  isValidJson: boolean;
  validationErrors?: ValidationError[];
}

export interface FileUploadRequest {
  fileType: FileType;
  customerName?: string;
  jsonFile?: File;
}