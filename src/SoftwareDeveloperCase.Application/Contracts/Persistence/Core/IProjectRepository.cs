using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence.Core;

/// <summary>
/// Repository interface for Project entity operations
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
    // These methods are now already inherited from IRepository<Project>
    
    /// <summary>
    /// Gets projects by team ID
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of projects for the team</returns>
    Task<IReadOnlyList<Project>> GetProjectsByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by user ID (where user is assigned or team member)
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of projects the user has access to</returns>
    Task<IReadOnlyList<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by status
    /// </summary>
    /// <param name="status">The project status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of projects with the specified status</returns>
    Task<IReadOnlyList<Project>> GetProjectsByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets project with team and tasks included
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project with related data or null if not found</returns>
    Task<Project?> GetProjectWithDetailsAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if project name exists within a team
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="name">The project name</param>
    /// <param name="excludeId">ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists, false otherwise</returns>
    Task<bool> IsProjectNameExistsInTeamAsync(Guid teamId, string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active projects by team ID
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active projects</returns>
    Task<IReadOnlyList<Project>> GetActiveProjectsByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);
}
