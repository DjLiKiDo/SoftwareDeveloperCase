using SoftwareDeveloperCase.Domain.Enums.Core;
using TaskStatusEnum = SoftwareDeveloperCase.Domain.Enums.Core.TaskStatus;

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// DTO for updating a task
/// </summary>
public class UpdateTaskDto
{
    /// <summary>
    /// Task ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Task title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Task description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Assigned user ID
    /// </summary>
    public int? AssigneeId { get; set; }

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
    /// Start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Completion date
    /// </summary>
    public DateTime? CompletionDate { get; set; }

    /// <summary>
    /// Estimated hours to complete
    /// </summary>
    public decimal? EstimatedHours { get; set; }

    /// <summary>
    /// Actual hours spent
    /// </summary>
    public decimal? ActualHours { get; set; }
}
