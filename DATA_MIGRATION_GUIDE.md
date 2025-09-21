# Data Migration Guide

## Overview
This guide covers the data migration considerations implemented to ensure backward compatibility between old dictionary-based data structures and new strongly-typed models.

## Migration Services

### DataMigrationService
Handles seamless transformation between Dictionary<string, object> and strongly-typed models.

**Key Methods:**
- `MigrateToModel<T>(Dictionary<string, object>? data)` - Convert dictionary to model
- `MigrateFromModel<T>(T model)` - Convert model to dictionary
- `ValidateDataStructure<T>(Dictionary<string, object>? data)` - Validate data compatibility

### BackwardCompatibilityService
Ensures data format compatibility and handles missing fields.

**Key Methods:**
- `EnsureCompatibility(Dictionary<string, object> data, Type targetType)` - Fix data structure issues
- `SafeDeserialize<T>(object? data)` - Safe deserialization with fallback

## Data Structure Transformations

### EventMapping Migration
**Old Format:**
```json
{
  "Event": "Ready For Scheduling",
  "OrderType": "Delivery",
  "Templates": {
    "Email": "template-id-1",
    "Sms": "template-id-2"
  }
}
```

**New Format (EventMapping model):**
```json
{
  "Event": "Ready For Scheduling",
  "OrderType": "Delivery",
  "Phone": "$consignee.phone$",
  "Email": "$consignee.email$",
  "Templates": {
    "Email": "template-id-1",
    "Sms": "template-id-2"
  },
  "IsSuppressed": false,
  "PreferredCommunication": [],
  "ContentVariables": {},
  "TriggerConditions": {},
  "ContentVariablesOverrides": {}
}
```

### AfterHours Migration
**Old Format:**
```json
{
  "start": "22:00",
  "end": "08:00"
}
```

**New Format (AfterHours model):**
```json
{
  "RestrictedHoursPeriod": {
    "Start": "22:00",
    "End": "08:00"
  },
  "RestrictedDays": [0, 6],
  "ExceptionOfValidation": {
    "Events": []
  }
}
```

### OptOut Migration
**Old Format:**
```json
{
  "keywords": "STOP,QUIT",
  "message": "You have been unsubscribed"
}
```

**New Format (OptOut model):**
```json
{
  "en": {
    "US": {
      "optOutKeywords": "STOP, QUIT, CANCEL",
      "optInKeywords": "START, YES",
      "helpKeywords": "HELP, INFO",
      "optOutFooter": "Reply STOP to opt out",
      "optOutPhrase": "You have been unsubscribed",
      "optInPhrase": "You have been subscribed",
      "optInMessage": "Welcome! You will receive notifications",
      "helpPhrase": "For help, contact support"
    }
  }
}
```

## Fallback Mechanisms

### Missing Data Handling
- **Default Values**: Missing required fields are populated with sensible defaults
- **Null Safety**: All operations handle null/missing data gracefully
- **Type Conversion**: Automatic conversion between compatible types (string ↔ enum, etc.)

### Error Recovery
- **Validation Failures**: Invalid data structures fall back to empty/default models
- **Serialization Errors**: JSON parsing errors return null with logging
- **Model Binding**: Failed model binding returns appropriate error responses

## Testing Data Migration

### Validation Steps
1. **Structure Validation**: Verify data can be migrated to target model
2. **Round-trip Testing**: Ensure data survives model → dictionary → model conversion
3. **Backward Compatibility**: Confirm old data formats still work
4. **Forward Compatibility**: Ensure new models serialize correctly

### Example Usage
```csharp
// Migrate old data to new model
var oldData = await _mockDb.GetCustomerAsync(customerId);
var afterHours = _migrationService.MigrateToModel<AfterHours>(oldData);

// Save new model back to storage
var newData = _migrationService.MigrateFromModel(afterHours);
await _mockDb.SaveCustomerAsync(customerId, newData);
```

## Best Practices

### Controller Implementation
- Always use migration services for data transformation
- Validate data structure before processing
- Provide meaningful error messages for migration failures
- Log migration issues for debugging

### Service Layer
- Implement both generic and specific migration methods
- Handle edge cases (null, empty, malformed data)
- Maintain audit trail of data transformations
- Use consistent error handling patterns

### Model Design
- Use data annotations for validation
- Provide sensible default values
- Design for extensibility (optional properties)
- Document breaking changes clearly