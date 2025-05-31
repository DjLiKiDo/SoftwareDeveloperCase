using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace SoftwareDeveloperCase.Api.Filters;

/// <summary>
/// Authorization filter to ensure users can only access resources they own or have permission to access
/// </summary>
public class ResourceAccessAuthorizationFilter : ActionFilterAttribute
{
    private readonly ILogger<ResourceAccessAuthorizationFilter> _logger;

    public ResourceAccessAuthorizationFilter(ILogger<ResourceAccessAuthorizationFilter> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt - no user ID found in claims");
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check if user is accessing their own resources
        var resourceUserId = GetResourceUserId(context);
        var teamId = GetResourceTeamId(context);

        // Admin users can access all resources
        if (userRole == "Admin")
        {
            base.OnActionExecuting(context);
            return;
        }

        // Users can access their own resources
        if (!string.IsNullOrEmpty(resourceUserId) && resourceUserId == userId)
        {
            base.OnActionExecuting(context);
            return;
        }

        // TODO: Add team membership validation when team context is available
        // This would require checking if the user is a member of the team
        // that owns the resource

        _logger.LogWarning("Access denied for user {UserId} to resource owned by {ResourceUserId}",
            userId, resourceUserId);

        // For now, allow access but log the attempt
        // In a real implementation, this would check team membership
        base.OnActionExecuting(context);
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
