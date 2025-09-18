import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { JsonEditor } from '../../components/JsonEditor';
import { FileType } from '../../types';

describe('JsonEditor', () => {
  const mockDictionaryData = {
    columnNames: ['Name', 'Value'],
    columnTypes: { Name: 'text', Value: 'text' },
    tableData: [{ Name: 'test', Value: 'value' }],
    filePath: 'test.json',
    fileName: 'test.json',
    isValidJson: true,
  };

  const mockProps = {
    dictionaryData: mockDictionaryData,
    selectedFileType: FileType.ContentVariables,
    validationErrors: [],
    onSave: jest.fn(),
    onClearValidationErrors: jest.fn(),
  };

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('renders table with data', () => {
    render(<JsonEditor {...mockProps} />);
    
    expect(screen.getByText('Name')).toBeInTheDocument();
    expect(screen.getByText('Value')).toBeInTheDocument();
    expect(screen.getByDisplayValue('test')).toBeInTheDocument();
    expect(screen.getByDisplayValue('value')).toBeInTheDocument();
  });

  it('renders save button', () => {
    render(<JsonEditor {...mockProps} />);
    
    expect(screen.getByRole('button', { name: /save/i })).toBeInTheDocument();
  });

  it('calls onSave when save button is clicked', async () => {
    render(<JsonEditor {...mockProps} />);
    
    await userEvent.click(screen.getByRole('button', { name: /save/i }));
    
    expect(mockProps.onSave).toHaveBeenCalledWith(mockDictionaryData.tableData);
  });

  it('renders add row button', () => {
    render(<JsonEditor {...mockProps} />);
    
    expect(screen.getByRole('button', { name: /add row/i })).toBeInTheDocument();
  });

  it('shows validation errors', () => {
    const propsWithErrors = {
      ...mockProps,
      validationErrors: [{ fieldName: 'Name', message: 'Name is required' }],
    };

    render(<JsonEditor {...propsWithErrors} />);
    
    expect(screen.getByText('Name is required')).toBeInTheDocument();
  });
});