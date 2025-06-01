using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// Service interface for project management operations
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Validates project business rules before creation or update
    /// </summary>
    /// <param name="project">The project to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if valid, false otherwise</returns>
    Task<bool> ValidateProjectAsync(Project project, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates project progress percentage
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Progress percentage (0-100)</returns>
    Task<decimal> CalculateProjectProgressAsync(int projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets project health status based on various metrics
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project health information</returns>
    Task<ProjectHealthDto> GetProjectHealthAsync(int projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if project can transition to new status
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="newStatus">The new status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if transition is valid, false otherwise</returns>
    Task<bool> CanTransitionToStatusAsync(int projectId, ProjectStatus newStatus, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets project timeline analytics
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project timeline information</returns>
    Task<ProjectTimelineDto> GetProjectTimelineAnalyticsAsync(int projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives completed project and related data
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if archived successfully, false otherwise</returns>
    Task<bool> ArchiveProjectAsync(int projectId, CancellationToken cancellationToken = default);
}
