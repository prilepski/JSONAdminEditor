import { z } from 'zod';

export const dictionaryRowSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, 'Name is required'),
  value: z.string().min(1, 'Value is required'),
  isActive: z.boolean().default(true),
});

export const fileUploadSchema = z.object({
  fileType: z.number().min(1, 'File type is required'),
  customerName: z.string().optional(),
  jsonFile: z.instanceof(File, { message: 'File is required' }),
});

export const eventSchema = z.object({
  Event: z.string().min(1, 'Event name is required'),
  OrderType: z.string().min(1, 'Order type is required'),
  Phone: z.string().optional(),
  Email: z.string().email().optional(),
  IsSuppressed: z.boolean().default(false),
  Templates: z.object({
    Email: z.string().optional(),
    Sms: z.string().optional(),
    Voice: z.string().optional(),
  }).optional(),
});

export type DictionaryRowInput = z.infer<typeof dictionaryRowSchema>;
export type FileUploadInput = z.infer<typeof fileUploadSchema>;
export type EventInput = z.infer<typeof eventSchema>;