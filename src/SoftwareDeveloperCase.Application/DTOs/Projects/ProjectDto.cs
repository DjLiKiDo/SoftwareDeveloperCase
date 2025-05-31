using SoftwareDeveloperCase.Application.DTOs.Common;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.DTOs.Projects;

/// <summary>
/// DTO for Project entity
/// </summary>
public class ProjectDto : AuditableDto
{
    /// <summary>
    /// Project unique identifier
    /// </summary>
    public new int Id { get; set; }

    /// <summary>
    /// Project name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Project description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Team ID that owns the project
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// Team name
    /// </summary>
    public string TeamName { get; set; } = string.Empty;

    /// <summary>
    /// Project status
    /// </summary>
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// Project start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Estimated completion date
    /// </summary>
    public DateTime? EstimatedCompletionDate { get; set; }

    /// <summary>
    /// Actual completion date
    /// </summary>
    public DateTime? ActualCompletionDate { get; set; }

    /// <summary>
    /// Project progress percentage
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Number of tasks in the project
    /// </summary>
    public int TaskCount { get; set; }

    /// <summary>
    /// Number of completed tasks
    /// </summary>
    public int CompletedTaskCount { get; set; }

    /// <summary>
    /// Number of overdue tasks
    /// </summary>
    public int OverdueTaskCount { get; set; }
}

/// <summary>
/// DTO for creating a new project
/// </summary>
public class CreateProjectDto
{
    /// <summary>
    /// Project name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Project description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Team ID that will own the project
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// Project start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Estimated completion date
    /// </summary>
    public DateTime? EstimatedCompletionDate { get; set; }
}

/// <summary>
/// DTO for updating a project
/// </summary>
public class UpdateProjectDto
{
    /// <summary>
    /// Project ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Project name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Project description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Project status
    /// </summary>
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// Project start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Estimated completion date
    /// </summary>
    public DateTime? EstimatedCompletionDate { get; set; }

    /// <summary>
    /// Actual completion date
    /// </summary>
    public DateTime? ActualCompletionDate { get; set; }
}

/// <summary>
/// DTO for project summary view
/// </summary>
public class ProjectSummaryDto
{
    /// <summary>
    /// Project unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Project name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Team name
    /// </summary>
    public string TeamName { get; set; } = string.Empty;

    /// <summary>
    /// Project status
    /// </summary>
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// Project progress percentage
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Project start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Number of tasks
    /// </summary>
    public int TaskCount { get; set; }

    /// <summary>
    /// Number of completed tasks
    /// </summary>
    public int CompletedTaskCount { get; set; }
}
