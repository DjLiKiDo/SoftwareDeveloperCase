# Clean Architecture Instructions for GitHub Copilot

## Overview

This project follows Clean Architecture (Onion Architecture) principles with .NET 8 and C# 13. Ensure all code adheres to these patterns and dependency rules.

## Architecture Layers

### 1. Domain Layer (`SoftwareDeveloperCase.Domain`)

**NO DEPENDENCIES** - Pure business logic

```
Domain/
├── Entities/           # Core business entities
├── ValueObjects/       # Immutable value objects
├── Enums/             # Domain enumerations
├── Events/            # Domain events
├── Exceptions/        # Domain-specific exceptions
└── Interfaces/        # Domain contracts
```

**Patterns:**

- Rich domain models with behavior
- Domain events for cross-aggregate communication
- Value objects for immutable concepts
- Custom exceptions for domain rules violations

**Example Entity:**

```csharp
namespace SoftwareDeveloperCase.Domain.Entities;

public class Project : BaseEntity
{
    private readonly List<Task> _tasks = [];

    public Project(string name, string description, UserId ownerId)
    {
        Name = Guard.Against.NullOrEmpty(name);
        Description = description;
        OwnerId = ownerId;
        Status = ProjectStatus.Planning;
        CreatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public ProjectStatus Status { get; private set; }
    public UserId OwnerId { get; private set; }
    public IReadOnlyList<Task> Tasks => _tasks.AsReadOnly();

    public void AddTask(Task task)
    {
        if (Status == ProjectStatus.Completed)
            throw new DomainException("Cannot add tasks to completed project");

        _tasks.Add(task);
        AddDomainEvent(new TaskAddedEvent(Id, task.Id));
    }
}
```

### 2. Application Layer (`SoftwareDeveloperCase.Application`)

**Dependencies:** Domain only

```
Application/
├── Features/          # CQRS Commands/Queries by feature
│   ├── Projects/
│   ├── Tasks/
│   └── Users/
├── Common/
│   ├── Behaviors/     # MediatR pipeline behaviors
│   ├── Exceptions/    # Application exceptions
│   ├── Interfaces/    # Application contracts
│   ├── Mappings/      # AutoMapper profiles
│   └── Models/        # DTOs and view models
└── DependencyInjection.cs
```

**Patterns:**

- CQRS with MediatR
- Command/Query handlers
- Pipeline behaviors for cross-cutting concerns
- DTOs for data transfer
- Validators using FluentValidation

**Example Command:**

```csharp
namespace SoftwareDeveloperCase.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    string Name,
    string Description,
    Guid OwnerId
) : IRequest<Result<ProjectDto>>;

public class CreateProjectCommandHandler(
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = new Project(
            request.Name,
            request.Description,
            new UserId(request.OwnerId));

        await projectRepository.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(mapper.Map<ProjectDto>(project));
    }
}
```

### 3. Infrastructure Layer (`SoftwareDeveloperCase.Infrastructure`)

**Dependencies:** Application, Domain

```
Infrastructure/
├── Data/
│   ├── Contexts/      # DbContext implementations
│   ├── Configurations/# Entity configurations
│   ├── Migrations/    # EF Core migrations
│   └── Repositories/  # Repository implementations
├── Identity/          # Authentication/Authorization
├── Services/          # External service implementations
└── DependencyInjection.cs
```

**Patterns:**

- Repository pattern with Unit of Work
- Entity Framework Core with configurations
- Identity management with JWT
- External service integrations

**Example Repository:**

```csharp
namespace SoftwareDeveloperCase.Infrastructure.Data.Repositories;

public class ProjectRepository(ApplicationDbContext context)
    : IProjectRepository
{
    public async Task<Project?> GetByIdAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        return await context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Project>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Projects.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Project>(items, totalCount, pageNumber, pageSize);
    }
}
```

### 4. API Layer (`SoftwareDeveloperCase.Api`)

**Dependencies:** Application (Infrastructure referenced for DI only)

```
Api/
├── Controllers/       # REST API controllers
├── Middleware/        # Custom middleware
├── Filters/          # Action filters
├── Extensions/       # Service collection extensions
└── Program.cs        # Application entry point
```

**Patterns:**

- RESTful API design
- Controller-based routing
- Global exception handling
- OpenAPI/Swagger documentation

## Dependency Rules

### ✅ Allowed Dependencies

- Domain → None
- Application → Domain
- Infrastructure → Application, Domain
- API → Application (Infrastructure only for DI registration)

### ❌ Forbidden Dependencies

- Domain → Any other layer
- Application → Infrastructure, API
- Infrastructure → API
- Circular dependencies between any layers

## Key Architectural Patterns

### 1. CQRS (Command Query Responsibility Segregation)

```csharp
// Commands modify state
public record CreateProjectCommand(...) : IRequest<Result<ProjectDto>>;

// Queries read data
public record GetProjectQuery(Guid Id) : IRequest<Result<ProjectDto>>;
```

### 2. Repository Pattern with Unit of Work

```csharp
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(ProjectId id, CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 3. Result Pattern

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    public static Result<T> Success(T value) => new(true, value, string.Empty);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

### 4. Domain Events

```csharp
public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

## Code Generation Guidelines

### When creating new features:

1. Start with Domain entities and value objects
2. Define Application interfaces
3. Create Commands/Queries with handlers
4. Implement Infrastructure repositories
5. Add API controllers
6. Write comprehensive tests

### When adding new entities:

1. Place in appropriate Domain folder
2. Inherit from BaseEntity if needed
3. Add to DbContext
4. Create entity configuration
5. Generate and apply migration
6. Create repository interface and implementation
7. Add DTOs and mappings

### When adding new use cases:

1. Create Command/Query in Application layer
2. Add validator using FluentValidation
3. Implement handler with business logic
4. Add mapping profiles if needed
5. Create controller action
6. Write unit and integration tests

## Testing Structure

```
Tests/
├── SoftwareDeveloperCase.Test.Unit/
│   ├── Domain/        # Domain entity tests
│   ├── Application/   # Handler and service tests
│   └── Common/        # Test utilities
└── SoftwareDeveloperCase.Test.Integration/
    ├── Api/          # Controller integration tests
    ├── Infrastructure/ # Repository tests
    └── Common/       # Test fixtures
```

## Performance Considerations

### Database Queries

- Use projections for read-only operations
- Implement proper indexing strategies
- Use `AsNoTracking()` for read-only queries
- Implement query caching where appropriate

### Memory Management

- Use `IAsyncEnumerable` for large datasets
- Implement proper disposal patterns
- Use value types for small immutable data
- Consider memory pooling for high-frequency operations

### Async Operations

- Always use `ConfigureAwait(false)` in libraries
- Implement `CancellationToken` support
- Use `ValueTask` for hot-path operations
- Avoid `async void` except for event handlers

## Security Patterns

### Authorization

```csharp
[Authorize(Roles = "Admin,Manager")]
[HttpPost]
public async Task<ActionResult<ProjectDto>> CreateProject(
    CreateProjectCommand command,
    CancellationToken cancellationToken)
{
    var result = await mediator.Send(command, cancellationToken);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
}
```

### Input Validation

```csharp
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
```

## Common Mistakes to Avoid

1. **Domain referencing other layers** - Keep Domain pure
2. **Application referencing Infrastructure** - Use interfaces only
3. **Mixing business logic in controllers** - Keep controllers thin
4. **Not using async/await properly** - Always implement cancellation tokens
5. **Forgetting to validate inputs** - Use FluentValidation consistently
6. **Not implementing proper error handling** - Use Result pattern
7. **Exposing entities directly** - Always use DTOs for API responses
8. **Not following naming conventions** - Use consistent naming patterns

## Success Metrics

- ✅ No dependency rule violations
- ✅ All async methods support cancellation
- ✅ 80%+ test coverage for Domain and Application
- ✅ Consistent error handling throughout
- ✅ Proper input validation on all endpoints
- ✅ Clean separation of concerns
- ✅ SOLID principles adherence
