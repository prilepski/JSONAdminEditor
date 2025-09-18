describe('Validation Utils', () => {
  describe('validateRequired', () => {
    it('returns error for empty string', () => {
      const validateRequired = (value: string) => 
        value.trim() === '' ? 'Field is required' : null;
      
      expect(validateRequired('')).toBe('Field is required');
      expect(validateRequired('   ')).toBe('Field is required');
    });

    it('returns null for valid string', () => {
      const validateRequired = (value: string) => 
        value.trim() === '' ? 'Field is required' : null;
      
      expect(validateRequired('valid')).toBeNull();
    });
  });

  describe('validateEmail', () => {
    it('validates email format', () => {
      const validateEmail = (email: string) => {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email) ? null : 'Invalid email format';
      };
      
      expect(validateEmail('test@example.com')).toBeNull();
      expect(validateEmail('invalid-email')).toBe('Invalid email format');
      expect(validateEmail('test@')).toBe('Invalid email format');
    });
  });

  describe('validateJSON', () => {
    it('validates JSON string', () => {
      const validateJSON = (jsonString: string) => {
        try {
          JSON.parse(jsonString);
          return null;
        } catch {
          return 'Invalid JSON format';
        }
      };
      
      expect(validateJSON('{"valid": "json"}')).toBeNull();
      expect(validateJSON('invalid json')).toBe('Invalid JSON format');
      expect(validateJSON('{"incomplete":')).toBe('Invalid JSON format');
    });
  });
});