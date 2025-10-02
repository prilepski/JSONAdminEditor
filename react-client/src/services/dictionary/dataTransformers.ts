import { FileType, LogoUrl, TableData } from '../../types';

export const transformApiToDisplay = (data: any[], fileType: FileType) => {
  if (fileType === FileType.LogoUrls) {
    return {
      columnNames: ['fileName', 'url'],
      columnTypes: { 'fileName': 'string', 'url': 'string' },
      tableData: data as LogoUrl[]
    };
  }

  const rawColumnNames = data.length > 0 ? Object.keys(data[0]) : [];
  const columnNames = rawColumnNames.map(col => 
    col.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase())
  );
  
  const columnTypes = rawColumnNames.reduce((acc, col, index) => {
    const isActiveCol = col.toLowerCase() === 'isactive';
    const displayName = columnNames[index];
    return { ...acc, [displayName]: isActiveCol ? 'boolean' : 'string' };
  }, {});

  const displayTableData = data.map(row => {
    const displayRow: any = {};
    Object.entries(row).forEach(([apiField, value]) => {
      const displayName = apiField.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());
      displayRow[displayName] = value;
    });
    return displayRow;
  });

  return { columnNames, columnTypes, tableData: displayTableData };
};

export const transformDisplayToApi = (jsonData: TableData[], fileType: FileType) => {
  if (fileType === FileType.LogoUrls) {
    return jsonData;
  }

  return jsonData.map(row => {
    const apiRow: any = {};
    Object.entries(row).forEach(([displayName, value]) => {
      const apiFieldName = displayName.replace(/\s+/g, '').replace(/^./, str => str.toLowerCase());
      apiRow[apiFieldName] = value;
    });
    return apiRow;
  });
};