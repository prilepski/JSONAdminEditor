import React from 'react';
import { TableData, ValidationError } from '../../types';

interface DataTableProps {
  data: TableData[];
  columns: string[];
  columnTypes: Record<string, string>;
  validationErrors: ValidationError[];
  onUpdateCell: (rowIndex: number, column: string, value: any) => void;
  onDeleteRow: (index: number) => void;
  channelOptions?: string[];
  selectedFileType?: number;
}

export const DataTable: React.FC<DataTableProps> = ({
  data,
  columns,
  columnTypes,
  validationErrors,
  onUpdateCell,
  onDeleteRow,
  channelOptions = [],
  selectedFileType,
}) => {
  const getValidationError = (rowIndex: number, fieldName: string): ValidationError | undefined => {
    return validationErrors.find((e) => e.rowIndex === rowIndex && e.fieldName === fieldName);
  };

  const renderCell = (row: TableData, column: string, rowIndex: number) => {
    const columnType = columnTypes[column] || 'text';
    const cellValue = row[column] || '';
    const validationError = getValidationError(rowIndex, column);
    const inputClass = validationError
      ? 'form-control form-control-sm is-invalid'
      : 'form-control form-control-sm';

    if (columnType === 'boolean') {
      return (
        <div className="form-check">
          <input
            type="checkbox"
            className="form-check-input"
            checked={cellValue === true || cellValue === 'true'}
            onChange={(e) => onUpdateCell(rowIndex, column, e.target.checked)}
          />
        </div>
      );
    }

    if (column.toLowerCase() === 'channeltype' && selectedFileType === 1) {
      return (
        <select
          className={inputClass}
          value={cellValue.toString()}
          onChange={(e) => onUpdateCell(rowIndex, column, e.target.value)}
          required
        >
          <option value="">Select Channel...</option>
          {channelOptions.map((channel) => (
            <option key={channel} value={channel}>
              {channel}
            </option>
          ))}
        </select>
      );
    }

    return (
      <input
        type="text"
        className={inputClass}
        value={cellValue.toString()}
        onChange={(e) => onUpdateCell(rowIndex, column, e.target.value)}
      />
    );
  };

  return (
    <div className="table-responsive">
      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            {columns.map((column) => (
              <th key={column}>{column}</th>
            ))}
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {data.map((row, rowIndex) => (
            <tr key={rowIndex}>
              {columns.map((column) => {
                const validationError = getValidationError(rowIndex, column);
                return (
                  <td key={column}>
                    {renderCell(row, column, rowIndex)}
                    {validationError && (
                      <div className="invalid-feedback d-block">
                        <small>{validationError.message}</small>
                      </div>
                    )}
                  </td>
                );
              })}
              <td>
                <button
                  type="button"
                  className="btn btn-danger btn-sm"
                  onClick={() => onDeleteRow(rowIndex)}
                >
                  <i className="fas fa-trash"></i>
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
