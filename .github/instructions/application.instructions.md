---
applyTo: "src/SoftwareDeveloperCase.Application/**/*.cs"
---

# Application Layer Instructions

## File Type Application

**Applies to:** All Application layer C# files (\*.cs)

- Command/Query handlers (`Features/**/*Handler.cs`)
- Commands and queries (`Features/**/*Command.cs`, `Features/**/*Query.cs`)
- DTOs and view models (`DTOs/**/*.cs`, `Models/**/*.cs`)
- Validators (`Validation/**/*.cs`, `Features/**/*Validator.cs`)
- AutoMapper profiles (`Mappings/**/*.cs`)
- Application interfaces (`Contracts/**/*.cs`)
- Behaviors and pipeline components (`Behaviours/**/*.cs`)
- Application exceptions (`Exceptions/**/*.cs`)
- Application services (`Services/**/*.cs`)

## SoftwareDeveloperCase - Application Layer Guidelines

### Context

This file provides specific instructions for developing the Application layer in the SoftwareDeveloperCase project, following Clean Architecture and CQRS principles with .NET 8.

---

## CQRS Implementation

### Commands (State Modification)

```csharp
namespace SoftwareDeveloperCase.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    string Name,
    string Description,
    Guid OwnerId) : IRequest<Result<ProjectDto>>;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.OwnerId)
            .NotEmpty();
    }
}

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

### Queries (Data Retrieval)

```csharp
namespace SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProject;

public record GetProjectQuery(Guid Id) : IRequest<Result<ProjectDto>>;

public class GetProjectQueryValidator : AbstractValidator<GetProjectQuery>
{
    public GetProjectQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class GetProjectQueryHandler(
    IProjectRepository projectRepository,
    IMapper mapper) : IRequestHandler<GetProjectQuery, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(
        GetProjectQuery request,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(
            new ProjectId(request.Id),
            cancellationToken);

        if (project is null)
        {
            return Result<ProjectDto>.Failure("Project not found");
        }

        return Result.Success(mapper.Map<ProjectDto>(project));
    }
}
```

## DTO Design Patterns

### Response DTOs

```csharp
namespace SoftwareDeveloperCase.Application.DTOs.Projects;

public class ProjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ProjectStatus Status { get; init; }
    public Guid OwnerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<TaskDto> Tasks { get; init; } = [];
}

public class TaskDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TaskStatus Status { get; init; }
    public Priority Priority { get; init; }
    public Guid? AssigneeId { get; init; }
    public DateTime? DueDate { get; init; }
}
```

### Paginated Response DTOs

```csharp
namespace SoftwareDeveloperCase.Application.Models;

public class PagedResult<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public record PagedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false) : IRequest<Result<PagedResult<object>>>;
```

## Validation Patterns

### FluentValidation Best Practices

```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-zA-Z\\s]+$")
            .WithMessage("First name can only contain letters and spaces");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-zA-Z\\s]+$");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit and one special character");
    }
}
```

### Custom Validators

```csharp
public class ProjectExistsValidator : AbstractValidator<Guid>
{
    private readonly IProjectRepository _projectRepository;

    public ProjectExistsValidator(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;

        RuleFor(id => id)
            .MustAsync(ProjectExists)
            .WithMessage("Project with specified ID does not exist");
    }

    private async Task<bool> ProjectExists(Guid projectId, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(
            new ProjectId(projectId),
            cancellationToken);
        return project is not null;
    }
}
```

## AutoMapper Configuration

### Mapping Profiles

```csharp
namespace SoftwareDeveloperCase.Application.Mappings;

public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId.Value));

        CreateMap<Domain.Task, TaskDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.AssigneeId, opt => opt.MapFrom(src => src.AssigneeId.Value));

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
    }
}
```

## Pipeline Behaviors

### Validation Behavior

```csharp
namespace SoftwareDeveloperCase.Application.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

### Performance Logging Behavior

```csharp
public class PerformanceBehaviour<TRequest, TResponse>(
    ILogger<PerformanceBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;

            logger.LogWarning(
                "Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                requestName,
                elapsedMilliseconds,
                request);
        }

        return response;
    }
}
```

## Exception Handling

### Application Exceptions

```csharp
namespace SoftwareDeveloperCase.Application.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Access to this resource is forbidden.")
    {
    }
}
```

## Result Pattern Implementation

### Result Base Class

```csharp
namespace SoftwareDeveloperCase.Application.Models;

public class Result
{
    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

public class Result<T> : Result
{
    private readonly T? _value;

    protected internal Result(T? value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of failed result");
}
```

## Dependency Injection Registration

### Application Services Registration

```csharp
namespace SoftwareDeveloperCase.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
```

## File Organization Guidelines

### Feature-Based Structure

```
Features/
├── Projects/
│   ├── Commands/
│   │   ├── CreateProject/
│   │   │   ├── CreateProjectCommand.cs
│   │   │   ├── CreateProjectCommandHandler.cs
│   │   │   └── CreateProjectCommandValidator.cs
│   │   └── UpdateProject/
│   └── Queries/
│       ├── GetProject/
│       └── GetProjects/
├── Tasks/
└── Users/
```

### Common Folder Structure

```
Common/
├── Behaviours/
├── Exceptions/
├── Interfaces/
├── Mappings/
└── Models/
```

## Testing Guidelines for Application Layer

### Handler Testing

```csharp
[Fact]
public async Task Handle_ValidRequest_ReturnsSuccessResult()
{
    // Arrange
    var command = new CreateProjectCommand("Test Project", "Description", Guid.NewGuid());
    var project = new Project(command.Name, command.Description, new UserId(command.OwnerId));

    _projectRepository.Setup(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);
    _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(1);
    _mapper.Setup(x => x.Map<ProjectDto>(It.IsAny<Project>()))
        .Returns(new ProjectDto { Id = project.Id.Value });

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeNull();
    _projectRepository.Verify(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

## Performance Considerations

### Async Best Practices

- Always use `ConfigureAwait(false)` in library code
- Implement `CancellationToken` support in all async operations
- Use `ValueTask<T>` for frequently called methods
- Avoid `async void` except for event handlers

### Memory Management

- Use `IAsyncEnumerable<T>` for streaming large datasets
- Implement proper disposal in handlers that use disposable resources
- Consider object pooling for frequently created objects

### Query Optimization

- Use projections in queries to avoid loading unnecessary data
- Implement proper caching strategies for frequently accessed data
- Use `AsNoTracking()` for read-only operations

## Code Quality Standards

### Handler Requirements

- ✅ Implement proper error handling with Result pattern
- ✅ Add comprehensive input validation
- ✅ Support cancellation tokens
- ✅ Use dependency injection correctly
- ✅ Follow single responsibility principle
- ✅ Add XML documentation for public APIs
- ✅ Write unit tests with 80%+ coverage

### Validation Requirements

- ✅ Validate all required fields
- ✅ Implement business rule validation
- ✅ Use appropriate validation messages
- ✅ Support localization where needed
- ✅ Test validation rules thoroughly

### Mapping Requirements

- ✅ Handle null values appropriately
- ✅ Map value objects correctly
- ✅ Avoid complex logic in mapping profiles
- ✅ Test mappings thoroughly
- ✅ Use conditional mapping when needed
