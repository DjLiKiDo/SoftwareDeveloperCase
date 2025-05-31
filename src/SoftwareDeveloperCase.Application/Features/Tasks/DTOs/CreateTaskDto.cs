using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// DTO for creating a new task
/// </summary>
public class CreateTaskDto
{
    /// <summary>
    /// Task title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Task description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Project ID the task belongs to
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Assigned user ID
    /// </summary>
    public int? AssigneeId { get; set; }

    /// <summary>
    /// Task priority
    /// </summary>
    public Priority Priority { get; set; } = Priority.Medium;

    /// <summary>
    /// Parent task ID (for subtasks)
    /// </summary>
    public int? ParentTaskId { get; set; }

    /// <summary>
    /// Due date
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Estimated hours to complete
    /// </summary>
    public decimal? EstimatedHours { get; set; }
}
