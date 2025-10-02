import { FileType } from '../../types';

export const DICTIONARY_ENDPOINTS = {
  [FileType.Templates]: 'templates',
  [FileType.EventTriggers]: 'event-triggers', 
  [FileType.EventChannels]: 'event-channels',
  [FileType.OrderTypes]: 'order-types',
  [FileType.Customers]: 'customers',
  [FileType.LogoUrls]: 'logo-url'
} as const;

export const AVAILABLE_DICTIONARIES = [
  { type: FileType.Templates, name: 'Notification Templates', endpoint: 'templates' },
  { type: FileType.EventTriggers, name: 'Event Triggers', endpoint: 'event-triggers' },
  { type: FileType.EventChannels, name: 'Event Channels', endpoint: 'event-channels' },
  { type: FileType.OrderTypes, name: 'Order Types', endpoint: 'order-types' },
  { type: FileType.Customers, name: 'Customers', endpoint: 'customers' },
  { type: FileType.LogoUrls, name: 'Logo URLs', endpoint: 'logo-url' },
];

export const getEndpoint = (fileType: FileType): string => {
  const endpoint = DICTIONARY_ENDPOINTS[fileType];
  if (!endpoint) {
    throw new Error(`Unsupported dictionary type: ${fileType}`);
  }
  return endpoint;
};