---
applyTo: "src/SoftwareDeveloperCase.Domain/**/*.cs"
---

# Domain Layer Instructions

## File Type Application

**Applies to:** All Domain layer C# files (\*.cs)

- Entity classes (`Entities/**/*.cs`)
- Value Object classes (`ValueObjects/**/*.cs`)
- Domain events (`Events/**/*.cs`)
- Enumerations (`Enums/**/*.cs`)
- Domain interfaces (`Services/**/*.cs`)
- Domain exceptions (`Exceptions/**/*.cs`)
- Common base classes (`Common/**/*.cs`)

## SoftwareDeveloperCase - Domain Layer Guidelines

### Context

This file provides specific instructions for developing the Domain layer in the SoftwareDeveloperCase project, following Domain-Driven Design (DDD) and Clean Architecture principles with .NET 8.

---

## Domain Entity Patterns

### Base Entity

```csharp
namespace SoftwareDeveloperCase.Domain.Common;

public abstract class BaseEntity<TId> where TId : class
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected BaseEntity(TId id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    public TId Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### Aggregate Root Example

```csharp
namespace SoftwareDeveloperCase.Domain.Entities;

public class Project : BaseEntity<ProjectId>
{
    private readonly List<Task> _tasks = [];
    private readonly List<TeamMember> _teamMembers = [];

    private Project(ProjectId id) : base(id) { }

    public Project(string name, string description, UserId ownerId)
        : base(new ProjectId(Guid.NewGuid()))
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Description = description ?? string.Empty;
        OwnerId = Guard.Against.Null(ownerId, nameof(ownerId));
        Status = ProjectStatus.Planning;

        AddDomainEvent(new ProjectCreatedEvent(Id, name, ownerId));
    }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ProjectStatus Status { get; private set; }
    public UserId OwnerId { get; private set; }
    public IReadOnlyList<Task> Tasks => _tasks.AsReadOnly();
    public IReadOnlyList<TeamMember> TeamMembers => _teamMembers.AsReadOnly();

    public void UpdateDetails(string name, string description)
    {
        var oldName = Name;

        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Description = description ?? string.Empty;
        SetUpdatedAt();

        if (oldName != name)
        {
            AddDomainEvent(new ProjectNameChangedEvent(Id, oldName, name));
        }
    }

    public void ChangeStatus(ProjectStatus newStatus)
    {
        if (Status == newStatus) return;

        ValidateStatusTransition(Status, newStatus);

        var oldStatus = Status;
        Status = newStatus;
        SetUpdatedAt();

        AddDomainEvent(new ProjectStatusChangedEvent(Id, oldStatus, newStatus));
    }

    public void AddTask(string title, string description, Priority priority)
    {
        if (Status == ProjectStatus.Completed)
            throw new DomainException("Cannot add tasks to completed project");

        var task = new Task(title, description, Id, priority);
        _tasks.Add(task);
        SetUpdatedAt();

        AddDomainEvent(new TaskAddedToProjectEvent(Id, task.Id));
    }

    public void AssignTeamMember(UserId userId, TeamRole role)
    {
        if (_teamMembers.Any(tm => tm.UserId == userId))
            throw new DomainException("User is already a team member");

        var teamMember = new TeamMember(userId, Id, role);
        _teamMembers.Add(teamMember);
        SetUpdatedAt();

        AddDomainEvent(new TeamMemberAssignedEvent(Id, userId, role));
    }

    private static void ValidateStatusTransition(ProjectStatus from, ProjectStatus to)
    {
        var allowedTransitions = new Dictionary<ProjectStatus, ProjectStatus[]>
        {
            { ProjectStatus.Planning, [ProjectStatus.Active, ProjectStatus.Cancelled] },
            { ProjectStatus.Active, [ProjectStatus.OnHold, ProjectStatus.Completed, ProjectStatus.Cancelled] },
            { ProjectStatus.OnHold, [ProjectStatus.Active, ProjectStatus.Cancelled] },
            { ProjectStatus.Completed, [] },
            { ProjectStatus.Cancelled, [] }
        };

        if (!allowedTransitions[from].Contains(to))
        {
            throw new DomainException($"Cannot transition project from {from} to {to}");
        }
    }
}
```

## Value Objects

### Strongly Typed IDs

```csharp
namespace SoftwareDeveloperCase.Domain.ValueObjects;

public record ProjectId(Guid Value)
{
    public static implicit operator Guid(ProjectId id) => id.Value;
    public static implicit operator ProjectId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}

public record UserId(Guid Value)
{
    public static implicit operator Guid(UserId id) => id.Value;
    public static implicit operator UserId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}

public record TaskId(Guid Value)
{
    public static implicit operator Guid(TaskId id) => id.Value;
    public static implicit operator TaskId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
```

### Complex Value Objects

```csharp
namespace SoftwareDeveloperCase.Domain.ValueObjects;

public record Email
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");

        if (!IsValidEmail(email))
            throw new DomainException("Invalid email format");

        return new Email(email.Trim().ToLowerInvariant());
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static implicit operator string(Email email) => email.Value;
    public override string ToString() => Value;
}

public record TimeSpent
{
    private TimeSpent(int hours, int minutes)
    {
        Hours = hours;
        Minutes = minutes;
    }

    public int Hours { get; }
    public int Minutes { get; }
    public int TotalMinutes => (Hours * 60) + Minutes;

    public static TimeSpent Create(int hours, int minutes)
    {
        if (hours < 0)
            throw new DomainException("Hours cannot be negative");

        if (minutes < 0 || minutes >= 60)
            throw new DomainException("Minutes must be between 0 and 59");

        return new TimeSpent(hours, minutes);
    }

    public static TimeSpent FromMinutes(int totalMinutes)
    {
        if (totalMinutes < 0)
            throw new DomainException("Total minutes cannot be negative");

        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;

        return new TimeSpent(hours, minutes);
    }

    public TimeSpent Add(TimeSpent other)
    {
        return FromMinutes(TotalMinutes + other.TotalMinutes);
    }

    public override string ToString() => $"{Hours}h {Minutes}m";
}
```

## Domain Events

### Event Base Class

```csharp
namespace SoftwareDeveloperCase.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}

public abstract record DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid EventId { get; }
}
```

### Specific Domain Events

```csharp
namespace SoftwareDeveloperCase.Domain.Events;

public record ProjectCreatedEvent(
    ProjectId ProjectId,
    string ProjectName,
    UserId OwnerId) : DomainEvent;

public record ProjectStatusChangedEvent(
    ProjectId ProjectId,
    ProjectStatus OldStatus,
    ProjectStatus NewStatus) : DomainEvent;

public record TaskAssignedEvent(
    TaskId TaskId,
    ProjectId ProjectId,
    UserId AssigneeId) : DomainEvent;

public record TaskStatusChangedEvent(
    TaskId TaskId,
    TaskStatus OldStatus,
    TaskStatus NewStatus,
    UserId? ChangedBy) : DomainEvent;

public record TeamMemberAssignedEvent(
    ProjectId ProjectId,
    UserId UserId,
    TeamRole Role) : DomainEvent;
```

## Domain Enumerations

### Smart Enums Pattern

```csharp
namespace SoftwareDeveloperCase.Domain.Enums;

public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>
    where TEnum : Enumeration<TEnum>
{
    private static readonly Dictionary<int, TEnum> Enumerations = CreateEnumerations();

    protected Enumeration(int value, string name)
    {
        Value = value;
        Name = name;
    }

    public int Value { get; protected init; }
    public string Name { get; protected init; } = string.Empty;

    public static TEnum? FromValue(int value)
    {
        return Enumerations.TryGetValue(value, out var enumeration) ? enumeration : null;
    }

    public static TEnum? FromName(string name)
    {
        return Enumerations.Values.SingleOrDefault(x => x.Name == name);
    }

    public static IEnumerable<TEnum> GetAll()
    {
        return Enumerations.Values;
    }

    public bool Equals(Enumeration<TEnum>? other)
    {
        if (other is null) return false;
        return GetType() == other.GetType() && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Enumeration<TEnum> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString() => Name;

    private static Dictionary<int, TEnum> CreateEnumerations()
    {
        var enumerationType = typeof(TEnum);

        var fieldsForType = enumerationType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => enumerationType.IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!);

        return fieldsForType.ToDictionary(x => x.Value);
    }
}

public class Priority : Enumeration<Priority>
{
    public static readonly Priority Low = new(1, nameof(Low));
    public static readonly Priority Medium = new(2, nameof(Medium));
    public static readonly Priority High = new(3, nameof(High));
    public static readonly Priority Critical = new(4, nameof(Critical));

    private Priority(int value, string name) : base(value, name) { }
}
```

### Standard Enums

```csharp
namespace SoftwareDeveloperCase.Domain.Enums;

public enum ProjectStatus
{
    Planning = 1,
    Active = 2,
    OnHold = 3,
    Completed = 4,
    Cancelled = 5
}

public enum TaskStatus
{
    Todo = 1,
    InProgress = 2,
    InReview = 3,
    Done = 4,
    Blocked = 5
}

public enum TeamRole
{
    Leader = 1,
    Member = 2
}

public enum UserRole
{
    Admin = 1,
    Manager = 2,
    Developer = 3
}
```

## Domain Services

### Interface Definition

```csharp
namespace SoftwareDeveloperCase.Domain.Services;

public interface IProjectDomainService
{
    Task<bool> CanUserAccessProjectAsync(UserId userId, ProjectId projectId, CancellationToken cancellationToken = default);
    Task<TimeSpent> CalculateProjectTimeSpentAsync(ProjectId projectId, CancellationToken cancellationToken = default);
    Task<bool> IsProjectNameUniqueAsync(string name, ProjectId? excludeProjectId = null, CancellationToken cancellationToken = default);
}

public interface ITaskDomainService
{
    Task<bool> CanAssignTaskToUserAsync(TaskId taskId, UserId userId, CancellationToken cancellationToken = default);
    Task<Priority> SuggestTaskPriorityAsync(TaskId taskId, CancellationToken cancellationToken = default);
}
```

## Domain Exceptions

### Base Domain Exception

```csharp
namespace SoftwareDeveloperCase.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class ProjectDomainException : DomainException
{
    public ProjectId ProjectId { get; }

    public ProjectDomainException(ProjectId projectId, string message) : base(message)
    {
        ProjectId = projectId;
    }
}

public class TaskDomainException : DomainException
{
    public TaskId TaskId { get; }

    public TaskDomainException(TaskId taskId, string message) : base(message)
    {
        TaskId = taskId;
    }
}

public class UserDomainException : DomainException
{
    public UserId UserId { get; }

    public UserDomainException(UserId userId, string message) : base(message)
    {
        UserId = userId;
    }
}
```

## Repository Interfaces (Domain Contracts)

### Generic Repository Interface

```csharp
namespace SoftwareDeveloperCase.Domain.Common;

public interface IRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : class
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
```

### Specific Repository Interfaces

```csharp
namespace SoftwareDeveloperCase.Domain.Entities;

public interface IProjectRepository : IRepository<Project, ProjectId>
{
    Task<Project?> GetWithTasksAsync(ProjectId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByOwnerAsync(UserId ownerId, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNameAsync(string name, ProjectId? excludeId = null, CancellationToken cancellationToken = default);
    Task<PagedResult<Project>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
}

public interface ITaskRepository : IRepository<Domain.Task, TaskId>
{
    Task<IEnumerable<Domain.Task>> GetByProjectAsync(ProjectId projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Task>> GetByAssigneeAsync(UserId assigneeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Task>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
}

public interface IUserRepository : IRepository<User, UserId>
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithEmailAsync(Email email, UserId? excludeId = null, CancellationToken cancellationToken = default);
}
```

## Domain Validation Patterns

### Guard Clauses

```csharp
namespace SoftwareDeveloperCase.Domain.Common;

public static class Guard
{
    public static T Against<T>(T value, Func<T, bool> predicate, string message)
    {
        if (predicate(value))
            throw new DomainException(message);
        return value;
    }

    public static string NullOrEmpty(string value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
            throw new DomainException($"{parameterName} cannot be null or empty");
        return value;
    }

    public static T Null<T>(T value, string parameterName) where T : class
    {
        if (value is null)
            throw new DomainException($"{parameterName} cannot be null");
        return value;
    }

    public static T NullableNull<T>(T? value, string parameterName) where T : struct
    {
        if (!value.HasValue)
            throw new DomainException($"{parameterName} cannot be null");
        return value.Value;
    }

    public static int NegativeOrZero(int value, string parameterName)
    {
        if (value <= 0)
            throw new DomainException($"{parameterName} must be greater than zero");
        return value;
    }

    public static DateTime FutureDate(DateTime value, string parameterName)
    {
        if (value <= DateTime.UtcNow)
            throw new DomainException($"{parameterName} must be in the future");
        return value;
    }
}
```

## Entity Relationship Patterns

### One-to-Many Relationships

```csharp
public class Project : BaseEntity<ProjectId>
{
    private readonly List<Task> _tasks = [];

    public IReadOnlyList<Task> Tasks => _tasks.AsReadOnly();

    public void AddTask(Task task)
    {
        Guard.Against.Null(task, nameof(task));

        if (task.ProjectId != Id)
            throw new DomainException("Task must belong to this project");

        _tasks.Add(task);
        AddDomainEvent(new TaskAddedToProjectEvent(Id, task.Id));
    }

    public void RemoveTask(TaskId taskId)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task is null)
            throw new DomainException("Task not found in project");

        _tasks.Remove(task);
        AddDomainEvent(new TaskRemovedFromProjectEvent(Id, taskId));
    }
}
```

### Many-to-Many Relationships

```csharp
public class User : BaseEntity<UserId>
{
    private readonly List<TeamMember> _teamMemberships = [];

    public IReadOnlyList<TeamMember> TeamMemberships => _teamMemberships.AsReadOnly();

    public void JoinTeam(ProjectId projectId, TeamRole role)
    {
        if (_teamMemberships.Any(tm => tm.ProjectId == projectId))
            throw new DomainException("User is already a member of this project team");

        var membership = new TeamMember(Id, projectId, role);
        _teamMemberships.Add(membership);

        AddDomainEvent(new UserJoinedTeamEvent(Id, projectId, role));
    }
}

public class TeamMember : BaseEntity<TeamMemberId>
{
    private TeamMember(TeamMemberId id) : base(id) { }

    public TeamMember(UserId userId, ProjectId projectId, TeamRole role)
        : base(new TeamMemberId(Guid.NewGuid()))
    {
        UserId = userId;
        ProjectId = projectId;
        Role = role;
        Status = MemberStatus.Active;
        JoinedAt = DateTime.UtcNow;
    }

    public UserId UserId { get; private set; }
    public ProjectId ProjectId { get; private set; }
    public TeamRole Role { get; private set; }
    public MemberStatus Status { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public void ChangeRole(TeamRole newRole)
    {
        if (Role == newRole) return;

        var oldRole = Role;
        Role = newRole;
        SetUpdatedAt();

        AddDomainEvent(new TeamMemberRoleChangedEvent(UserId, ProjectId, oldRole, newRole));
    }
}
```

## Domain Testing Guidelines

### Entity Testing

```csharp
[Fact]
public void CreateProject_WithValidData_ShouldCreateProjectSuccessfully()
{
    // Arrange
    var name = "Test Project";
    var description = "Test Description";
    var ownerId = new UserId(Guid.NewGuid());

    // Act
    var project = new Project(name, description, ownerId);

    // Assert
    project.Name.Should().Be(name);
    project.Description.Should().Be(description);
    project.OwnerId.Should().Be(ownerId);
    project.Status.Should().Be(ProjectStatus.Planning);
    project.DomainEvents.Should().ContainSingle()
        .Which.Should().BeOfType<ProjectCreatedEvent>();
}

[Fact]
public void AddTask_ToCompletedProject_ShouldThrowDomainException()
{
    // Arrange
    var project = new Project("Test", "Description", new UserId(Guid.NewGuid()));
    project.ChangeStatus(ProjectStatus.Completed);

    // Act & Assert
    var action = () => project.AddTask("Task", "Description", Priority.Medium);

    action.Should().Throw<DomainException>()
        .WithMessage("Cannot add tasks to completed project");
}
```

### Value Object Testing

```csharp
[Theory]
[InlineData("test@example.com")]
[InlineData("user.name@domain.co.uk")]
[InlineData("valid.email+tag@example.org")]
public void Create_WithValidEmail_ShouldCreateSuccessfully(string emailValue)
{
    // Act
    var email = Email.Create(emailValue);

    // Assert
    email.Value.Should().Be(emailValue.ToLowerInvariant());
}

[Theory]
[InlineData("")]
[InlineData("invalid")]
[InlineData("@example.com")]
[InlineData("test@")]
public void Create_WithInvalidEmail_ShouldThrowDomainException(string invalidEmail)
{
    // Act & Assert
    var action = () => Email.Create(invalidEmail);

    action.Should().Throw<DomainException>();
}
```

## Code Quality Standards

### Domain Layer Requirements

- ✅ NO dependencies on other layers
- ✅ Rich domain models with behavior
- ✅ Proper encapsulation with private setters
- ✅ Domain events for important state changes
- ✅ Guard clauses for input validation
- ✅ Value objects for primitive obsession
- ✅ Strongly typed IDs
- ✅ Comprehensive domain exception handling
- ✅ Clean API design with intention-revealing methods
- ✅ 90%+ test coverage for domain logic
