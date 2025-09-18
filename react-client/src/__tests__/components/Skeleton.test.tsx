import { render, screen } from '@testing-library/react';
import { Skeleton } from '../../components/Skeleton';

describe('Skeleton', () => {
  it('renders children when not loading', () => {
    render(
      <Skeleton loading={false}>
        <div>Test Content</div>
      </Skeleton>
    );

    expect(screen.getByText('Test Content')).toBeInTheDocument();
    expect(screen.queryByTestId('skeleton')).not.toBeInTheDocument();
  });

  it('renders skeleton when loading', () => {
    render(
      <Skeleton loading={true}>
        <div>Test Content</div>
      </Skeleton>
    );

    expect(screen.getByTestId('skeleton')).toBeInTheDocument();
    expect(screen.queryByText('Test Content')).not.toBeInTheDocument();
  });

  it('renders correct number of skeleton rows', () => {
    render(
      <Skeleton loading={true} rows={3}>
        <div>Test Content</div>
      </Skeleton>
    );

    const skeleton = screen.getByTestId('skeleton');
    expect(skeleton.children).toHaveLength(3);
  });

  it('applies custom height', () => {
    render(
      <Skeleton loading={true} height="50px">
        <div>Test Content</div>
      </Skeleton>
    );

    const skeletonRow = screen.getByTestId('skeleton').firstChild as HTMLElement;
    expect(skeletonRow).toHaveStyle('height: 50px');
  });
});
