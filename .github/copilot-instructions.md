# GitHub Copilot Project Instructions

Solution: **SoftwareDeveloperCase**

## Tech Stack
- **Framework**: .NET 8
- **Language**: C# 13
- **Architecture**: Clean Architecture (Onion Architecture)
- **API Style**: RESTful with OpenAPI/Swagger
- **Database**: Entity Framework Core with SQL Server
- **Testing**: xUnit, FluentAssertions, Moq
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Logging**: Serilog with structured logging
- **Authentication**: JWT Bearer tokens with refresh token support
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

## Architecture Layers
1. **Domain**: Entities, Value Objects, Domain Events, Interfaces, Enums
2. **Application**: Use Cases, DTOs, Interfaces, Validators, Mappings
3. **Infrastructure**: Data Access, External Services, Identity, Caching
4. **API**: Controllers, Middleware, Filters, Program.cs

## Coding Standards
- Assess adherence to SOLID, DRY, KISS, YAGNI principles + Tell Don't Ask
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
- Version APIs using URL path versioning (/api/v1/)
- Support pagination for list endpoints

## Security
- Validate all inputs using FluentValidation
- Sanitize data before storage
- Use parameterized queries
- Implement rate limiting
- Enable CORS with specific origins
- Use HTTPS only
- Implement audit logging for sensitive operations
- JWT with refresh token support

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

This is a .NET 8 Clean Architecture solution for project management, including team collaboration, task tracking, and time management.

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
- Update API documentation if endpoints change

### Development Flow
- Build: `dotnet build`
- Test: `dotnet test`
- Run: `dotnet run --project SoftwareDeveloperCase.Api`

## Repository Structure
- `SoftwareDeveloperCase.Domain/`: Core business entities and interfaces
- `SoftwareDeveloperCase.Application/`: Use cases, DTOs, validators, mappings
- `SoftwareDeveloperCase.Infrastructure/`: Data access, external services
- `SoftwareDeveloperCase.Api/`: REST API controllers and configuration
- `SoftwareDeveloperCase.Test.Unit/`: Unit tests
- `SoftwareDeveloperCase.Test.Integration/`: Integration tests

## Domain Model Overview

### Core Entities
- **User**: System users with authentication and role assignment
- **Role**: User roles (Admin, Manager, Developer)
- **Team**: Groups of users working on projects
- **TeamMember**: Relationship between users and teams
- **Project**: Work containers with status and timeline
- **Task**: Work items with hierarchy, assignment, and time tracking
- **TaskComment**: Collaboration on tasks

### Key Enumerations
- **Role**: Admin, Manager, Developer
- **TeamRole**: Leader, Member
- **MemberStatus**: Active, Inactive, OnLeave
- **ProjectStatus**: Planning, Active, OnHold, Completed, Cancelled
- **TaskStatus**: Todo, InProgress, InReview, Done, Blocked
- **Priority**: Low, Medium, High, Critical

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
11. Support pagination for list endpoints
12. Implement proper authorization checks

## Testing Requirements
- Minimum 80% code coverage for Domain and Application layers
- Use xUnit for test framework
- Use FluentAssertions for test assertions
- Use Moq for mocking dependencies
- Follow AAA pattern (Arrange, Act, Assert)
- Test naming: MethodName_StateUnderTest_ExpectedBehavior
- Integration tests for all API endpoints

## Common Tasks
- To add a new feature: Create command/query in Application layer, implement handler, add controller action
- To add validation: Create validator class inheriting from AbstractValidator<T>
- To add mapping: Update MappingProfile with new mappings
- To add repository: Create interface in Application, implement in Infrastructure
- To add new entity: Create in Domain, add to DbContext, create migration

## Database
- Entity Framework Core with SQL Server
- Migrations: `dotnet ef migrations add <name> -p SoftwareDeveloperCase.Infrastructure -s SoftwareDeveloperCase.Api`
- Update database: `dotnet ef database update -p SoftwareDeveloperCase.Infrastructure -s SoftwareDeveloperCase.Api`

## API Conventions
- Use plural nouns for resource names
- Use HTTP verbs appropriately (GET, POST, PUT, PATCH, DELETE)
- Return 201 Created with Location header for resource creation
- Return 204 No Content for successful operations with no response body
- Use query parameters for filtering and pagination
- Use PATCH for partial updates
- Nest resources logically (e.g., /projects/{id}/tasks)
