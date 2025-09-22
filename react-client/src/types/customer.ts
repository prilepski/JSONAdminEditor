// Customer data structure based on mock files
export interface CustomerSettings {
  customerName?: string;
  customSettings?: {
    theme?: string;
    logoUrl?: string;
    primaryColor?: string;
    secondaryColor?: string;
  };
  features?: {
    enableAdvancedReporting?: boolean;
    customDashboard?: boolean;
    maxUsers?: number;
  };
  integrations?: {
    sso?: {
      enabled?: boolean;
      provider?: string;
    };
    api?: {
      rateLimit?: number;
      customEndpoints?: string[];
    };
  };
  ContentVariables?: Record<string, string>;
  Events?: CustomerEventData[];
}

export interface CustomerEventData {
  Event: string;
  OrderType: string;
  Phone?: string;
  Email?: string;
  Logo?: string;
  IsSuppressed?: boolean;
  Templates?: {
    Email?: string;
    Sms?: string;
    Voice?: string;
  };
  ContentVariables?: Record<string, string>;
}

export interface CustomerLookupResult {
  customerId: string;
  companyName: string;
  contactPerson?: string;
  email?: string;
  phone?: string;
  address?: string;
  IsActive?: boolean;
}

export interface CustomerContentVariable {
  name: string;
  value: string;
  isRedefined: boolean;
  isCustomerSpecific: boolean;
  globalValue?: string;
}
