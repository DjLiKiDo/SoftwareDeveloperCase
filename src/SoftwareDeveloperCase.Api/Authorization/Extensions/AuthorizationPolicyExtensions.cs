using Microsoft.AspNetCore.Authorization;
using SoftwareDeveloperCase.Api.Authorization.Handlers;
using SoftwareDeveloperCase.Api.Authorization.Requirements;

namespace SoftwareDeveloperCase.Api.Authorization.Extensions;

/// <summary>
/// Extension methods for configuring authorization policies
/// </summary>
public static class AuthorizationPolicyExtensions
{
    /// <summary>
    /// Adds comprehensive authorization policies and handlers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddComprehensiveAuthorization(this IServiceCollection services)
    {
        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, TeamAccessHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectAccessHandler>();
        services.AddScoped<IAuthorizationHandler, TaskAccessHandler>();

        services.AddAuthorization(options =>
        {
            // Role-based policies
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("ManagerOrAdmin", policy =>
                policy.RequireRole("Manager", "Admin"));

            options.AddPolicy("DeveloperOrManager", policy =>
                policy.RequireRole("Developer", "Manager", "Admin"));

            // Resource-based policies for teams
            options.AddPolicy("TeamRead", policy =>
                policy.Requirements.Add(new TeamAccessRequirement(TeamAccessRequirement.Operations.Read)));

            options.AddPolicy("TeamCreate", policy =>
                policy.Requirements.Add(new TeamAccessRequirement(TeamAccessRequirement.Operations.Create)));

            options.AddPolicy("TeamUpdate", policy =>
                policy.Requirements.Add(new TeamAccessRequirement(TeamAccessRequirement.Operations.Update)));

            options.AddPolicy("TeamDelete", policy =>
                policy.Requirements.Add(new TeamAccessRequirement(TeamAccessRequirement.Operations.Delete)));

            options.AddPolicy("TeamManageMembers", policy =>
                policy.Requirements.Add(new TeamAccessRequirement(TeamAccessRequirement.Operations.ManageMembers)));

            // Resource-based policies for projects
            options.AddPolicy("ProjectRead", policy =>
                policy.Requirements.Add(new ProjectAccessRequirement(ProjectAccessRequirement.Operations.Read)));

            options.AddPolicy("ProjectCreate", policy =>
                policy.Requirements.Add(new ProjectAccessRequirement(ProjectAccessRequirement.Operations.Create)));

            options.AddPolicy("ProjectUpdate", policy =>
                policy.Requirements.Add(new ProjectAccessRequirement(ProjectAccessRequirement.Operations.Update)));

            options.AddPolicy("ProjectDelete", policy =>
                policy.Requirements.Add(new ProjectAccessRequirement(ProjectAccessRequirement.Operations.Delete)));

            options.AddPolicy("ProjectManageTasks", policy =>
                policy.Requirements.Add(new ProjectAccessRequirement(ProjectAccessRequirement.Operations.ManageTasks)));

            // Resource-based policies for tasks
            options.AddPolicy("TaskRead", policy =>
                policy.Requirements.Add(new TaskAccessRequirement(TaskAccessRequirement.Operations.Read)));

            options.AddPolicy("TaskCreate", policy =>
                policy.Requirements.Add(new TaskAccessRequirement(TaskAccessRequirement.Operations.Create)));

            options.AddPolicy("TaskUpdate", policy =>
                policy.Requirements.Add(new TaskAccessRequirement(TaskAccessRequirement.Operations.Update)));

            options.AddPolicy("TaskDelete", policy =>
                policy.Requirements.Add(new TaskAccessRequirement(TaskAccessRequirement.Operations.Delete)));

            options.AddPolicy("TaskAssign", policy =>
                policy.Requirements.Add(new TaskAccessRequirement(TaskAccessRequirement.Operations.Assign)));

            options.AddPolicy("TaskUpdateStatus", policy =>
                policy.Requirements.Add(new TaskAccessRequirement(TaskAccessRequirement.Operations.UpdateStatus)));

            options.AddPolicy("TaskAddComment", policy =>
                policy.Requirements.Add(new TaskAccessRequirement(TaskAccessRequirement.Operations.AddComment)));

            // Complex business rule policies
            options.AddPolicy("CanAccessOwnResources", policy =>
                policy.RequireAssertion(context =>
                {
                    var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    return !string.IsNullOrEmpty(userIdClaim);
                }));

            options.AddPolicy("TeamLeaderOrManager", policy =>
                policy.RequireAssertion(context =>
                {
                    var userRole = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                    return userRole == "Manager" || userRole == "Admin";
                    // Note: Team leader check would require additional context about the specific team
                }));
        });

        return services;
    }
}
