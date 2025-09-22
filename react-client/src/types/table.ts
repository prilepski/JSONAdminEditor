// UI Table types
export type TableData = Record<string, any>;

export interface DictionaryData {
  filePath: string;
  fileName: string;
  columnNames: string[];
  columnTypes: Record<string, string>;
  tableData: TableData[];
  isValidJson: boolean;
}

// Content Variables (domain-specific)
export type ContentVariables = Record<string, string>;