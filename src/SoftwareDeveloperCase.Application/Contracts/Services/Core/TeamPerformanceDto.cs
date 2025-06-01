namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for team performance metrics
/// </summary>
public class TeamPerformanceDto
{
    /// <summary>
    /// The unique identifier of the team
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// The name of the team
    /// </summary>
    public string TeamName { get; set; } = string.Empty;

    /// <summary>
    /// The start date of the performance measurement period
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The end date of the performance measurement period
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// The number of tasks completed during the period
    /// </summary>
    public int TasksCompleted { get; set; }

    /// <summary>
    /// The number of tasks created during the period
    /// </summary>
    public int TasksCreated { get; set; }

    /// <summary>
    /// The ratio of completed tasks to created tasks (percentage)
    /// </summary>
    public decimal CompletionRate { get; set; }

    /// <summary>
    /// The average duration to complete a task in hours
    /// </summary>
    public decimal AverageTaskDuration { get; set; }

    /// <summary>
    /// The total hours worked by the team during the period
    /// </summary>
    public decimal TotalHoursWorked { get; set; }

    /// <summary>
    /// Productivity metric (tasks completed per hour)
    /// </summary>
    public decimal Productivity { get; set; }
}
