using SoftwareDeveloperCase.Domain.Entities.Core;
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

/// <summary>
/// DTO for project health information
/// </summary>
public class ProjectHealthDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public bool IsOnSchedule { get; set; }
    public bool IsOnBudget { get; set; }
    public int OverdueTasks { get; set; }
    public int HighPriorityTasks { get; set; }
    public int BlockedTasks { get; set; }
    public decimal TeamUtilization { get; set; }
    public ProjectHealthLevel HealthLevel { get; set; }
    public List<string> HealthIssues { get; set; } = [];
    public List<string> Recommendations { get; set; } = [];
}

/// <summary>
/// DTO for project timeline analytics
/// </summary>
public class ProjectTimelineDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? EstimatedCompletionDate { get; set; }
    public int TotalDays { get; set; }
    public int DaysElapsed { get; set; }
    public int DaysRemaining { get; set; }
    public decimal ProgressPercentage { get; set; }
    public bool IsDelayed { get; set; }
    public int DelayDays { get; set; }
    public List<ProjectMilestoneDto> Milestones { get; set; } = [];
}

/// <summary>
/// DTO for project milestones
/// </summary>
public class ProjectMilestoneDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Enumeration for project health levels
/// </summary>
public enum ProjectHealthLevel
{
    Excellent,
    Good,
    Warning,
    Critical
}
