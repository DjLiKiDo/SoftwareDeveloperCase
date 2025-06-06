# Authorization Documentation - SoftwareDeveloperCase API

## Overview

The SoftwareDeveloperCase API implements a comprehensive authorization system using ASP.NET Core's policy-based authorization combined with resource-based access control. This system ensures that users can only access and modify resources they have appropriate permissions for.

## Architecture

### Authorization Components

1. **ResourceAccessAuthorizationFilter** - Action filter that validates resource access
2. **ResourceAuthorizationService** - Core service for resource-based authorization logic
3. **Authorization Handlers** - Policy handlers for specific resource types
4. **Role-based Policies** - Pre-defined policies for user roles

### File Structure

```
src/SoftwareDeveloperCase.Api/
├── Authorization/
│   ├── Handlers/
│   │   └── ProjectAccessHandler.cs
│   ├── Requirements/
│   │   └── ResourceAccessRequirement.cs
│   └── Services/
│       └── ResourceAuthorizationService.cs
├── Filters/
│   └── ResourceAccessAuthorizationFilter.cs
└── Controllers/V1/
    ├── TeamsController.cs
    ├── ProjectsController.cs
    └── TasksController.cs
```

## User Roles and Permissions

### Role Hierarchy

| Role          | Level | Permissions                                       |
| ------------- | ----- | ------------------------------------------------- |
| **Admin**     | 3     | Full system access, can manage all resources      |
| **Manager**   | 2     | Can manage teams and projects they're assigned to |
| **Developer** | 1     | Can view and modify only assigned tasks           |

### Permission Matrix

| Resource     | Admin     | Manager                        | Developer                   |
| ------------ | --------- | ------------------------------ | --------------------------- |
| **Teams**    | Full CRUD | Create, Read, Update own teams | Read assigned teams         |
| **Projects** | Full CRUD | Full CRUD on assigned projects | Read assigned projects      |
| **Tasks**    | Full CRUD | Full CRUD on team tasks        | CRUD only on assigned tasks |
| **Users**    | Full CRUD | Read team members              | Read basic info             |

## Authorization Flow

### 1. Authentication

- User authenticates via JWT token
- Token contains user ID and role claims
- Token validated on each request

### 2. Role-Based Authorization

- Controller actions decorated with `[Authorize(Roles = "Admin,Manager")]`
- Basic role checking performed first

### 3. Resource-Based Authorization

- `ResourceAccessAuthorizationFilter` extracts resource ID from route
- `ResourceAuthorizationService` validates user's access to specific resource
- Authorization handlers evaluate complex business rules

### 4. Decision Logic

```csharp
// Simplified decision flow
if (user.IsAdmin())
    return Allow();

if (user.IsManager() && user.CanAccessResource(resourceId))
    return Allow();

if (user.IsDeveloper() && user.IsAssignedTo(resourceId))
    return Allow();

return Deny();
```

## Implementation Details

### ResourceAccessAuthorizationFilter

Handles authorization at the action level:

```csharp
public override async void OnActionExecuting(ActionExecutingContext context)
{
    // Extract resource ID from route parameters
    var resourceId = ExtractResourceId(context);
    var resourceType = DetermineResourceType(context);

    // Validate access using ResourceAuthorizationService
    var hasAccess = await _resourceAuthorizationService
        .AuthorizeAccessAsync(user, resourceType, resourceId, operation);

    if (!hasAccess)
    {
        context.Result = new ForbidResult();
        return;
    }
}
```

### ResourceAuthorizationService

Core authorization logic for different resource types:

```csharp
public async Task<bool> AuthorizeAccessAsync(
    ClaimsPrincipal user,
    string resourceType,
    int resourceId,
    string operation)
{
    return resourceType switch
    {
        "teams" => await AuthorizeTeamAccessAsync(user, resourceId, operation),
        "projects" => await AuthorizeProjectAccessAsync(user, resourceId, operation),
        "tasks" => await AuthorizeTaskAccessAsync(user, resourceId, operation),
        _ => false
    };
}
```

### Authorization Handlers

Implement complex business rules:

```csharp
public class ProjectAccessHandler : AuthorizationHandler<ResourceAccessRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceAccessRequirement requirement)
    {
        // Complex project access logic
        // Check team membership, project assignment, etc.
    }
}
```

## HTTP Status Codes

The authorization system returns appropriate HTTP status codes:

| Scenario          | Status Code | Description                                 |
| ----------------- | ----------- | ------------------------------------------- |
| Successful access | 200/201/204 | Operation completed successfully            |
| Unauthorized      | 401         | User not authenticated                      |
| Forbidden         | 403         | User authenticated but lacks permission     |
| Not Found         | 404         | Resource doesn't exist or user can't see it |

## Security Features

### 1. Defense in Depth

- Multiple authorization layers (role + resource-based)
- Action-level and controller-level protection
- Parameter validation and sanitization

### 2. Principle of Least Privilege

- Users granted minimum necessary permissions
- Resource-specific access control
- Operation-specific validation (read vs. write)

### 3. Audit Trail

- All authorization decisions logged
- Failed authorization attempts tracked
- User actions audited for compliance

### 4. Token Security

- JWT tokens with appropriate expiration
- Refresh token rotation
- Secure token storage practices

## Testing

### Integration Tests

The authorization system includes comprehensive integration tests:

```csharp
[Fact]
public async Task CreateTeam_AsAdmin_ShouldReturnCreated()
{
    // Test admin can create teams
}

[Fact]
public async Task DeleteTask_AsDeveloper_ShouldReturnForbidden()
{
    // Test developer cannot delete tasks
}

[Fact]
public async Task DeleteProject_AsAdmin_ShouldReturnOk()
{
    // Test admin can delete projects
}
```

**Test Coverage:** 20/20 authorization integration tests passing

### Test Categories

1. **Role-based tests** - Verify role permissions work correctly
2. **Resource-based tests** - Ensure users can only access assigned resources
3. **Cross-resource tests** - Validate complex scenarios (team → project → task)
4. **Edge cases** - Test non-existent resources, invalid permissions

## Configuration

### Program.cs Setup

```csharp
// Register authorization services
builder.Services.AddScoped<IResourceAuthorizationService, ResourceAuthorizationService>();
builder.Services.AddScoped<ResourceAccessAuthorizationFilter>();

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("RequireManagerRole", policy =>
        policy.RequireRole("Admin", "Manager"));
});

// Register authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, ProjectAccessHandler>();
```

### Controller Configuration

```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize] // Require authentication
[ServiceFilter(typeof(ResourceAccessAuthorizationFilter))] // Resource authorization
public class ProjectsController : ControllerBase
{
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")] // Role-based authorization
    public async Task<IActionResult> DeleteProject(int id)
    {
        // Action implementation
    }
}
```

## Best Practices

### 1. Consistent Authorization Patterns

- Apply authorization filters consistently across controllers
- Use role-based authorization for simple cases
- Implement resource-based authorization for complex scenarios

### 2. Error Handling

- Return appropriate HTTP status codes
- Avoid leaking information in error messages
- Log authorization failures for security monitoring

### 3. Performance Considerations

- Cache authorization decisions where appropriate
- Minimize database queries in authorization logic
- Use efficient user/role lookups

### 4. Maintainability

- Keep authorization logic centralized
- Document complex authorization rules
- Write comprehensive tests for all scenarios

## Troubleshooting

### Common Issues

1. **403 Forbidden on valid requests**

   - Check user roles in JWT token
   - Verify resource ownership/assignment
   - Review authorization filter logic

2. **401 Unauthorized**

   - Validate JWT token format and expiration
   - Check authentication middleware configuration
   - Verify token signing key

3. **Inconsistent authorization behavior**
   - Check authorization filter registration
   - Verify policy configuration in Program.cs
   - Review handler implementation logic

### Debugging Tips

1. Enable detailed authorization logging
2. Use browser developer tools to inspect JWT tokens
3. Check database for user-resource relationships
4. Review authorization test results for patterns

## Future Enhancements

### Planned Improvements

1. **Fine-grained permissions** - Implement permission-based authorization
2. **Dynamic policies** - Allow runtime policy configuration
3. **Audit improvements** - Enhanced logging and monitoring
4. **Performance optimization** - Caching and query optimization

### Extensibility Points

- Add new authorization handlers for custom resources
- Implement additional authentication providers
- Extend role hierarchy with custom roles
- Add resource-specific permission matrices

---

**Last Updated:** January 2025  
**Version:** 1.0  
**Status:** ✅ Implementation Complete
