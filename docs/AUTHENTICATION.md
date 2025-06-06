# JWT Authentication Implementation Guide

This document describes the JWT Bearer authentication system implemented in the SoftwareDeveloperCase API, including technical decisions, security considerations, and usage examples.

## Overview

The application implements a comprehensive JWT Bearer token authentication system with refresh token support, following OAuth2 best practices and Clean Architecture principles.

## Architecture Decisions

### Token Strategy
- **Access Tokens**: 15-minute expiration with HMAC-SHA256 signing
- **Refresh Tokens**: 7-day expiration with automatic rotation
- **Signing Algorithm**: HMAC-SHA256 for performance and security balance
- **Token Storage**: Refresh tokens stored in database with user relationships

**Rationale**: Short-lived access tokens minimize security exposure while refresh tokens provide seamless user experience. HMAC-SHA256 offers excellent performance with strong security guarantees.

### Domain Model Design
- `RefreshToken` entity with proper domain relationships
- User entity enhanced with refresh token collection
- Clean separation between authentication concerns and core domain

### Security Implementation
- **Input Sanitization Protection**: Passwords and tokens bypass automatic sanitization to prevent corruption
- **Role-Based Authorization**: Admin, Manager, Developer roles with proper claims
- **Token Validation**: Comprehensive validation including expiration, signature, and issuer verification
- **Secure Logout**: Immediate refresh token revocation

## API Endpoints

### POST /api/v1/auth/login
Authenticates user credentials and returns JWT tokens.

**Request**:
```json
{
  "email": "user@example.com",
  "password": "securePassword123"
}
```

**Response**:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "b8a9f5e3-d2c1-4b6a-8f7e-1c9d8e7f6a5b",
  "expiresAt": "2024-01-15T10:30:00Z",
  "user": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["Developer"]
  }
}
```

### POST /api/v1/auth/refresh
Exchanges refresh token for new access token with automatic refresh token rotation.

**Request**:
```json
{
  "refreshToken": "b8a9f5e3-d2c1-4b6a-8f7e-1c9d8e7f6a5b"
}
```

**Response**:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "c9b0f6e4-e3d2-5c7b-9f8e-2d0e9f8g7b6c",
  "expiresAt": "2024-01-15T11:00:00Z"
}
```

### POST /api/v1/auth/logout
Revokes all refresh tokens for the authenticated user.

**Headers**: `Authorization: Bearer {access_token}`

**Response**: `204 No Content`

### GET /api/v1/auth/me
Returns current authenticated user information.

**Headers**: `Authorization: Bearer {access_token}`

**Response**:
```json
{
  "id": 1,
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "roles": ["Developer"]
}
```

## JWT Token Structure

### Access Token Claims
```json
{
  "sub": "1",                           // User ID
  "email": "user@example.com",          // User email
  "given_name": "John",                 // First name
  "family_name": "Doe",                 // Last name
  "role": ["Developer"],                // User roles
  "jti": "unique-token-id",             // Token unique identifier
  "iat": 1642248600,                    // Issued at timestamp
  "exp": 1642249500,                    // Expiration timestamp
  "iss": "SoftwareDeveloperCase.Api",   // Issuer
  "aud": "SoftwareDeveloperCase.Client" // Audience
}
```

## Configuration

### Required Settings
```json
{
  "Jwt": {
    "Secret": "your-secret-key-minimum-32-characters-for-hmac-sha256-security",
    "Issuer": "SoftwareDeveloperCase.Api",
    "Audience": "SoftwareDeveloperCase.Client",
    "ExpirationMinutes": 15,
    "RefreshExpirationDays": 7
  }
}
```

### Environment Variables
- `JWT_SECRET`: Override JWT signing secret (required in production)
- `JWT_ISSUER`: Override token issuer
- `JWT_AUDIENCE`: Override token audience
- `JWT_EXPIRATION_MINUTES`: Override access token expiration
- `JWT_REFRESH_EXPIRATION_DAYS`: Override refresh token expiration

## Implementation Details

### Clean Architecture Compliance
- **Domain Layer**: `RefreshToken` entity with business rules
- **Application Layer**: Authentication commands/queries, `IJwtTokenService` contract
- **Infrastructure Layer**: `JwtTokenService` implementation, refresh token repository
- **API Layer**: `AuthController` with proper HTTP concerns

### Dependency Injection
```csharp
services.AddScoped<IJwtTokenService, JwtTokenService>();
services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
```

### Input Sanitization Integration
Authentication-specific fields are protected from automatic sanitization:
- Passwords: `[SkipSanitization]` to prevent corruption
- Tokens: `[SkipSanitization]` to maintain integrity
- Emails: Sanitized using `InputSanitizer.SanitizeEmail()`

### Error Handling
- `AuthenticationException`: Domain-specific authentication errors
- Structured error responses with appropriate HTTP status codes
- Secure error logging without exposing sensitive information

## Security Considerations

### Token Security
- **Secret Key**: Minimum 32 characters for HMAC-SHA256
- **Token Rotation**: Refresh tokens are rotated on each use
- **Expiration**: Short-lived access tokens minimize exposure window
- **Revocation**: Immediate refresh token revocation on logout

### Input Validation
- Email format validation before authentication processing
- Password complexity requirements (if implemented)
- Refresh token format and existence validation

### Logging Security
- User inputs sanitized before logging using `InputSanitizer.SanitizeForLogging()`
- Sensitive data (passwords, tokens) never logged
- Authentication attempts logged for security monitoring

## Testing Strategy

### Unit Tests Coverage
- **JWT Token Service**: Token generation, validation, claim extraction
- **Login Command Handler**: Authentication flow with various scenarios
- **Error Handling**: Invalid credentials, inactive users, expired tokens
- **Input Sanitization**: Protection of sensitive authentication data

### Test Examples
```csharp
[Fact]
public async Task GenerateAccessToken_ValidUser_ReturnsValidToken()
{
    // Arrange
    var user = CreateTestUser();
    
    // Act
    var token = await _jwtTokenService.GenerateAccessTokenAsync(user);
    
    // Assert
    token.Should().NotBeNullOrEmpty();
    var principal = _jwtTokenService.ValidateToken(token);
    principal.Identity.IsAuthenticated.Should().BeTrue();
}
```

## Usage Examples

### Client-Side Token Management
```javascript
// Login and store tokens
const loginResponse = await fetch('/api/v1/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, password })
});

const { accessToken, refreshToken, expiresAt } = await loginResponse.json();
localStorage.setItem('accessToken', accessToken);
localStorage.setItem('refreshToken', refreshToken);

// Automatic token refresh
async function makeAuthenticatedRequest(url, options = {}) {
  let token = localStorage.getItem('accessToken');
  
  // Check if token needs refresh
  if (isTokenExpired(token)) {
    token = await refreshAccessToken();
  }
  
  return fetch(url, {
    ...options,
    headers: {
      ...options.headers,
      'Authorization': `Bearer ${token}`
    }
  });
}
```

### Server-Side Authorization
```csharp
[Authorize(Roles = "Admin,Manager")]
[HttpPost("teams")]
public async Task<IActionResult> CreateTeam([FromBody] CreateTeamCommand command)
{
    var result = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetTeam), new { id = result.Id }, result);
}
```

## Troubleshooting

### Common Issues

1. **Token Invalid/Expired**
   - Verify token format and expiration
   - Check system clock synchronization
   - Ensure secret key matches between environments

2. **Refresh Token Not Found**
   - Token may have expired (7-day limit)
   - User may have logged out (tokens revoked)
   - Check database connectivity

3. **Authorization Failed**
   - Verify user has required roles
   - Check token claims and role assignments
   - Ensure proper `[Authorize]` attribute usage

### Debug Configuration
```json
{
  "Logging": {
    "LogLevel": {
      "SoftwareDeveloperCase.Infrastructure.Services.JwtTokenService": "Debug",
      "SoftwareDeveloperCase.Api.Controllers.V1.AuthController": "Debug"
    }
  }
}
```

This implementation provides a robust, secure, and maintainable authentication system that follows Clean Architecture principles while ensuring excellent security practices.