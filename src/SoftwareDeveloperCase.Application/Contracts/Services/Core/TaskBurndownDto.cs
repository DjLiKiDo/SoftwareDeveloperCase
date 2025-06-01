namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for task burndown chart data
/// </summary>
public class TaskBurndownDto
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
    /// Start date of the project or burndown period
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date of the project or burndown period
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Total number of tasks in the project
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Number of completed tasks in the project
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// List of data points for the burndown chart
    /// </summary>
    public List<BurndownDataPointDto> DataPoints { get; set; } = [];

    /// <summary>
    /// Team velocity in tasks completed per time period
    /// </summary>
    public decimal Velocity { get; set; }

    /// <summary>
    /// Projected completion date based on current velocity
    /// </summary>
    public DateTime? ProjectedCompletionDate { get; set; }
}
