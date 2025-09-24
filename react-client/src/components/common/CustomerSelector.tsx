import React, { useState } from 'react';

interface Customer {
  customerId: string;
  companyName: string;
}

interface CustomerSelectorProps {
  customers: Customer[];
  selectedCustomer: string;
  onCustomerChange: (customer: string) => void;
  title?: string;
}

export const CustomerSelector: React.FC<CustomerSelectorProps> = ({
  customers,
  selectedCustomer,
  onCustomerChange,
  title = 'Select Customer',
}) => {
  const [inputValue, setInputValue] = useState(selectedCustomer);
  const [showSuggestions, setShowSuggestions] = useState(false);

  const filteredCustomers = inputValue.length >= 3 
    ? customers.filter(customer => 
        customer.customerId.toLowerCase().includes(inputValue.toLowerCase()) ||
        customer.companyName.toLowerCase().includes(inputValue.toLowerCase())
      ).slice(0, 10)
    : [];

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setInputValue(value);
    setShowSuggestions(value.length >= 3);
    
    const exactMatch = customers.find(c => c.customerId === value);
    if (exactMatch) {
      onCustomerChange(value);
    } else if (selectedCustomer && !value) {
      onCustomerChange('');
    }
  };

  const handleSelectCustomer = (customerId: string) => {
    setInputValue(customerId);
    onCustomerChange(customerId);
    setShowSuggestions(false);
  };

  const handleClear = () => {
    setInputValue('');
    onCustomerChange('');
    setShowSuggestions(false);
  };

  return (
    <div className="card mb-4">
      <div className="card-header">
        <h3>
          <i className="fas fa-user-search me-2"></i>
          {title}
        </h3>
      </div>
      <div className="card-body">
        <div className="input-group">
          <span className="input-group-text">
            <i className="fas fa-search"></i>
          </span>
          <input
            type="text"
            className="form-control"
            placeholder="Type to search customers..."
            value={inputValue}
            onChange={handleInputChange}
            onFocus={() => inputValue.length >= 3 && setShowSuggestions(true)}
            onBlur={() => setTimeout(() => setShowSuggestions(false), 200)}
            autoComplete="off"
          />
          {inputValue && (
            <button
              className="btn btn-outline-secondary"
              type="button"
              onClick={handleClear}
              title="Clear selection"
            >
              <i className="fas fa-times"></i>
            </button>
          )}
        </div>
        
        {showSuggestions && filteredCustomers.length > 0 && (
          <div className="position-relative">
            <div className="list-group position-absolute w-100" style={{ zIndex: 1000, maxHeight: '200px', overflowY: 'auto' }}>
              {filteredCustomers.map((customer) => (
                <button
                  key={customer.customerId}
                  type="button"
                  className={`list-group-item list-group-item-action ${selectedCustomer === customer.customerId ? 'active' : ''}`}
                  onClick={() => handleSelectCustomer(customer.customerId)}
                >
                  {customer.customerId} ({customer.companyName})
                </button>
              ))}
            </div>
          </div>
        )}
        
        {selectedCustomer && (
          <div className="mt-3 p-2 bg-light rounded">
            <small className="text-muted">Selected Customer:</small>
            <div className="fw-bold text-primary">
              {selectedCustomer} ({customers.find(c => c.customerId === selectedCustomer)?.companyName || 'Unknown'})
            </div>
          </div>
        )}
      </div>
    </div>
  );
};