# GitHub Copilot Project Instructions

Solution: **SoftwareDeveloperCase**

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

## Technical Debt Management
- Monitor code quality metrics continuously
- Document technical debt items in /docs/TECHNICAL_DEBT_REPORT.md
- Review and update technical debt quarterly
- Track debt reduction progress in sprint retrospectives
- Include debt items in backlog with proper prioritization
- Measure impact of debt reduction initiatives

---

# SoftwareDeveloperCase - GitHub Copilot Instructions

This is a .NET 9 Clean Architecture solution for managing users, roles, and permissions. Please follow these guidelines when contributing:

## Architecture Overview
- **Clean Architecture**: Domain → Application → Infrastructure → API
- **CQRS Pattern**: Using MediatR for commands and queries
- **Repository Pattern**: With Unit of Work
- **Dependency Injection**: Throughout all layers

## Code Standards

### Required Before Each Commit
- Run `dotnet format` to ensure consistent code formatting
- Run `dotnet test` to ensure all tests pass
- Update CHANGELOG.md for any user-facing changes

### Development Flow
- Build: `dotnet build`
- Test: `dotnet test`
- Run: `dotnet run --project SoftwareDeveloperCase.Api`

## Repository Structure
- `SoftwareDeveloperCase.Domain/`: Core business entities and interfaces
- `SoftwareDeveloperCase.Application/`: Use cases, DTOs, validators, mappings
- `SoftwareDeveloperCase.Infrastructure/`: Data access, external services
- `SoftwareDeveloperCase.Api/`: REST API controllers and configuration
- `SoftwareDeveloperCase.Test.Unit/`: Unit tests (needs implementation)

## Key Guidelines
1. Use file-scoped namespaces
2. Prefer primary constructors where appropriate
3. Follow Clean Architecture dependency rules strictly
4. Use FluentValidation for all input validation
5. Use AutoMapper for entity-DTO mappings
6. Implement proper error handling with custom exceptions
7. Add XML documentation for public APIs
8. Write unit tests for all business logic
9. Use async/await for all database operations
10. Implement CancellationToken support in async methods

## Testing Requirements
- Minimum 80% code coverage for Domain and Application layers
- Use xUnit for test framework
- Use FluentAssertions for test assertions
- Use Moq for mocking dependencies
- Follow AAA pattern (Arrange, Act, Assert)
- Test naming: MethodName_StateUnderTest_ExpectedBehavior

## Common Tasks
- To add a new feature: Create command/query in Application layer, implement handler, add controller action
- To add validation: Create validator class inheriting from AbstractValidator<T>
- To add mapping: Update MappingProfile with new mappings
- To add repository: Create interface in Application, implement in Infrastructure

## Database
- Entity Framework Core with SQL Server
- Migrations: `dotnet ef migrations add <name> -p SoftwareDeveloperCase.Infrastructure -s SoftwareDeveloperCase.Api`
- Update database: `dotnet ef database update -p SoftwareDeveloperCase.Infrastructure -s SoftwareDeveloperCase.Api`
