namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for project timeline analytics
/// </summary>
public class ProjectTimelineDto
{
    /// <summary>
    /// Unique identifier of the project
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Name of the project
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Project start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Actual project end date, if completed
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Estimated project completion date based on current progress
    /// </summary>
    public DateTime? EstimatedCompletionDate { get; set; }

    /// <summary>
    /// Total number of days planned for the project
    /// </summary>
    public int TotalDays { get; set; }

    /// <summary>
    /// Number of days elapsed since the project started
    /// </summary>
    public int DaysElapsed { get; set; }

    /// <summary>
    /// Number of days remaining until the planned completion
    /// </summary>
    public int DaysRemaining { get; set; }

    /// <summary>
    /// Percentage of project completion from 0 to 100
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Indicates whether the project is delayed from its original schedule
    /// </summary>
    public bool IsDelayed { get; set; }

    /// <summary>
    /// Number of days the project is delayed from original schedule
    /// </summary>
    public int DelayDays { get; set; }

    /// <summary>
    /// List of project milestones
    /// </summary>
    public List<ProjectMilestoneDto> Milestones { get; set; } = [];
}
