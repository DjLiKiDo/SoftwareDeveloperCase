using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SoftwareDeveloperCase.Api.Authorization.Requirements;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Enums.Core;
using SoftwareDeveloperCase.Domain.Enums.Identity;

namespace SoftwareDeveloperCase.Api.Authorization.Handlers;

/// <summary>
/// Authorization handler for project access requirements
/// </summary>
public class ProjectAccessHandler : AuthorizationHandler<ProjectAccessRequirement, Project>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectAccessHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectAccessHandler"/> class
    /// </summary>
    /// <param name="unitOfWork">The unit of work for data access</param>
    /// <param name="logger">The logger</param>
    public ProjectAccessHandler(IUnitOfWork unitOfWork, ILogger<ProjectAccessHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the authorization requirement for project access
    /// </summary>
    /// <param name="context">The authorization context</param>
    /// <param name="requirement">The project access requirement</param>
    /// <param name="resource">The project resource</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectAccessRequirement requirement,
        Project resource)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRoleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Invalid user ID in claims: {UserId}", userIdClaim);
            context.Fail();
            return;
        }

        if (!Enum.TryParse<SystemRole>(userRoleClaim, out var userRole))
        {
            _logger.LogWarning("Invalid role in claims: {UserRole}", userRoleClaim);
            context.Fail();
            return;
        }

        try
        {
            // Admin users have full access
            if (userRole == SystemRole.Admin)
            {
                _logger.LogDebug("Admin user {UserId} granted access to project {ProjectId}", userId, resource.Id);
                context.Succeed(requirement);
                return;
            }

            // Check if user is a member of the team that owns the project
            var teamMembers = await _unitOfWork.TeamMemberRepository.GetAsync(
                tm => tm.TeamId == resource.TeamId && tm.UserId == userId, CancellationToken.None);

            var teamMember = teamMembers.FirstOrDefault();

            if (teamMember == null)
            {
                _logger.LogDebug("User {UserId} is not a member of team {TeamId} that owns project {ProjectId}",
                    userId, resource.TeamId, resource.Id);

                // Managers can read project information even if not a team member
                if (userRole == SystemRole.Manager && requirement.Operation == ProjectAccessRequirement.Operations.Read)
                {
                    context.Succeed(requirement);
                    return;
                }

                context.Fail();
                return;
            }

            // Check permissions based on operation, team role, and user role
            var hasPermission = requirement.Operation switch
            {
                ProjectAccessRequirement.Operations.Read => true, // All team members can read
                ProjectAccessRequirement.Operations.Create => userRole == SystemRole.Manager, // Only managers can create projects
                ProjectAccessRequirement.Operations.Update =>
                    teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                ProjectAccessRequirement.Operations.Delete =>
                    teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                ProjectAccessRequirement.Operations.ManageTasks =>
                    teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                _ => false
            };

            if (hasPermission)
            {
                _logger.LogDebug("User {UserId} granted {Operation} access to project {ProjectId}",
                    userId, requirement.Operation, resource.Id);
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogDebug("User {UserId} denied {Operation} access to project {ProjectId}",
                    userId, requirement.Operation, resource.Id);
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking project access for user {UserId} and project {ProjectId}",
                userId, resource.Id);
            context.Fail();
        }
    }
}
