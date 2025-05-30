# Configuration Management

This document describes how configuration is managed across different environments in the SoftwareDeveloperCase application.

## Environment-Specific Configuration Files

The application uses different configuration files for each environment:

- `appsettings.json` - Base configuration shared across environments
- `appsettings.Development.json` - Development-specific settings
- `appsettings.Staging.json` - Staging environment settings
- `appsettings.Production.json` - Production environment settings

## Strongly-Typed Configuration

All configuration sections use strongly-typed classes with validation:

### DatabaseSettings
- **Section**: `DatabaseSettings`
- **Class**: `SoftwareDeveloperCase.Application.Models.DatabaseSettings`
- **Purpose**: Database connection and Entity Framework configuration

### EmailSettings
- **Section**: `EmailSettings`
- **Class**: `SoftwareDeveloperCase.Application.Models.EmailSettings`
- **Purpose**: SMTP email service configuration

## Environment Variables for Sensitive Data

Sensitive configuration values should be provided through environment variables:

### Database Configuration
- `DATABASE_CONNECTION_STRING` - Override database connection string

### Email Configuration
- `EMAIL_USERNAME` - SMTP authentication username
- `EMAIL_PASSWORD` - SMTP authentication password
- `EMAIL_FROM_ADDRESS` - Override sender email address

## Configuration Validation

Configuration is validated using the IValidateOptions pattern:
- `EmailSettingsValidator` - Validates email configuration
- `DatabaseSettingsValidator` - Validates database configuration

Validation can be performed manually using extension methods:
```csharp
var emailValidation = emailSettings.ValidateEmailSettings();
var databaseValidation = databaseSettings.ValidateDatabaseSettings();
```

## Environment Setup

### Development
- Uses in-memory database by default
- Enables detailed Entity Framework errors and sensitive data logging
- Uses localhost SMTP settings for testing

### Staging
- Uses SQL Server database
- Reduced logging verbosity
- Production-like SMTP configuration

### Production
- Uses SQL Server database
- Minimal logging (Warning level and above)
- Secure SMTP configuration
- Sensitive data logging disabled

## Best Practices

1. **Never commit sensitive data** - Use environment variables for passwords, API keys, and connection strings
2. **Validate configuration** - Use the provided validators to ensure configuration is correct
3. **Environment-specific settings** - Override base settings in environment-specific files
4. **Logging levels** - Use appropriate logging levels for each environment
5. **Health checks** - Configuration issues are reported through health check endpoints