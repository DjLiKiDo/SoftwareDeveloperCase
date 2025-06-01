using SoftwareDeveloperCase.Domain.Enums.Core;
using TaskStatusEnum = SoftwareDeveloperCase.Domain.Enums.Core.TaskStatus;

namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for task summary information (used in task service)
/// </summary>
public class TaskServiceSummaryDto
{
    /// <summary>
    /// The unique identifier of the task
    /// </summary>
    public int TaskId { get; set; }
    
    /// <summary>
    /// The title of the task
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// The current status of the task
    /// </summary>
    public TaskStatusEnum Status { get; set; }
    
    /// <summary>
    /// The priority level of the task
    /// </summary>
    public Priority Priority { get; set; }
    
    /// <summary>
    /// The due date for task completion
    /// </summary>
    public DateTime? DueDate { get; set; }
    
    /// <summary>
    /// The name of the project this task belongs to
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;
    
    /// <summary>
    /// The estimated hours to complete the task
    /// </summary>
    public decimal? EstimatedHours { get; set; }
    
    /// <summary>
    /// The actual hours spent on the task
    /// </summary>
    public decimal? ActualHours { get; set; }
}
