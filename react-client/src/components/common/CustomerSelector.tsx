import React from 'react';

interface CustomerSelectorProps {
  customers: string[];
  selectedCustomer: string;
  onCustomerChange: (customer: string) => void;
  title?: string;
}

export const CustomerSelector: React.FC<CustomerSelectorProps> = ({
  customers,
  selectedCustomer,
  onCustomerChange,
  title = "Select Customer"
}) => (
  <div className="card mb-4">
    <div className="card-header">
      <h3><i className="fas fa-user-search me-2"></i>{title}</h3>
    </div>
    <div className="card-body">
      <select
        className="form-select"
        value={selectedCustomer}
        onChange={(e) => onCustomerChange(e.target.value)}
      >
        <option value="">Select a customer...</option>
        {customers.map(customer => (
          <option key={customer} value={customer}>{customer}</option>
        ))}
      </select>
    </div>
  </div>
);