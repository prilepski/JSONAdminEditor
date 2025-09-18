import { isValidFileType, isTableData, isValidationError } from '../../types/utils';
import { FileType } from '../../types';

describe('Type Guards', () => {
  describe('isValidFileType', () => {
    it('returns true for valid file types', () => {
      expect(isValidFileType(1)).toBe(true);
      expect(isValidFileType(2)).toBe(true);
    });

    it('returns false for invalid file types', () => {
      expect(isValidFileType(999)).toBe(false);
      expect(isValidFileType(-1)).toBe(false);
    });
  });

  describe('isTableData', () => {
    it('returns true for valid table data objects', () => {
      expect(isTableData({ name: 'test', value: 'data' })).toBe(true);
      expect(isTableData({})).toBe(true);
    });

    it('returns false for invalid data', () => {
      expect(isTableData(null)).toBe(false);
      expect(isTableData([])).toBe(false);
      expect(isTableData('string')).toBe(false);
      expect(isTableData(123)).toBe(false);
    });
  });

  describe('isValidationError', () => {
    it('returns true for valid validation error objects', () => {
      const validError = { fieldName: 'test', message: 'error message' };
      expect(isValidationError(validError)).toBe(true);
    });

    it('returns false for invalid validation error objects', () => {
      expect(isValidationError({})).toBe(false);
      expect(isValidationError({ fieldName: 'test' })).toBe(false);
      expect(isValidationError({ message: 'error' })).toBe(false);
      expect(isValidationError(null)).toBe(false);
    });
  });
});