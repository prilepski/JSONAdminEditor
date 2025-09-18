import { render, screen, waitFor } from '@testing-library/react';
import { ContentVariables } from '../../pages/ContentVariables';
import { renderWithProviders } from '../utils/testUtils';
import * as contentVariableHooks from '../../hooks/useContentVariableQuery';

jest.mock('../../hooks/useContentVariableQuery');

const mockUseContentVariablesQuery =
  contentVariableHooks.useContentVariablesQuery as jest.MockedFunction<
    typeof contentVariableHooks.useContentVariablesQuery
  >;
const mockUseContentVariablesMutation =
  contentVariableHooks.useContentVariablesMutation as jest.MockedFunction<
    typeof contentVariableHooks.useContentVariablesMutation
  >;

describe('ContentVariables', () => {
  beforeEach(() => {
    mockUseContentVariablesQuery.mockReturnValue({
      data: [
        {
          'Variable Name': 'test_var',
          'Variable Value': 'test_value',
          Description: 'Test description',
        },
      ],
      isLoading: false,
    } as any);

    mockUseContentVariablesMutation.mockReturnValue({
      mutateAsync: jest.fn(),
      isPending: false,
    } as any);
  });

  it('renders page header', () => {
    renderWithProviders(<ContentVariables />);

    expect(screen.getByRole('heading', { level: 1 })).toHaveTextContent(
      'Content Variables Management'
    );
  });

  it('renders description', () => {
    renderWithProviders(<ContentVariables />);

    expect(screen.getByText(/Manage global content variables/)).toBeInTheDocument();
  });

  it('renders editor when data is loaded', async () => {
    renderWithProviders(<ContentVariables />);

    await waitFor(() => {
      expect(screen.getByText('Content Variables Editor')).toBeInTheDocument();
    });
  });

  it('shows loading skeleton when loading', () => {
    mockUseContentVariablesQuery.mockReturnValue({
      data: [],
      isLoading: true,
    } as any);

    renderWithProviders(<ContentVariables />);

    expect(screen.getByTestId('skeleton')).toBeInTheDocument();
  });
});
