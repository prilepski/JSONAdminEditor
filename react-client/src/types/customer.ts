import { EventData } from './events';

// Customer-specific types
export interface CustomerContentVariable {
  name: string;
  value: string;
  isRedefined: boolean;
  isCustomerSpecific: boolean;
  globalValue?: string;
}

export interface CustomerSettings {
  contentVariables: Record<string, string>;
  preferences?: Record<string, unknown>;
}

// Use EventData as CustomerEventData
export type CustomerEventData = EventData;

export interface PreferredCommunication {
  customerId: string;
  channels: {
    email: boolean;
    sms: boolean;
    voice: boolean;
  };
  preferences?: Record<string, unknown>;
}
