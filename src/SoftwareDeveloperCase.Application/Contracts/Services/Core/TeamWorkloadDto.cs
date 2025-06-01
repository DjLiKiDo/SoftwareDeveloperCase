namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for team workload information
/// </summary>
public class TeamWorkloadDto
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
    /// The total number of members in the team
    /// </summary>
    public int TotalMembers { get; set; }

    /// <summary>
    /// The number of active projects assigned to the team
    /// </summary>
    public int ActiveProjects { get; set; }

    /// <summary>
    /// The total number of tasks assigned to the team
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// The number of completed tasks
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// The number of pending tasks (not yet completed)
    /// </summary>
    public int PendingTasks { get; set; }

    /// <summary>
    /// The average number of tasks assigned per team member
    /// </summary>
    public decimal AverageTasksPerMember { get; set; }

    /// <summary>
    /// The workload percentage (0-100) indicating capacity utilization
    /// </summary>
    public decimal WorkloadPercentage { get; set; }
}
