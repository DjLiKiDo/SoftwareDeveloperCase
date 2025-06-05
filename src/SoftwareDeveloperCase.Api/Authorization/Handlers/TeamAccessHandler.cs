using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SoftwareDeveloperCase.Api.Authorization.Requirements;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Enums.Core;
using SoftwareDeveloperCase.Domain.Enums.Identity;

namespace SoftwareDeveloperCase.Api.Authorization.Handlers;

/// <summary>
/// Authorization handler for team access requirements
/// </summary>
public class TeamAccessHandler : AuthorizationHandler<TeamAccessRequirement, Team>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TeamAccessHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TeamAccessHandler"/> class
    /// </summary>
    /// <param name="unitOfWork">The unit of work for data access</param>
    /// <param name="logger">The logger</param>
    public TeamAccessHandler(IUnitOfWork unitOfWork, ILogger<TeamAccessHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the authorization requirement for team access
    /// </summary>
    /// <param name="context">The authorization context</param>
    /// <param name="requirement">The team access requirement</param>
    /// <param name="resource">The team resource</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TeamAccessRequirement requirement,
        Team resource)
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
                _logger.LogDebug("Admin user {UserId} granted access to team {TeamId}", userId, resource.Id);
                context.Succeed(requirement);
                return;
            }

            // Check team membership for other operations
            var teamMembers = await _unitOfWork.TeamMemberRepository.GetAsync(
                tm => tm.TeamId == resource.Id && tm.UserId == userId, CancellationToken.None);

            var teamMember = teamMembers.FirstOrDefault();

            if (teamMember == null)
            {
                _logger.LogDebug("User {UserId} is not a member of team {TeamId}", userId, resource.Id);

                // Managers can read team information even if not a member
                if (userRole == SystemRole.Manager && requirement.Operation == TeamAccessRequirement.Operations.Read)
                {
                    context.Succeed(requirement);
                    return;
                }

                context.Fail();
                return;
            }

            // Check permissions based on operation and team role
            var hasPermission = requirement.Operation switch
            {
                TeamAccessRequirement.Operations.Read => true, // All team members can read
                TeamAccessRequirement.Operations.Create => userRole == SystemRole.Manager, // Only managers can create teams
                TeamAccessRequirement.Operations.Update =>
                    teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                TeamAccessRequirement.Operations.Delete =>
                    teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                TeamAccessRequirement.Operations.ManageMembers =>
                    teamMember.TeamRole == TeamRole.Leader || userRole == SystemRole.Manager,
                _ => false
            };

            if (hasPermission)
            {
                _logger.LogDebug("User {UserId} granted {Operation} access to team {TeamId}",
                    userId, requirement.Operation, resource.Id);
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogDebug("User {UserId} denied {Operation} access to team {TeamId}",
                    userId, requirement.Operation, resource.Id);
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking team access for user {UserId} and team {TeamId}",
                userId, resource.Id);
            context.Fail();
        }
    }
}
