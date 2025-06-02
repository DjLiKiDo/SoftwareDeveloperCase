---
applyTo: "tests/**/*.cs"
---

# Testing Instructions

## File Type Application

**Applies to:** All test C# files (\*.cs)

- Unit test files (`SoftwareDeveloperCase.Test.Unit/**/*.cs`)
- Integration test files (`SoftwareDeveloperCase.Test.Integration/**/*.cs`)
- Test utilities and fixtures (`**/Common/**/*.cs`)
- Test builders and helpers (`**/Builders/**/*.cs`, `**/Helpers/**/*.cs`)

## SoftwareDeveloperCase - Testing Guidelines

### Context

This file provides specific instructions for writing tests in the SoftwareDeveloperCase project, following Clean Architecture principles with .NET 8, xUnit, FluentAssertions, and Moq.

---

## Testing Strategy

### Testing Pyramid

1. **Unit Tests (70%)** - Domain and Application layers
2. **Integration Tests (20%)** - Infrastructure and API layers
3. **End-to-End Tests (10%)** - Critical user journeys

### Minimum Coverage Requirements

- **Domain Layer**: 90% coverage
- **Application Layer**: 85% coverage
- **API Layer**: 70% coverage
- **Overall Project**: 80% coverage

---

## Test Naming Conventions

### Method Naming Pattern

```
MethodName_StateUnderTest_ExpectedBehavior
```

### Examples

```csharp
[Fact]
public void CreateProject_WithValidData_ReturnsSuccessResult()

[Fact]
public void CreateProject_WithNullName_ReturnsValidationError()

[Fact]
public void GetProject_WhenProjectNotExists_ReturnsNotFoundResult()

[Theory]
[InlineData("", "Name cannot be empty")]
[InlineData(null, "Name is required")]
public void CreateProject_WithInvalidName_ReturnsValidationError(string name, string expectedError)
```

---

## Unit Testing Patterns

### AAA Pattern (Arrange, Act, Assert)

```csharp
[Fact]
public void CreateProjectCommandHandler_WithValidData_ReturnsSuccessResult()
{
    // Arrange
    var projectRepository = new Mock<IProjectRepository>();
    var unitOfWork = new Mock<IUnitOfWork>();
    var mapper = new Mock<IMapper>();

    var command = new CreateProjectCommand("Test Project", "Description", DateTime.Now.AddDays(30));
    var handler = new CreateProjectCommandHandler(projectRepository.Object, unitOfWork.Object, mapper.Object);

    var expectedProject = new Project("Test Project", "Description", DateTime.Now.AddDays(30));
    mapper.Setup(m => m.Map<Project>(command)).Returns(expectedProject);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeNull();
    result.Value.Name.Should().Be("Test Project");

    projectRepository.Verify(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
    unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
}
```

---

## Domain Entity Testing

### Testing Value Objects

```csharp
public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@example.co.uk")]
    public void Email_WithValidFormat_ShouldCreateSuccessfully(string validEmail)
    {
        // Act
        var email = Email.Create(validEmail);

        // Assert
        email.IsSuccess.Should().BeTrue();
        email.Value.Value.Should().Be(validEmail);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    public void Email_WithInvalidFormat_ShouldReturnError(string invalidEmail)
    {
        // Act
        var email = Email.Create(invalidEmail);

        // Assert
        email.IsFailure.Should().BeTrue();
        email.Error.Should().Contain("Invalid email format");
    }
}
```

### Testing Entities

```csharp
public class ProjectTests
{
    [Fact]
    public void Project_Create_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = "Test Project";
        var description = "Test Description";
        var dueDate = DateTime.Now.AddDays(30);

        // Act
        var result = Project.Create(name, description, dueDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
        result.Value.DueDate.Should().Be(dueDate);
        result.Value.Status.Should().Be(ProjectStatus.Planning);
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Project_AddTask_WhenProjectIsActive_ShouldAddTask()
    {
        // Arrange
        var project = Project.Create("Test Project", "Description", DateTime.Now.AddDays(30)).Value;
        project.Start(); // Make project active
        var task = ProjectTask.Create("Test Task", "Task Description", Priority.Medium).Value;

        // Act
        var result = project.AddTask(task);

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.Tasks.Should().Contain(task);
        project.DomainEvents.Should().ContainSingle(e => e is TaskAddedToProjectEvent);
    }
}
```

---

## Application Layer Testing

### Command Handler Testing

```csharp
public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _projectRepository = new Mock<IProjectRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _currentUserService = new Mock<ICurrentUserService>();

        _handler = new CreateProjectCommandHandler(
            _projectRepository.Object,
            _unitOfWork.Object,
            _mapper.Object,
            _currentUserService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProject()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateProjectCommand("Test Project", "Description", DateTime.Now.AddDays(30));
        var project = Project.Create(command.Name, command.Description, command.DueDate).Value;

        _currentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mapper.Setup(m => m.Map<Project>(command)).Returns(project);
        _mapper.Setup(m => m.Map<ProjectDto>(project)).Returns(new ProjectDto { Id = project.Id });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        _projectRepository.Verify(r => r.AddAsync(project, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

### Query Handler Testing

```csharp
public class GetProjectByIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetProjectByIdQueryHandler _handler;

    public GetProjectByIdQueryHandlerTests()
    {
        _projectRepository = new Mock<IProjectRepository>();
        _mapper = new Mock<IMapper>();
        _handler = new GetProjectByIdQueryHandler(_projectRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task Handle_WithExistingProject_ShouldReturnProjectDto()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Description", DateTime.Now.AddDays(30)).Value;
        var projectDto = new ProjectDto { Id = projectId, Name = "Test Project" };

        _projectRepository.Setup(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(project);
        _mapper.Setup(m => m.Map<ProjectDto>(project)).Returns(projectDto);

        var query = new GetProjectByIdQuery(projectId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(projectDto);
    }

    [Fact]
    public async Task Handle_WithNonExistingProject_ShouldReturnNotFoundError()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _projectRepository.Setup(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Project?)null);

        var query = new GetProjectByIdQuery(projectId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Message.Should().Contain("Project not found");
    }
}
```

---

## Validation Testing

### FluentValidation Testing

```csharp
public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator;

    public CreateProjectCommandValidatorTests()
    {
        _validator = new CreateProjectCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateProjectCommand("Test Project", "Description", DateTime.Now.AddDays(30));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Name is required")]
    [InlineData(null, "Name is required")]
    [InlineData("A", "Name must be at least 2 characters")]
    public void Validate_WithInvalidName_ShouldFail(string name, string expectedError)
    {
        // Arrange
        var command = new CreateProjectCommand(name, "Description", DateTime.Now.AddDays(30));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedError));
    }
}
```

---

## Integration Testing

### API Integration Tests

```csharp
public class ProjectsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProjectsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateProject_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var command = new CreateProjectCommand("Integration Test Project", "Description", DateTime.Now.AddDays(30));
        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Add authentication header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetValidJwtToken());

        // Act
        var response = await _client.PostAsync("/api/v1/projects", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var responseContent = await response.Content.ReadAsStringAsync();
        var project = JsonSerializer.Deserialize<ProjectDto>(responseContent);
        project.Should().NotBeNull();
        project.Name.Should().Be("Integration Test Project");
    }

    private string GetValidJwtToken()
    {
        // Implementation to get valid JWT token for testing
        // This could use a test identity provider or mock JWT
    }
}
```

### Database Integration Tests

```csharp
public class ProjectRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public ProjectRepositoryIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_WithValidProject_ShouldPersistToDatabase()
    {
        // Arrange
        await using var context = _fixture.CreateContext();
        var repository = new ProjectRepository(context);
        var project = Project.Create("Test Project", "Description", DateTime.Now.AddDays(30)).Value;

        // Act
        await repository.AddAsync(project, CancellationToken.None);
        await context.SaveChangesAsync();

        // Assert
        var savedProject = await context.Projects.FindAsync(project.Id);
        savedProject.Should().NotBeNull();
        savedProject.Name.Should().Be("Test Project");
    }
}
```

---

## Test Data Builders

### Object Mother Pattern

```csharp
public static class ProjectMother
{
    public static Project Default() =>
        Project.Create("Default Project", "Default Description", DateTime.Now.AddDays(30)).Value;

    public static Project WithName(string name) =>
        Project.Create(name, "Default Description", DateTime.Now.AddDays(30)).Value;

    public static Project Completed() =>
        Project.Create("Completed Project", "Description", DateTime.Now.AddDays(30)).Value
               .Complete();

    public static Project WithTasks(int taskCount)
    {
        var project = Default();
        for (int i = 0; i < taskCount; i++)
        {
            var task = ProjectTask.Create($"Task {i + 1}", "Description", Priority.Medium).Value;
            project.AddTask(task);
        }
        return project;
    }
}
```

### Builder Pattern

```csharp
public class ProjectBuilder
{
    private string _name = "Default Project";
    private string _description = "Default Description";
    private DateTime _dueDate = DateTime.Now.AddDays(30);
    private ProjectStatus _status = ProjectStatus.Planning;

    public ProjectBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProjectBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ProjectBuilder WithDueDate(DateTime dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public ProjectBuilder WithStatus(ProjectStatus status)
    {
        _status = status;
        return this;
    }

    public Project Build()
    {
        var project = Project.Create(_name, _description, _dueDate).Value;
        // Set status through reflection or internal methods if needed
        return project;
    }
}
```

---

## Test Configuration

### Test Settings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SoftwareDeveloperCase_Test;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "JwtSettings": {
    "Key": "test-key-for-testing-purposes-only",
    "Issuer": "SoftwareDeveloperCase.Test",
    "Audience": "SoftwareDeveloperCase.Test",
    "ExpirationMinutes": 60
  }
}
```

---

## Performance Testing

### Benchmark Testing

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class ProjectServiceBenchmarks
{
    private IProjectService _projectService;

    [GlobalSetup]
    public void Setup()
    {
        // Setup service with in-memory database
    }

    [Benchmark]
    public async Task CreateProject_Performance()
    {
        var command = new CreateProjectCommand("Benchmark Project", "Description", DateTime.Now.AddDays(30));
        await _projectService.CreateAsync(command);
    }
}
```

---

## Best Practices

### General Guidelines

1. **One Assert Per Test** - Each test should verify one specific behavior
2. **Descriptive Test Names** - Test names should clearly describe what is being tested
3. **Arrange-Act-Assert** - Follow AAA pattern consistently
4. **Independent Tests** - Tests should not depend on each other
5. **Fast Tests** - Unit tests should run quickly (< 100ms each)

### FluentAssertions Usage

```csharp
// Good
result.Should().NotBeNull();
result.Value.Should().BeOfType<ProjectDto>();
result.Value.Name.Should().Be("Expected Name");
result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

// Collections
projects.Should().HaveCount(3);
projects.Should().OnlyContain(p => p.Status == ProjectStatus.Active);
projects.Should().BeInAscendingOrder(p => p.Name);
```

### Mock Verification

```csharp
// Verify method was called
_repository.Verify(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);

// Verify method was not called
_repository.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

// Verify with specific parameters
_repository.Verify(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()), Times.Once);
```

---

## Coverage Requirements

### Measuring Coverage

```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html
```

### Coverage Exclusions

- Auto-generated code
- Program.cs and Startup.cs
- Migration files
- DTOs and simple data classes
- Third-party integrations (mark with `[ExcludeFromCodeCoverage]`)

---

## Continuous Integration

### Test Execution in CI

```yaml
- name: Run Unit Tests
  run: dotnet test tests/SoftwareDeveloperCase.Test.Unit/ --configuration Release --no-build --verbosity normal

- name: Run Integration Tests
  run: dotnet test tests/SoftwareDeveloperCase.Test.Integration/ --configuration Release --no-build --verbosity normal
```

### Quality Gates

- All tests must pass
- Minimum 80% code coverage
- No critical or major SonarQube issues
- All FluentValidation rules covered by tests
