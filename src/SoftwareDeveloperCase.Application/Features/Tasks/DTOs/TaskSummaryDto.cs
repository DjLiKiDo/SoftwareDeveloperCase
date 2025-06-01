using SoftwareDeveloperCase.Domain.Enums.Core;
using TaskStatusEnum = SoftwareDeveloperCase.Domain.Enums.Core.TaskStatus;

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// DTO for task summary view
/// </summary>
public class TaskSummaryDto
{
    /// <summary>
    /// Task unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Task title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Project name
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Assigned user name
    /// </summary>
    public string? AssigneeName { get; set; }

    /// <summary>
    /// Task status
    /// </summary>
    public TaskStatusEnum Status { get; set; }

    /// <summary>
    /// Task priority
    /// </summary>
    public Priority Priority { get; set; }

    /// <summary>
    /// Due date
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Progress percentage
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Is task overdue
    /// </summary>
    public bool IsOverdue => DueDate.HasValue && DueDate < DateTime.UtcNow && Status != TaskStatusEnum.Done;
}
