import '@testing-library/jest-dom';

// Mock jest functions for vitest
global.jest = {
  fn: vi.fn,
  mock: vi.mock,
  clearAllMocks: vi.clearAllMocks,
} as any;
