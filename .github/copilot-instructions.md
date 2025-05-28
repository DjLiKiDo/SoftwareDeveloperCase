# GitHub Copilot Project Instructions

Solution: **DotNetSkills**

## Tech Stack
- **Framework**: .NET 9
- **Language**: C# 14
- **Architecture**: Clean Architecture (Onion Architecture)
- **API Style**: RESTful with OpenAPI/Swagger
- **Database**: Entity Framework Core with SQL Server
- **Testing**: xUnit, FluentAssertions, Moq
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Logging**: Serilog with structured logging
- **Authentication**: JWT Bearer tokens
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

## Architecture Layers
1. **Domain**: Entities, Value Objects, Domain Events, Interfaces
2. **Application**: Use Cases, DTOs, Interfaces, Validators, Mappings
3. **Infrastructure**: Data Access, External Services, Identity
4. **API**: Controllers, Middleware, Filters, Program.cs

## Coding Standards
- Use file-scoped namespaces
- Prefer primary constructors for simple classes
- Use nullable reference types (`#nullable enable`)
- Follow Microsoft C# naming conventions
- One class per file, matching filename
- Use `IAsyncEnumerable` for streaming data
- Implement `CancellationToken` in async methods
- Use `ValueTask` for hot-path async operations

## Dependency Rules
- Domain layer has no dependencies
- Application depends only on Domain
- Infrastructure depends on Application and Domain
- API depends on all layers but only references Application directly
- Use interfaces for dependency inversion

## Testing Requirements
- Unit tests for Domain and Application layers (minimum 80% coverage)
- Integration tests for Infrastructure and API
- Use AAA pattern (Arrange, Act, Assert)
- Mock external dependencies
- Test naming: `MethodName_StateUnderTest_ExpectedBehavior`

## API Design
- Use action filters for cross-cutting concerns
- Implement global exception handling middleware
- Return consistent response formats using `Result<T>` pattern
- Use proper HTTP status codes
- Implement request/response logging
- Version APIs using URL path versioning

## Security
- Validate all inputs using FluentValidation
- Sanitize data before storage
- Use parameterized queries
- Implement rate limiting
- Enable CORS with specific origins
- Use HTTPS only
- Implement audit logging for sensitive operations

---

## Documentation Standards
- Maintain a CHANGELOG.md following Keep a Changelog format
- Document all notable changes from the user's perspective
- Update changelog immediately after task validation
- Use clear, concise language avoiding technical jargon

## Changelog Practices
- Add new entries to the Unreleased section
- Categorize changes: Added, Changed, Deprecated, Removed, Fixed, Security
- Include issue/PR references when applicable
- Move Unreleased entries to versioned sections upon release
