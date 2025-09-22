import { ValidationError } from './api';
import { TableData } from './table';

// Dictionary UI types
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
