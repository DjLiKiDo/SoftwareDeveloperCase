# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Enhanced null safety checks in role and user validation logic
- Proper async/await patterns in FluentValidation validators
- Global exception handling middleware for consistent error responses
- Structured logging for exception handling with TraceId correlation
- In-memory database support for development and testing environments

### Changed
- Updated all project target frameworks from .NET 6.0 to .NET 8.0
- Converted synchronous role name validation to asynchronous pattern for better performance
- Improved null safety in user command handler for default role assignment
- Database configuration switched to in-memory database for improved testing

### Fixed
- Resolved compiler warnings CS8602 in EmailServiceTests by adding null-forgiving operators for test logging verification
- Eliminated all compiler warnings to ensure clean build output
- Made SoftwareDeveloperCaseDbContext and EntitySaveChangesInterceptor public for testing access

### Fixed
- Resolved compiler warnings for null reference dereferences in validation classes
- Fixed nullability mismatch in user email validation method signature
- Proper HTTP status code mapping for different exception types (ValidationException → 400, NotFoundException → 404)
- Corrected potential null reference access in role name validation logic

## [1.0.0] - 2024-01-01

### Added
- Initial release of SoftwareDeveloperCase API
- User management endpoints (CRUD operations)
- Role management endpoints (CRUD operations)
- Permission management system
- JWT authentication and authorization
- Clean Architecture implementation with Domain, Application, Infrastructure, and API layers
- Entity Framework Core integration with SQL Server
- FluentValidation for input validation
- AutoMapper for entity-DTO mappings
- Serilog structured logging
- OpenAPI/Swagger documentation
- Unit test project structure

[Unreleased]: https://github.com/yourusername/SoftwareDeveloperCase/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/yourusername/SoftwareDeveloperCase/releases/tag/v1.0.0
