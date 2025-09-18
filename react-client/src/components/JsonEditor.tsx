import React, { useState, useEffect } from 'react';
import { FileType, DictionaryData, ValidationError, TableData } from '../types';
import { useChannelOptionsQuery } from '../hooks/useDictionaryQuery';

interface JsonEditorProps {
  dictionaryData?: DictionaryData | null;
  selectedFileType: FileType;
  validationErrors: ValidationError[];
  onSave: (tableData: TableData[]) => Promise<{ success: boolean; message?: string; error?: string }>;
  onClearValidationErrors: () => void;
}

export const JsonEditor: React.FC<JsonEditorProps> = ({
  dictionaryData,
  selectedFileType,
  validationErrors,
  onSave,
  onClearValidationErrors,
}) => {
  const [tableData, setTableData] = useState<TableData[]>([]);
  const { data: channelOptions = ['Email', 'Sms', 'Voice'] } = useChannelOptionsQuery();
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (dictionaryData?.tableData) {
      setTableData([...dictionaryData.tableData]);
    }
  }, [dictionaryData]);



  const getDictionaryName = (fileType: FileType): string => {
    const names: Record<FileType, string> = {
      [FileType.None]: '',
      [FileType.Templates]: 'Notification Templates',
      [FileType.EventTriggers]: 'Event Triggers',
      [FileType.EventChannels]: 'Event Channels',
      [FileType.OrderTypes]: 'Order Types',
      [FileType.Customers]: 'Customers',
    };
    return names[fileType] || 'Dictionary';
  };

  const addNewRow = () => {
    if (!dictionaryData?.columnNames) return;

    const newRow: TableData = {};
    dictionaryData.columnNames.forEach((column) => {
      const columnType = dictionaryData.columnTypes?.[column] || 'text';
      newRow[column] = columnType === 'boolean' ? false : '';
    });

    setTableData([...tableData, newRow]);
  };

  const deleteRow = (index: number) => {
    if (window.confirm('Are you sure you want to delete this row?')) {
      const newData = tableData.filter((_, i) => i !== index);
      setTableData(newData);
    }
  };

  const updateCell = (rowIndex: number, column: string, value: any) => {
    const newData = [...tableData];
    const columnType = dictionaryData?.columnTypes?.[column] || 'text';
    
    if (columnType === 'boolean') {
      newData[rowIndex][column] = value === 'true';
    } else {
      newData[rowIndex][column] = value;
    }
    
    setTableData(newData);
    onClearValidationErrors();
  };

  const handleSave = async () => {
    setSaving(true);
    await onSave(tableData);
    setSaving(false);
  };

  const getValidationError = (rowIndex: number, fieldName: string): ValidationError | undefined => {
    return validationErrors.find(e => e.rowIndex === rowIndex && e.fieldName === fieldName);
  };

  if (selectedFileType === FileType.None) {
    return null;
  }

  if (!dictionaryData) {
    return (
      <div className="text-center py-4">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
        <p className="mt-2 text-muted">Loading dictionary data...</p>
      </div>
    );
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4>Edit {getDictionaryName(selectedFileType)} Dictionary</h4>
        <div>
          <button
            type="button"
            className="btn btn-success me-2"
            onClick={addNewRow}
            disabled={saving}
          >
            <i className="fas fa-plus me-1"></i>Add Row
          </button>
          <button
            type="button"
            className="btn btn-primary"
            onClick={handleSave}
            disabled={saving}
          >
            {saving ? (
              <>
                <span className="spinner-border spinner-border-sm me-1" role="status"></span>
                Saving...
              </>
            ) : (
              <>
                <i className="fas fa-save me-1"></i>Save Changes
              </>
            )}
          </button>
        </div>
      </div>

      {tableData.length === 0 ? (
        <div className="text-center py-4">
          <i className="fas fa-table fa-3x text-muted mb-3"></i>
          <p className="text-muted">No data in this dictionary yet. Click "Add Row" to start adding entries.</p>
        </div>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped table-hover">
            <thead className="table-dark">
              <tr>
                {dictionaryData.columnNames?.map((column) => (
                  <th key={column}>{column}</th>
                ))}
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {tableData.map((row, rowIndex) => (
                <tr key={rowIndex}>
                  {dictionaryData.columnNames?.map((column) => {
                    const columnType = dictionaryData.columnTypes?.[column] || 'text';
                    const cellValue = row[column] || '';
                    const validationError = getValidationError(rowIndex, column);
                    const inputClass = validationError ? 'form-control form-control-sm is-invalid' : 'form-control form-control-sm';

                    return (
                      <td key={column}>
                        {columnType === 'boolean' ? (
                          <select
                            className={inputClass}
                            value={cellValue.toString()}
                            onChange={(e) => updateCell(rowIndex, column, e.target.value)}
                          >
                            <option value="true">True</option>
                            <option value="false">False</option>
                          </select>
                        ) : column.toLowerCase() === 'channeltype' && selectedFileType === FileType.Templates ? (
                          <select
                            className={inputClass}
                            value={cellValue.toString()}
                            onChange={(e) => updateCell(rowIndex, column, e.target.value)}
                            required
                          >
                            <option value="">Select Channel...</option>
                            {channelOptions.map((channel) => (
                              <option key={channel} value={channel}>
                                {channel}
                              </option>
                            ))}
                          </select>
                        ) : (
                          <input
                            type="text"
                            className={inputClass}
                            value={cellValue.toString()}
                            onChange={(e) => updateCell(rowIndex, column, e.target.value)}
                          />
                        )}
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
                      onClick={() => deleteRow(rowIndex)}
                      disabled={saving}
                    >
                      <i className="fas fa-trash"></i>
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};