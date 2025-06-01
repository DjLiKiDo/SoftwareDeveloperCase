using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

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
