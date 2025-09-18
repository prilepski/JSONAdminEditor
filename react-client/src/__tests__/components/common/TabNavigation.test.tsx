import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { TabNavigation } from '../../../components/common/TabNavigation';

describe('TabNavigation', () => {
  const mockTabs = [
    { id: 'tab1', label: 'Tab 1', icon: 'fa-home' },
    { id: 'tab2', label: 'Tab 2', icon: 'fa-user' },
  ];
  const mockOnTabChange = jest.fn();

  beforeEach(() => {
    mockOnTabChange.mockClear();
  });

  it('renders all tabs', () => {
    render(
      <TabNavigation
        tabs={mockTabs}
        activeTab="tab1"
        onTabChange={mockOnTabChange}
      />
    );
    
    expect(screen.getByText('Tab 1')).toBeInTheDocument();
    expect(screen.getByText('Tab 2')).toBeInTheDocument();
  });

  it('marks active tab correctly', () => {
    render(
      <TabNavigation
        tabs={mockTabs}
        activeTab="tab1"
        onTabChange={mockOnTabChange}
      />
    );
    
    const activeTab = screen.getByText('Tab 1').closest('button');
    const inactiveTab = screen.getByText('Tab 2').closest('button');
    
    expect(activeTab).toHaveClass('active');
    expect(inactiveTab).not.toHaveClass('active');
  });

  it('calls onTabChange when tab is clicked', async () => {
    render(
      <TabNavigation
        tabs={mockTabs}
        activeTab="tab1"
        onTabChange={mockOnTabChange}
      />
    );
    
    await userEvent.click(screen.getByText('Tab 2'));
    
    expect(mockOnTabChange).toHaveBeenCalledWith('tab2');
  });
});