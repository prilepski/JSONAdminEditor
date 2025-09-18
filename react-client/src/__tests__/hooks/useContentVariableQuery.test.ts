import { renderHook, waitFor } from '@testing-library/react';
import { useContentVariablesQuery, useContentVariablesMutation } from '../../hooks/useContentVariableQuery';
import { TestWrapper } from '../utils/testUtils';
import * as contentVariableService from '../../services/contentVariableService';

jest.mock('../../services/contentVariableService');

const mockContentVariableService = contentVariableService.contentVariableService as jest.Mocked<typeof contentVariableService.contentVariableService>;

describe('useContentVariableQuery', () => {
  it('fetches content variables successfully', async () => {
    const mockData = [{ 'Variable Name': 'test', 'Variable Value': 'value' }];
    mockContentVariableService.getContentVariables.mockResolvedValue(mockData);

    const { result } = renderHook(() => useContentVariablesQuery(), {
      wrapper: TestWrapper,
    });

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });

    expect(result.current.data).toEqual(mockData);
  });

  it('handles error when fetching fails', async () => {
    mockContentVariableService.getContentVariables.mockRejectedValue(new Error('API Error'));

    const { result } = renderHook(() => useContentVariablesQuery(), {
      wrapper: TestWrapper,
    });

    await waitFor(() => {
      expect(result.current.isError).toBe(true);
    });
  });
});

describe('useContentVariablesMutation', () => {
  it('saves content variables successfully', async () => {
    const mockResponse = { success: true, message: 'Saved successfully' };
    mockContentVariableService.saveContentVariables.mockResolvedValue(mockResponse);

    const { result } = renderHook(() => useContentVariablesMutation(), {
      wrapper: TestWrapper,
    });

    const testData = [{ 'Variable Name': 'test', 'Variable Value': 'value' }];
    
    await result.current.mutateAsync(testData);

    expect(mockContentVariableService.saveContentVariables).toHaveBeenCalledWith(testData);
  });
});