using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Enums.Core;
using SoftwareDeveloperCase.Domain.ValueObjects;

namespace SoftwareDeveloperCase.Domain.Entities.Core;

/// <summary>
/// Represents a task in the system.
/// </summary>
public class Task : BaseEntity
{
    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the task status.
    /// </summary>
    public Enums.Core.TaskStatus Status { get; set; } = Enums.Core.TaskStatus.Todo;

    /// <summary>
    /// Gets or sets the task priority.
    /// </summary>
    public Priority Priority { get; set; } = Priority.Medium;

    /// <summary>
    /// Gets or sets the estimated hours for the task.
    /// </summary>
    public decimal? EstimatedHours { get; set; }

    /// <summary>
    /// Gets or sets the actual hours spent on the task.
    /// </summary>
    public decimal? ActualHours { get; set; }

    /// <summary>
    /// Gets or sets the task due date
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the task hierarchy information.
    /// </summary>
    public TaskHierarchy Hierarchy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the project identifier this task belongs to.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the project this task belongs to.
    /// </summary>
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user identifier assigned to this task.
    /// </summary>
    public Guid? AssignedToId { get; set; }

    /// <summary>
    /// Gets or sets the user assigned to this task.
    /// </summary>
    public User? AssignedTo { get; set; }

    /// <summary>
    /// Gets or sets the parent task identifier for subtasks.
    /// </summary>
    public Guid? ParentTaskId { get; set; }

    /// <summary>
    /// Gets or sets the parent task for subtasks.
    /// </summary>
    public Task? ParentTask { get; set; }

    /// <summary>
    /// Gets or sets the collection of subtasks.
    /// </summary>
    public ICollection<Task> SubTasks { get; set; } = new List<Task>();

    /// <summary>
    /// Gets or sets the collection of comments on this task.
    /// </summary>
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
}
