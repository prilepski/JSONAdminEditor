// Common component types for reuse across the application

export interface EventField {
  name: string;
  value: string;
  isRedefined: boolean;
  globalValue?: string;
  type: 'text' | 'checkbox';
}

export interface TemplateField {
  channel: string;
  value: string;
  isRedefined: boolean;
  globalValue?: string;
}

export interface ContentVariable {
  name: string;
  value: string;
  isRedefined: boolean;
  globalValue?: string;
}