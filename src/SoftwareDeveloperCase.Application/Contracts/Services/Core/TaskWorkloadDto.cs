using SoftwareDeveloperCase.Domain.Enums.Core;
using TaskStatusEnum = SoftwareDeveloperCase.Domain.Enums.Core.TaskStatus;

namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for task workload information
/// </summary>
public class TaskWorkloadDto
{
    public int AssigneeId { get; set; }
    public string AssigneeName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int HighPriorityTasks { get; set; }
    public decimal TotalEstimatedHours { get; set; }
    public decimal TotalActualHours { get; set; }
    public decimal WorkloadPercentage { get; set; }
    public decimal EfficiencyRatio { get; set; }
    public List<TaskServiceSummaryDto> RecentTasks { get; set; } = [];
}
