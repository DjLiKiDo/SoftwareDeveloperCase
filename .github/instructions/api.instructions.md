# API Development Instructions

## SoftwareDeveloperCase - REST API Guidelines

### Context

This file provides specific instructions for developing REST API endpoints in the SoftwareDeveloperCase project, following Clean Architecture principles with .NET 8.

---

## Controller Development

### Base Controller Pattern

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class {EntityName}Controller : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<{EntityName}Controller> _logger;

    public {EntityName}Controller(IMediator mediator, ILogger<{EntityName}Controller> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
}
```

### RESTful Endpoint Patterns

#### GET Endpoints

- **List with pagination**: `GET /api/v1/entities?page=1&pageSize=10&sortBy=name&sortDirection=asc`
- **Get by ID**: `GET /api/v1/entities/{id}`
- **Nested resources**: `GET /api/v1/projects/{projectId}/tasks`

#### POST Endpoints

- **Create**: `POST /api/v1/entities`
- **Return**: `201 Created` with `Location` header
- **Body**: Use Command DTOs from Application layer

#### PUT/PATCH Endpoints

- **Full update**: `PUT /api/v1/entities/{id}`
- **Partial update**: `PATCH /api/v1/entities/{id}`
- **Return**: `204 No Content` or `200 OK` with updated resource

#### DELETE Endpoints

- **Soft delete**: `DELETE /api/v1/entities/{id}`
- **Return**: `204 No Content`

---

## Authorization Patterns

### Resource-Based Authorization

```csharp
[HttpGet("{id}")]
[ResourceAccess(ResourceType.Project, Permission.Read)]
public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
{
    var query = new GetProjectByIdQuery(id);
    var result = await _mediator.Send(query);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
}
```

### Role-Based Authorization

```csharp
[HttpPost]
[Authorize(Roles = "Admin,Manager")]
public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectCommand command)
```

---

## Response Patterns

### Success Responses

```csharp
// 200 OK - Get operations
return Ok(result.Value);

// 201 Created - Create operations
return CreatedAtAction(nameof(GetProject), new { id = result.Value.Id }, result.Value);

// 204 No Content - Update/Delete operations
return NoContent();
```

### Error Responses

```csharp
// Use Result<T> pattern from Application layer
var result = await _mediator.Send(command);
if (!result.IsSuccess)
{
    return result.Error.Type switch
    {
        ErrorType.NotFound => NotFound(result.Error.Message),
        ErrorType.Validation => BadRequest(result.Error.Message),
        ErrorType.Unauthorized => Forbid(),
        ErrorType.Conflict => Conflict(result.Error.Message),
        _ => StatusCode(500, "Internal server error")
    };
}
```

---

## Validation Patterns

### Model Validation

```csharp
[HttpPost]
public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectCommand command)
{
    // Validation handled by ModelValidationActionFilter
    // FluentValidation rules applied automatically

    var result = await _mediator.Send(command);
    return result.IsSuccess
        ? CreatedAtAction(nameof(GetProject), new { id = result.Value.Id }, result.Value)
        : BadRequest(result.Error);
}
```

---

## Pagination Implementation

### Query Parameters

```csharp
[HttpGet]
public async Task<ActionResult<PagedResult<ProjectDto>>> GetProjects(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? sortBy = null,
    [FromQuery] string sortDirection = "asc",
    [FromQuery] string? searchTerm = null)
{
    var query = new GetProjectsQuery(page, pageSize, sortBy, sortDirection, searchTerm);
    var result = await _mediator.Send(query);
    return Ok(result);
}
```

### Response Headers

```csharp
Response.Headers.Add("X-Total-Count", totalCount.ToString());
Response.Headers.Add("X-Page", page.ToString());
Response.Headers.Add("X-Page-Size", pageSize.ToString());
```

---

## OpenAPI/Swagger Documentation

### Controller Documentation

```csharp
/// <summary>
/// Creates a new project
/// </summary>
/// <param name="command">Project creation data</param>
/// <returns>Created project</returns>
/// <response code="201">Project created successfully</response>
/// <response code="400">Invalid request data</response>
/// <response code="401">Unauthorized</response>
[HttpPost]
[ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectCommand command)
```

---

## Error Handling

### Global Exception Handling

- Exceptions are handled by `GlobalExceptionHandlingMiddleware`
- Controllers should return `Result<T>` from Application layer
- Use appropriate HTTP status codes

### Logging

```csharp
[HttpPost]
public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectCommand command)
{
    _logger.LogInformation("Creating project with name: {ProjectName}", command.Name);

    var result = await _mediator.Send(command);

    if (result.IsSuccess)
    {
        _logger.LogInformation("Project created successfully with ID: {ProjectId}", result.Value.Id);
    }
    else
    {
        _logger.LogWarning("Failed to create project: {Error}", result.Error.Message);
    }

    return result.IsSuccess
        ? CreatedAtAction(nameof(GetProject), new { id = result.Value.Id }, result.Value)
        : BadRequest(result.Error);
}
```

---

## Security Considerations

### Input Sanitization

- All inputs are sanitized by `RequestSanitizationMiddleware`
- Use `[FromBody]`, `[FromQuery]`, `[FromRoute]` attributes explicitly
- Validate all route parameters

### Rate Limiting

- Applied via `RateLimitingMiddleware`
- Different limits for different endpoints
- Consider authenticated vs anonymous users

### CORS

- Configured in `Program.cs`
- Specific origins only in production
- Avoid wildcard origins

---

## Testing Guidelines

### Controller Tests

```csharp
[Fact]
public async Task CreateProject_WithValidData_ReturnsCreatedResult()
{
    // Arrange
    var command = new CreateProjectCommand("Test Project", "Description");
    var expectedResult = Result<ProjectDto>.Success(new ProjectDto { Id = Guid.NewGuid() });
    _mediator.Setup(m => m.Send(command, default)).ReturnsAsync(expectedResult);

    // Act
    var result = await _controller.CreateProject(command);

    // Assert
    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
    Assert.Equal(nameof(ProjectController.GetProject), createdResult.ActionName);
}
```

### Integration Tests

- Test complete HTTP request/response cycle
- Use `WebApplicationFactory<Program>`
- Include authentication scenarios
- Test error scenarios

---

## Common Patterns to Follow

1. **Always use async/await** for database operations
2. **Include CancellationToken** in method signatures
3. **Use IMediator** for all business logic
4. **Follow RESTful conventions** strictly
5. **Include comprehensive logging**
6. **Use Result<T> pattern** for error handling
7. **Apply proper authorization** on all endpoints
8. **Include XML documentation** for all public methods
9. **Use ActionFilters** for cross-cutting concerns
10. **Test all endpoints** with unit and integration tests
