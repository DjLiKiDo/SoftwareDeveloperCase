using SoftwareDeveloperCase.Domain.Enums.Core;
using TaskStatusEnum = SoftwareDeveloperCase.Domain.Enums.Core.TaskStatus;

namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for task summary information (used in task service)
/// </summary>
public class TaskServiceSummaryDto
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskStatusEnum Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
}
