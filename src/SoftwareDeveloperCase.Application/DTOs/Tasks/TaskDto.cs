using SoftwareDeveloperCase.Application.DTOs.Common;
using SoftwareDeveloperCase.Domain.Enums.Core;
using TaskStatusEnum = SoftwareDeveloperCase.Domain.Enums.Core.TaskStatus;

namespace SoftwareDeveloperCase.Application.DTOs.Tasks;

/// <summary>
/// DTO for Task entity
/// </summary>
public class TaskDto : AuditableDto
{
    /// <summary>
    /// Task unique identifier
    /// </summary>
    public new int Id { get; set; }

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
    /// Project name
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Assigned user ID
    /// </summary>
    public int? AssigneeId { get; set; }

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
    /// Parent task ID (for subtasks)
    /// </summary>
    public int? ParentTaskId { get; set; }

    /// <summary>
    /// Parent task title
    /// </summary>
    public string? ParentTaskTitle { get; set; }

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

    /// <summary>
    /// Task progress percentage
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Number of subtasks
    /// </summary>
    public int SubtaskCount { get; set; }

    /// <summary>
    /// Number of completed subtasks
    /// </summary>
    public int CompletedSubtaskCount { get; set; }

    /// <summary>
    /// Number of comments
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// List of subtasks
    /// </summary>
    public List<TaskSummaryDto> Subtasks { get; set; } = [];

    /// <summary>
    /// List of comments
    /// </summary>
    public List<TaskCommentDto> Comments { get; set; } = [];
}

/// <summary>
/// DTO for TaskComment entity
/// </summary>
public class TaskCommentDto : AuditableDto
{
    /// <summary>
    /// Comment unique identifier
    /// </summary>
    public new int Id { get; set; }

    /// <summary>
    /// Task ID the comment belongs to
    /// </summary>
    public int TaskId { get; set; }

    /// <summary>
    /// Comment author user ID
    /// </summary>
    public int AuthorId { get; set; }

    /// <summary>
    /// Comment author name
    /// </summary>
    public string AuthorName { get; set; } = string.Empty;

    /// <summary>
    /// Comment content
    /// </summary>
    public string Content { get; set; } = string.Empty;
}

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

/// <summary>
/// DTO for creating a task comment
/// </summary>
public class CreateTaskCommentDto
{
    /// <summary>
    /// Task ID the comment belongs to
    /// </summary>
    public int TaskId { get; set; }

    /// <summary>
    /// Comment content
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
