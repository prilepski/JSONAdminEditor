# Configuration Setup

## Sensitive Configuration Files

This project uses local configuration files to store sensitive information like AWS credentials. These files are excluded from git to keep your credentials secure.

### Required Local Files

After cloning the repository, you need to create these local configuration files:

#### For Development:
Create `appsettings.Development.Local.json` with your actual credentials:

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
Create `appsettings.Local.json` with your production credentials:

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

### Configuration Hierarchy

ASP.NET Core loads configuration in this order:
1. `appsettings.json` (committed to git, no sensitive data)
2. `appsettings.{Environment}.json` (committed to git, no sensitive data)
3. `appsettings.Local.json` (local only, contains sensitive data)
4. `appsettings.{Environment}.Local.json` (local only, contains sensitive data)

The local files will override the empty values in the committed configuration files.

### Security Notes

- **Never commit** `appsettings.Local.json` or `appsettings.Development.Local.json`
- These files are already added to `.gitignore`
- Each developer needs to create their own local configuration files
- Use different credentials for development and production environments
