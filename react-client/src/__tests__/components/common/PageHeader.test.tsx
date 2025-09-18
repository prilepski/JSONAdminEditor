import { render, screen } from '@testing-library/react';
import { PageHeader } from '../../../components/common/PageHeader';

describe('PageHeader', () => {
  it('renders title with icon', () => {
    render(<PageHeader icon="fa-test" title="Test Title" />);
    
    expect(screen.getByRole('heading', { level: 1 })).toHaveTextContent('Test Title');
    expect(screen.getByRole('heading')).toContainHTML('<i class="fas fa-test me-2"></i>');
  });

  it('renders description when provided', () => {
    render(<PageHeader icon="fa-test" title="Test Title" description="Test description" />);
    
    expect(screen.getByText('Test description')).toBeInTheDocument();
  });

  it('does not render description when not provided', () => {
    render(<PageHeader icon="fa-test" title="Test Title" />);
    
    expect(screen.queryByText(/description/)).not.toBeInTheDocument();
  });
});