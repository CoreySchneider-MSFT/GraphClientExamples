# Graph Client Examples

This repository contains example implementations of Microsoft Graph API clients across different SDK versions, demonstrating best practices and evolution of the Graph SDK.

## Project Structure

- **GraphClientV3** - Implementation using Microsoft Graph SDK v3.x
- **GraphClientV4** - Implementation using Microsoft Graph SDK v4.x
- **GraphClientV5** - Implementation using Microsoft Graph SDK v5.x with enhanced features:
  - Azure Identity integration
  - Custom logging handlers
  - Polly-based retry policies
  - Configurable request handlers
- **GraphHandlers** - Common handlers for logging and retry logic
- **GraphClientSettings** - Shared configuration and settings management

## Features

- Azure AD authentication using client credentials
- Custom HTTP message handlers for logging and monitoring
- Exponential backoff retry policies
- Configurable request pipeline
- Structured logging integration

## Setup

1. Clone the repository
2. Create `appsettings.json` with your EntraID App Registration credentials:
```json
{
  "ClientId": "your-client-id",
  "ClientSecret": "your-client-secret",
  "TenantId": "your-tenant-id"
}
```
3. Build and run the solution

## Dependencies

- Azure.Identity
- Microsoft.Graph (v3.x, v4.x, v5.x)
- Microsoft.Extensions.Logging
- Polly
- Newtonsoft.Json

## Best Practices Demonstrated

- Separation of concerns (settings, handlers, client implementations)
- Proper error handling and retry logic
- Structured logging
- Configuration management
- Azure Identity integration

## Security Note

Sensitive configuration (client secrets, tenant IDs) should be stored in `appsettings.development.json` which is excluded from source control, or in 'dotnet user secrets' as best practice.
