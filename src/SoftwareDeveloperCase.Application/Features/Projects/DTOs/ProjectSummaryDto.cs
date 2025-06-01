using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Features.Projects.DTOs;

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
