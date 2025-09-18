import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { SaveButton } from '../../../components/common/SaveButton';

describe('SaveButton', () => {
  it('renders with default text', () => {
    render(<SaveButton onClick={jest.fn()} />);

    expect(screen.getByRole('button')).toHaveTextContent('Save Changes');
  });

  it('renders with custom text', () => {
    render(<SaveButton onClick={jest.fn()} text="Custom Save" />);

    expect(screen.getByRole('button')).toHaveTextContent('Custom Save');
  });

  it('shows loading state', () => {
    render(<SaveButton onClick={jest.fn()} loading={true} />);

    expect(screen.getByRole('button')).toHaveTextContent('Saving...');
    expect(screen.getByRole('button')).toBeDisabled();
  });

  it('calls onClick when clicked', async () => {
    const handleClick = jest.fn();
    render(<SaveButton onClick={handleClick} />);

    await userEvent.click(screen.getByRole('button'));

    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it('is disabled when disabled prop is true', () => {
    render(<SaveButton onClick={jest.fn()} disabled={true} />);

    expect(screen.getByRole('button')).toBeDisabled();
  });
});
