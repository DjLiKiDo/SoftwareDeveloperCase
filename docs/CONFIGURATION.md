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

### JwtSettings
- **Section**: `Jwt`
- **Class**: `SoftwareDeveloperCase.Application.Models.JwtSettings`
- **Purpose**: JWT token authentication configuration including signing keys, expiration times, and issuer details

## Environment Variables for Sensitive Data

Sensitive configuration values should be provided through environment variables:

### Database Configuration
- `DATABASE_CONNECTION_STRING` - Override database connection string

### Email Configuration
- `EMAIL_USERNAME` - SMTP authentication username
- `EMAIL_PASSWORD` - SMTP authentication password
- `EMAIL_FROM_ADDRESS` - Override sender email address

### JWT Configuration
- `JWT_SECRET` - JWT signing secret key (minimum 32 characters for HMAC-SHA256)
- `JWT_ISSUER` - JWT token issuer identifier
- `JWT_AUDIENCE` - JWT token audience identifier
- `JWT_EXPIRATION_MINUTES` - Access token expiration time in minutes (default: 15)
- `JWT_REFRESH_EXPIRATION_DAYS` - Refresh token expiration time in days (default: 7)

## Configuration Validation

Configuration is validated using the IValidateOptions pattern:
- `EmailSettingsValidator` - Validates email configuration
- `DatabaseSettingsValidator` - Validates database configuration
- `JwtSettingsValidator` - Validates JWT authentication configuration

Validation can be performed manually using extension methods:
```csharp
var emailValidation = emailSettings.ValidateEmailSettings();
var databaseValidation = databaseSettings.ValidateDatabaseSettings();
var jwtValidation = jwtSettings.ValidateJwtSettings();
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