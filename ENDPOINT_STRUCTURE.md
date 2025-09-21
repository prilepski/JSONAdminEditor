# API Endpoint Structure

## Customer-Level Endpoints

### Customer Management
- `GET /api/customers` - Get all customer IDs
- `GET /api/customers/{customerId}/events` - Get customer events
- `POST /api/customers/{customerId}/events` - Save customer events
- `GET /api/customers/{customerId}/settings` - Get customer settings
- `POST /api/customers/{customerId}/settings` - Save customer settings
- `GET /api/customers/{customerId}/afterhours` - Get customer after hours settings
- `POST /api/customers/{customerId}/afterhours` - Save customer after hours settings

## Global-Level Endpoints

### Configuration Management
- `GET /api/optout` - Get global opt-out settings
- `POST /api/optout/save` - Save global opt-out settings
- `GET /api/afterhours` - Get global after hours settings
- `POST /api/afterhours/save` - Save global after hours settings
- `GET /api/notification-mapping` - Get notification mapping configuration
- `POST /api/notification-mapping/save` - Save notification mapping configuration

### Content Management
- `GET /api/content-variables` - Get content variables
- `POST /api/content-variables/save` - Save content variables
- `GET /api/preferred-communication` - Get preferred communication settings
- `POST /api/preferred-communication/save` - Save preferred communication settings

### Event Management
- `GET /api/events/triggers` - Get active event triggers
- `GET /api/events/order-types` - Get available order types
- `GET /api/events/templates` - Get available templates
- `GET /api/events/supports-order-type` - Check if event supports order type
- `GET /api/events/data` - Get event by name and order type
- `GET /api/events/template` - Get event template
- `POST /api/events/save` - Save event configuration

### Dictionary Management
- `GET /api/dictionaries/data` - Get dictionary data by type
- `POST /api/dictionaries/save` - Save dictionary data
- `POST /api/dictionaries/upload` - Upload dictionary file

## Response Format

All endpoints return standardized responses:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { ... },
  "error": null
}
```

## Model Usage

### Customer-Level Models
- `AfterHours` - Customer-specific after hours configuration
- `EventMapping` - Customer event mappings
- `Dictionary<string, object>` - Customer settings (legacy format)

### Global-Level Models
- `OptOut` - Global opt-out configuration with locale support
- `AfterHours` - Global after hours configuration
- `NotificationMapping` - Complete notification mapping configuration
- `PreferredCommunication` - Communication preferences
- `List<Dictionary<string, object>>` - Content variables (legacy format)

### Validation
All models use data annotations for validation with automatic model state validation in controllers.