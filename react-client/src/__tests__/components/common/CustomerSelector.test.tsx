import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { CustomerSelector } from '../../../components/common/CustomerSelector';

describe('CustomerSelector', () => {
  const mockCustomers = ['Customer A', 'Customer B', 'Customer C'];
  const mockOnChange = jest.fn();

  beforeEach(() => {
    mockOnChange.mockClear();
  });

  it('renders with default title', () => {
    render(
      <CustomerSelector
        customers={mockCustomers}
        selectedCustomer=""
        onCustomerChange={mockOnChange}
      />
    );
    
    expect(screen.getByText('Select Customer')).toBeInTheDocument();
  });

  it('renders with custom title', () => {
    render(
      <CustomerSelector
        customers={mockCustomers}
        selectedCustomer=""
        onCustomerChange={mockOnChange}
        title="Choose Customer"
      />
    );
    
    expect(screen.getByText('Choose Customer')).toBeInTheDocument();
  });

  it('renders all customer options', () => {
    render(
      <CustomerSelector
        customers={mockCustomers}
        selectedCustomer=""
        onCustomerChange={mockOnChange}
      />
    );
    
    mockCustomers.forEach(customer => {
      expect(screen.getByRole('option', { name: customer })).toBeInTheDocument();
    });
  });

  it('calls onCustomerChange when selection changes', async () => {
    render(
      <CustomerSelector
        customers={mockCustomers}
        selectedCustomer=""
        onCustomerChange={mockOnChange}
      />
    );
    
    await userEvent.selectOptions(screen.getByRole('combobox'), 'Customer A');
    
    expect(mockOnChange).toHaveBeenCalledWith('Customer A');
  });
});