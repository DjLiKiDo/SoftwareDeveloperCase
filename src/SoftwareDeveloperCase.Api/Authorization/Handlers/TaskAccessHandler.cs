using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SoftwareDeveloperCase.Api.Authorization.Requirements;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Enums.Core;
using SoftwareDeveloperCase.Domain.Enums.Identity;
using TaskEntity = SoftwareDeveloperCase.Domain.Entities.Task.Task;

namespace SoftwareDeveloperCase.Api.Authorization.Handlers;

/// <summary>
/// Authorization handler for task access requirements
/// </summary>
public class TaskAccessHandler : AuthorizationHandler<TaskAccessRequirement, TaskEntity>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TaskAccessHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskAccessHandler"/> class
    /// </summary>
    /// <param name="unitOfWork">The unit of work for data access</param>
    /// <param name="logger">The logger</param>
    public TaskAccessHandler(IUnitOfWork unitOfWork, ILogger<TaskAccessHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the authorization requirement for task access
    /// </summary>
    /// <param name="context">The authorization context</param>
    /// <param name="requirement">The task access requirement</param>
    /// <param name="resource">The task resource</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async System.Threading.Tasks.Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TaskAccessRequirement requirement,
        TaskEntity resource)
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
                _logger.LogDebug("Admin user {UserId} granted access to task {TaskId}", userId, resource.Id);
                context.Succeed(requirement);
                return;
            }

            // Get the project and team information for the task
            var projects = await _unitOfWork.ProjectRepository.GetAsync(p => p.Id == resource.ProjectId, CancellationToken.None);
            var project = projects.FirstOrDefault();

            if (project == null)
            {
                _logger.LogWarning("Project not found for task {TaskId}", resource.Id);
                context.Fail();
                return;
            }

            // Check if user is a member of the team that owns the project
            var teamMembers = await _unitOfWork.TeamMemberRepository.GetAsync(
                tm => tm.TeamId == project.TeamId && tm.UserId == userId, CancellationToken.None);

            var teamMember = teamMembers.FirstOrDefault();

            if (teamMember == null)
            {
                _logger.LogDebug("User {UserId} is not a member of team {TeamId} that owns task {TaskId}",
                    userId, project.TeamId, resource.Id);

                // Managers can read task information even if not a team member
                if (userRole == SystemRole.Manager && requirement.Operation == TaskAccessRequirement.Operations.Read)
                {
                    context.Succeed(requirement);
                    return;
                }

                context.Fail();
                return;
            }

            // Check if user is assigned to the task
            var isAssignedToTask = resource.AssignedToId == userId;

            // Check permissions based on operation, assignment, team role, and user role
            var hasPermission = requirement.Operation switch
            {
                TaskAccessRequirement.Operations.Read => true, // All team members can read
                TaskAccessRequirement.Operations.Create =>
                    teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                TaskAccessRequirement.Operations.Update =>
                    isAssignedToTask || teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                TaskAccessRequirement.Operations.Delete =>
                    teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                TaskAccessRequirement.Operations.Assign =>
                    teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                TaskAccessRequirement.Operations.UpdateStatus =>
                    isAssignedToTask || teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                TaskAccessRequirement.Operations.AddComment =>
                    true, // All team members can add comments
                _ => false
            };

            if (hasPermission)
            {
                _logger.LogDebug("User {UserId} granted {Operation} access to task {TaskId}",
                    userId, requirement.Operation, resource.Id);
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogDebug("User {UserId} denied {Operation} access to task {TaskId}",
                    userId, requirement.Operation, resource.Id);
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking task access for user {UserId} and task {TaskId}",
                userId, resource.Id);
            context.Fail();
        }
    }
}
