# AWS Integration Rules
## Purpose
To ensure that all integrations with AWS services follow security guidelines and use best practices for resource access.

## Priority
Critical

## Instructions
-   **Use AWS SDK for .NET**: All interactions with AWS services must be done through the official AWS SDK for .NET.
-   **Avoid hardcoding credentials**: Never embed AWS access keys or secrets directly in code. Use the AWS SDK's default credential provider chain (e.g., IAM roles, environment variables).
-   **Configure logging for CloudWatch**: All application logs should be configured to be sent to Amazon CloudWatch Logs for centralized monitoring.
-   **Use AWS Secrets Manager for secrets**: Use AWS Secrets Manager for storing and retrieving sensitive configuration data like database connection strings.
