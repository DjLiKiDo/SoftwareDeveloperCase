using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SoftwareDeveloperCase.Api.Authorization.Requirements;
using SoftwareDeveloperCase.Api.Authorization.Services;

namespace SoftwareDeveloperCase.Api.Filters;

/// <summary>
/// Enhanced authorization filter that integrates with the policy-based authorization system
/// </summary>
public class ResourceAccessAuthorizationFilter : ActionFilterAttribute
{
    private readonly ILogger<ResourceAccessAuthorizationFilter> _logger;
    private readonly IAuthorizationService _authorizationService;
    private readonly IResourceAuthorizationService _resourceAuthorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceAccessAuthorizationFilter"/> class
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="authorizationService">The authorization service</param>
    /// <param name="resourceAuthorizationService">The resource authorization service</param>
    public ResourceAccessAuthorizationFilter(
        ILogger<ResourceAccessAuthorizationFilter> logger,
        IAuthorizationService authorizationService,
        IResourceAuthorizationService resourceAuthorizationService)
    {
        _logger = logger;
        _authorizationService = authorizationService;
        _resourceAuthorizationService = resourceAuthorizationService;
    }

    /// <summary>
    /// Called before the action method is invoked to verify user authorization
    /// </summary>
    /// <param name="context">The action executing context</param>
    public override async void OnActionExecuting(ActionExecutingContext context)
    {
        // Skip authorization if the action has AllowAnonymous attribute
        var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

        if (hasAllowAnonymous)
        {
            base.OnActionExecuting(context);
            return;
        }

        var user = context.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt - no user ID found in claims");
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get resource identifiers from the request based on controller context
        var controllerName = context.RouteData.Values["controller"]?.ToString()?.ToLower();

        var teamId = GetResourceId(context, "teamId", "teamid");
        var projectId = controllerName == "projects" ? GetResourceId(context, "projectId", "projectid", "id") : GetResourceId(context, "projectId", "projectid");
        var taskId = controllerName == "tasks" ? GetResourceId(context, "taskId", "taskid", "id") : GetResourceId(context, "taskId", "taskid");

        try
        {
            // Check authorization based on resource type and operation
            var authorized = await CheckResourceAuthorizationAsync(context, teamId, projectId, taskId);

            if (!authorized)
            {
                _logger.LogWarning("Access denied for user {UserId} to resource. TeamId: {TeamId}, ProjectId: {ProjectId}, TaskId: {TaskId}",
                    userId, teamId, projectId, taskId);
                context.Result = new ForbidResult();
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during resource authorization for user {UserId}", userId);
            context.Result = new StatusCodeResult(500);
            return;
        }

        base.OnActionExecuting(context);
    }

    /// <summary>
    /// Checks resource authorization based on the type of resource and operation
    /// </summary>
    private async Task<bool> CheckResourceAuthorizationAsync(
        ActionExecutingContext context,
        Guid? teamId,
        Guid? projectId,
        Guid? taskId)
    {
        var httpMethod = context.HttpContext.Request.Method;
        var operation = GetOperationFromHttpMethod(httpMethod);
        var controllerName = context.RouteData.Values["controller"]?.ToString()?.ToLower();

        // Prioritize resource type based on controller context
        if (controllerName == "tasks" && taskId.HasValue)
        {
            return await _resourceAuthorizationService.AuthorizeTaskAccessAsync(taskId.Value, operation);
        }

        if (controllerName == "projects" && projectId.HasValue)
        {
            return await _resourceAuthorizationService.AuthorizeProjectAccessAsync(projectId.Value, operation);
        }

        if (controllerName == "teams" && teamId.HasValue)
        {
            return await _resourceAuthorizationService.AuthorizeTeamAccessAsync(teamId.Value, operation);
        }

        // Fallback to general resource checking if controller-specific logic doesn't apply
        if (taskId.HasValue)
        {
            return await _resourceAuthorizationService.AuthorizeTaskAccessAsync(taskId.Value, operation);
        }

        if (projectId.HasValue)
        {
            return await _resourceAuthorizationService.AuthorizeProjectAccessAsync(projectId.Value, operation);
        }

        if (teamId.HasValue)
        {
            return await _resourceAuthorizationService.AuthorizeTeamAccessAsync(teamId.Value, operation);
        }

        // If no specific resource ID found, check general authorization policies
        var user = context.HttpContext.User;
        var policyName = GetPolicyNameFromContext(context);

        if (!string.IsNullOrEmpty(policyName))
        {
            var authResult = await _authorizationService.AuthorizeAsync(user, policyName);
            return authResult.Succeeded;
        }

        // Default to allowing access if no specific authorization is required
        return true;
    }

    /// <summary>
    /// Gets the operation type from HTTP method
    /// </summary>
    private static string GetOperationFromHttpMethod(string httpMethod)
    {
        return httpMethod.ToUpper() switch
        {
            "GET" => TeamAccessRequirement.Operations.Read,
            "POST" => TeamAccessRequirement.Operations.Create,
            "PUT" => TeamAccessRequirement.Operations.Update,
            "PATCH" => TeamAccessRequirement.Operations.Update,
            "DELETE" => TeamAccessRequirement.Operations.Delete,
            _ => TeamAccessRequirement.Operations.Read
        };
    }

    /// <summary>
    /// Gets the policy name from the action context based on controller and action
    /// </summary>
    private static string? GetPolicyNameFromContext(ActionExecutingContext context)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();

        return (controllerName?.ToLower(), actionName?.ToLower()) switch
        {
            ("teams", "getteams") => "DeveloperOrManager",
            ("teams", "createteam") => "ManagerOrAdmin",
            ("projects", "getprojects") => "DeveloperOrManager",
            ("projects", "createproject") => "ManagerOrAdmin",
            ("tasks", "gettasks") => "DeveloperOrManager",
            ("tasks", "createtask") => "TaskCreate",
            _ => null
        };
    }

    /// <summary>
    /// Gets a resource ID from route parameters or action arguments
    /// </summary>
    private static Guid? GetResourceId(ActionExecutingContext context, params string[] parameterNames)
    {
        // Check route parameters first
        foreach (var paramName in parameterNames)
        {
            if (context.RouteData.Values.TryGetValue(paramName, out var routeValue))
            {
                if (Guid.TryParse(routeValue?.ToString(), out var routeGuid))
                {
                    return routeGuid;
                }
            }
        }

        // Check action arguments
        foreach (var paramName in parameterNames)
        {
            foreach (var parameter in context.ActionArguments)
            {
                if (parameter.Key.Equals(paramName, StringComparison.OrdinalIgnoreCase))
                {
                    if (parameter.Value is Guid guidValue)
                    {
                        return guidValue;
                    }
                    if (Guid.TryParse(parameter.Value?.ToString(), out var parsedGuid))
                    {
                        return parsedGuid;
                    }
                }
            }
        }

        return null;
    }

    private static string? GetResourceUserId(ActionExecutingContext context)
    {
        // Try to get user ID from route parameters
        if (context.RouteData.Values.TryGetValue("userId", out var userIdValue))
        {
            return userIdValue?.ToString();
        }

        // Try to get from action parameters
        foreach (var parameter in context.ActionArguments)
        {
            if (parameter.Key.ToLower().Contains("userid"))
            {
                return parameter.Value?.ToString();
            }
        }

        return null;
    }

    private static string? GetResourceTeamId(ActionExecutingContext context)
    {
        // Try to get team ID from route parameters
        if (context.RouteData.Values.TryGetValue("teamId", out var teamIdValue))
        {
            return teamIdValue?.ToString();
        }

        // Try to get from action parameters
        foreach (var parameter in context.ActionArguments)
        {
            if (parameter.Key.ToLower().Contains("teamid"))
            {
                return parameter.Value?.ToString();
            }
        }

        return null;
    }
}
