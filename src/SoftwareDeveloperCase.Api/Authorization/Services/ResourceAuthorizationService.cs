using Microsoft.AspNetCore.Authorization;
using SoftwareDeveloperCase.Api.Authorization.Requirements;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Api.Authorization.Services;

/// <summary>
/// Service for performing authorization checks on resources
/// </summary>
public interface IResourceAuthorizationService
{
    /// <summary>
    /// Authorizes team access for the current user
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="operation">The operation being performed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if authorized, false otherwise</returns>
    Task<bool> AuthorizeTeamAccessAsync(Guid teamId, string operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authorizes project access for the current user
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="operation">The operation being performed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if authorized, false otherwise</returns>
    Task<bool> AuthorizeProjectAccessAsync(Guid projectId, string operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authorizes task access for the current user
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="operation">The operation being performed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if authorized, false otherwise</returns>
    Task<bool> AuthorizeTaskAccessAsync(Guid taskId, string operation, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of resource authorization service
/// </summary>
public class ResourceAuthorizationService : IResourceAuthorizationService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ResourceAuthorizationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceAuthorizationService"/> class
    /// </summary>
    /// <param name="authorizationService">The authorization service</param>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="httpContextAccessor">The HTTP context accessor</param>
    /// <param name="logger">The logger</param>
    public ResourceAuthorizationService(
        IAuthorizationService authorizationService,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ResourceAuthorizationService> logger)
    {
        _authorizationService = authorizationService;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> AuthorizeTeamAccessAsync(Guid teamId, string operation, CancellationToken cancellationToken = default)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            _logger.LogWarning("No user context available for team authorization");
            return false;
        }

        try
        {
            var teams = await _unitOfWork.TeamRepository.GetAsync(t => t.Id == teamId, cancellationToken);
            var team = teams.FirstOrDefault();

            if (team == null)
            {
                _logger.LogWarning("Team not found: {TeamId}", teamId);
                return false;
            }

            var requirement = new TeamAccessRequirement(operation);
            var result = await _authorizationService.AuthorizeAsync(user, team, requirement);

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authorizing team access for team {TeamId} and operation {Operation}",
                teamId, operation);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> AuthorizeProjectAccessAsync(Guid projectId, string operation, CancellationToken cancellationToken = default)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            _logger.LogWarning("No user context available for project authorization");
            return false;
        }

        try
        {
            var projects = await _unitOfWork.ProjectRepository.GetAsync(p => p.Id == projectId, cancellationToken);
            var project = projects.FirstOrDefault();

            if (project == null)
            {
                _logger.LogWarning("Project not found: {ProjectId}", projectId);
                // For DELETE operations, if the resource doesn't exist, allow the operation to proceed
                // so the controller can return the appropriate 404 response
                if (operation == ProjectAccessRequirement.Operations.Delete)
                {
                    // Check if user has general delete permissions (Admin role)
                    var userRole = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                    if (Enum.TryParse<Domain.Enums.Identity.SystemRole>(userRole, out var role) &&
                        role == Domain.Enums.Identity.SystemRole.Admin)
                    {
                        _logger.LogDebug("Admin user allowed to proceed with DELETE on non-existent project {ProjectId}", projectId);
                        return true;
                    }
                }
                return false;
            }

            var requirement = new ProjectAccessRequirement(operation);
            var result = await _authorizationService.AuthorizeAsync(user, project, requirement);

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authorizing project access for project {ProjectId} and operation {Operation}",
                projectId, operation);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> AuthorizeTaskAccessAsync(Guid taskId, string operation, CancellationToken cancellationToken = default)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            _logger.LogWarning("No user context available for task authorization");
            return false;
        }

        try
        {
            var tasks = await _unitOfWork.TaskRepository.GetAsync(t => t.Id == taskId, cancellationToken);
            var task = tasks.FirstOrDefault();

            if (task == null)
            {
                _logger.LogWarning("Task not found: {TaskId}", taskId);
                return false;
            }

            var requirement = new TaskAccessRequirement(operation);
            var result = await _authorizationService.AuthorizeAsync(user, task, requirement);

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authorizing task access for task {TaskId} and operation {Operation}",
                taskId, operation);
            return false;
        }
    }
}
