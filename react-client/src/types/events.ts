import { ChannelType } from './common';

// Re-export ChannelType for convenience
export type { ChannelType } from './common';

// Template-related types
export interface Template {
  templateId: string;
  templateName: string;
  channelType: ChannelType;
}

export interface TemplateSet {
  Email?: string;
  Sms?: string;
  Voice?: string;
}

// Event data structure
export interface EventData {
  Event?: string;
  OrderType?: string;
  Phone?: string;
  Email?: string;
  Logo?: string;
  IsSuppressed?: boolean;
  Templates?: TemplateSet;
  ContentVariables?: Record<string, string>;
}

// Field types for forms
export interface EventField {
  name: string;
  value: string;
  isRedefined: boolean;
  globalValue?: string;
  type: 'text' | 'checkbox' | 'select';
}

export interface TemplateField {
  channel: ChannelType;
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