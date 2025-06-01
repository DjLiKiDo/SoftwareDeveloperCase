using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for project health information
/// </summary>
public class ProjectHealthDto
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
    /// Current status of the project
    /// </summary>
    public ProjectStatus Status { get; set; }
    
    /// <summary>
    /// Percentage of project completion from 0 to 100
    /// </summary>
    public decimal ProgressPercentage { get; set; }
    
    /// <summary>
    /// Indicates whether the project is on schedule based on timeline
    /// </summary>
    public bool IsOnSchedule { get; set; }
    
    /// <summary>
    /// Indicates whether the project is on budget
    /// </summary>
    public bool IsOnBudget { get; set; }
    
    /// <summary>
    /// Count of tasks that are past their due date
    /// </summary>
    public int OverdueTasks { get; set; }
    
    /// <summary>
    /// Count of high priority tasks in the project
    /// </summary>
    public int HighPriorityTasks { get; set; }
    
    /// <summary>
    /// Count of tasks that are currently blocked
    /// </summary>
    public int BlockedTasks { get; set; }
    
    /// <summary>
    /// Team utilization percentage from 0 to 100
    /// </summary>
    public decimal TeamUtilization { get; set; }
    
    /// <summary>
    /// Overall health level assessment of the project
    /// </summary>
    public ProjectHealthLevel HealthLevel { get; set; }
    
    /// <summary>
    /// List of identified health issues affecting the project
    /// </summary>
    public List<string> HealthIssues { get; set; } = [];
    
    /// <summary>
    /// List of recommendations to improve project health
    /// </summary>
    public List<string> Recommendations { get; set; } = [];
}
