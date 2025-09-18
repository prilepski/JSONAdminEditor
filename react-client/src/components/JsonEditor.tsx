import React, { useState, useEffect } from 'react';
import { FileType, DictionaryData, ValidationError, TableData } from '../types';
import { useChannelOptionsQuery } from '../hooks/useDictionaryQuery';
import { DataTable } from './tables/DataTable';
import { SaveButton } from './common/SaveButton';

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
          <SaveButton
            onClick={handleSave}
            loading={saving}
            text="Save Changes"
          />
        </div>
      </div>

      {tableData.length === 0 ? (
        <div className="text-center py-4">
          <i className="fas fa-table fa-3x text-muted mb-3"></i>
          <p className="text-muted">No data in this dictionary yet. Click "Add Row" to start adding entries.</p>
        </div>
      ) : (
        <DataTable
          data={tableData}
          columns={dictionaryData.columnNames || []}
          columnTypes={dictionaryData.columnTypes || {}}
          validationErrors={validationErrors}
          onUpdateCell={updateCell}
          onDeleteRow={deleteRow}
          channelOptions={channelOptions}
          selectedFileType={selectedFileType}
        />
      )}
    </div>
  );
};