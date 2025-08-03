# Configuration Setup

## Sensitive Configuration Files

This project uses local configuration files to store sensitive information like AWS credentials. These files are excluded from git to keep your credentials secure.

## Storage Types

The application supports two storage backends that can be configured in your settings files:

### 1. FileSystem Storage (Default)
- Files are stored locally in the `wwwroot/data` folder
- No AWS credentials required
- Good for development and testing

### 2. AWS S3 Storage
- Files are stored in an AWS S3 bucket
- Requires AWS credentials (Access Key and Secret Key)
- Recommended for production deployments

## Switching Between Storage Types

### Configure FileSystem Storage

In your main configuration files (`appsettings.json` or `appsettings.Development.json`):

```json
{
  "StorageSettings": {
    "StorageType": "FileSystem"
  }
}
```

### Configure S3 Storage

1. In your main configuration file, set:
```json
{
  "StorageSettings": {
    "StorageType": "S3",
    "S3Settings": {
      "Region": "us-east-1",
      "BucketName": "",
      "AccessKey": "",
      "SecretKey": "",
      "UseCredentialsFromEnvironment": false
    }
  }
}
```

2. Create your Local configuration file with actual credentials:

#### For Development:
Create `appsettings.Development.Local.json`:

```json
{
  "StorageSettings": {
    "S3Settings": {
      "BucketName": "your-actual-bucket-name",
      "AccessKey": "your-actual-access-key",
      "SecretKey": "your-actual-secret-key"
    }
  }
}
```

#### For Production:
Create `appsettings.Local.json`:

```json
{
  "StorageSettings": {
    "S3Settings": {
      "BucketName": "your-production-bucket-name",
      "AccessKey": "your-production-access-key",
      "SecretKey": "your-production-secret-key"
    }
  }
}
```

## File Structure and Organization

### FileSystem Storage Structure
When using FileSystem storage, files are organized in the `wwwroot/data` folder:

```
wwwroot/data/
├── dictionaries/
│   ├── templates.json          # Notification templates (Email, SMS, Voice)
│   ├── customers.json          # Customer directory
│   ├── event-triggers.json     # Available event triggers
│   ├── event-channels.json     # Communication channels (Email, Sms, Voice)
│   └── order-types.json        # Order types (Delivery, Pickup)
├── customers/
│   ├── CUSTOMER1.json          # Customer-specific overrides
│   ├── CUSTOMER2.json          # Customer-specific overrides
│   └── ...
├── templates/
│   └── event-template.json     # Event template structure
└── notifications.json          # Main notifications configuration
```

### S3 Storage Structure
When using S3 storage, the same structure is maintained but stored in your S3 bucket:

```
s3://your-bucket-name/
├── dictionaries/
│   ├── templates.json
│   ├── customers.json
│   ├── event-triggers.json
│   ├── event-channels.json
│   └── order-types.json
├── customers/
│   ├── CUSTOMER1.json
│   ├── CUSTOMER2.json
│   └── ...
├── templates/
│   └── event-template.json
└── notifications.json
```

## Key Configuration Files

### Core Dictionary Files

1. **`dictionaries/templates.json`**
   - Contains notification templates for different channels
   - Structure: Array of objects with `templateId`, `templateName`, `channelType`
   - Example:
   ```json
   [
     {
       "templateId": "d-123456789",
       "templateName": "Ready For Scheduling - Delivery",
       "channelType": "Email"
     }
   ]
   ```

2. **`dictionaries/customers.json`**
   - Customer directory with company information
   - Structure: Array of customer objects
   - Used for customer lookup and validation

3. **`dictionaries/event-triggers.json`**
   - Available event types that can trigger notifications
   - Structure: Array with `Event Name`, `ByOrderType`, `IsActive`

4. **`dictionaries/event-channels.json`**
   - Communication channels (Email, Sms, Voice)
   - Structure: Array with `Channel Name`, `IsActive`

5. **`dictionaries/order-types.json`**
   - Available order types (Delivery, Pickup, etc.)
   - Structure: Array with order type information

### Main Configuration Files

6. **`notifications.json`**
   - **Primary configuration file** - This is where your notification settings are stored
   - Contains global events, content variables, and preferred communication settings
   - Structure:
   ```json
   {
     "Events": [...],              // Event configurations by order type
     "ContentVariables": {...},    // Global content variables
     "PreferredCommunication": [...] // Communication preferences
   }
   ```

7. **`customers/CUSTOMERID.json`**
   - Customer-specific overrides for events and settings
   - Inherits from global `notifications.json` but can override specific values
   - Structure: Same as notifications.json but only contains customer-specific changes

## Configuration Hierarchy

ASP.NET Core loads configuration in this order:
1. `appsettings.json` (committed to git, no sensitive data)
2. `appsettings.{Environment}.json` (committed to git, no sensitive data)
3. `appsettings.Local.json` (local only, contains sensitive data)
4. `appsettings.{Environment}.Local.json` (local only, contains sensitive data)

The local files will override the empty values in the committed configuration files.

## Security Notes

- **Never commit** `appsettings.Local.json` or `appsettings.Development.Local.json`
- These files are already added to `.gitignore`
- Each developer needs to create their own local configuration files
- Use different credentials for development and production environments
- When using S3, ensure your bucket has appropriate security policies

## Migration Between Storage Types

### From FileSystem to S3:
1. Set up your S3 bucket and credentials
2. Update your configuration to use S3 storage type
3. Upload your existing `wwwroot/data` files to S3 using the same structure
4. Test the application to ensure connectivity

### From S3 to FileSystem:
1. Download your S3 files to the `wwwroot/data` folder
2. Update your configuration to use FileSystem storage type
3. Ensure the local directory structure matches the S3 structure
