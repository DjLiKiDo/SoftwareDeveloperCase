namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for task workload information
/// </summary>
public class TaskWorkloadDto
{
    /// <summary>
    /// The unique identifier of the assignee
    /// </summary>
    public int AssigneeId { get; set; }

    /// <summary>
    /// The name of the assignee
    /// </summary>
    public string AssigneeName { get; set; } = string.Empty;

    /// <summary>
    /// The total number of tasks assigned to this assignee
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// The number of completed tasks
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// The number of tasks currently in progress
    /// </summary>
    public int InProgressTasks { get; set; }

    /// <summary>
    /// The number of tasks that are past their due date
    /// </summary>
    public int OverdueTasks { get; set; }

    /// <summary>
    /// The number of high priority tasks assigned
    /// </summary>
    public int HighPriorityTasks { get; set; }

    /// <summary>
    /// The total estimated hours for all assigned tasks
    /// </summary>
    public decimal TotalEstimatedHours { get; set; }

    /// <summary>
    /// The total actual hours spent on assigned tasks
    /// </summary>
    public decimal TotalActualHours { get; set; }

    /// <summary>
    /// The workload percentage (0-100) indicating capacity utilization
    /// </summary>
    public decimal WorkloadPercentage { get; set; }

    /// <summary>
    /// The ratio of actual hours to estimated hours
    /// </summary>
    public decimal EfficiencyRatio { get; set; }

    /// <summary>
    /// List of recently assigned or updated tasks
    /// </summary>
    public List<TaskServiceSummaryDto> RecentTasks { get; set; } = [];
}
