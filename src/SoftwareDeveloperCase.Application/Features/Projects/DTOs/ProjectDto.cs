using SoftwareDeveloperCase.Application.DTOs.Common;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Features.Projects.DTOs;

/// <summary>
/// DTO for Project entity
/// </summary>
public class ProjectDto : AuditableDto
{
    /// <summary>
    /// Project unique identifier
    /// </summary>
    public new Guid Id { get; set; }

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
    public Guid TeamId { get; set; }

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
