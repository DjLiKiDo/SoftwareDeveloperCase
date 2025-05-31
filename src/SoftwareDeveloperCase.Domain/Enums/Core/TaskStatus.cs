namespace SoftwareDeveloperCase.Domain.Enums.Core;

/// <summary>
/// Represents the status of a task.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// Task is in the backlog and not yet started.
    /// </summary>
    Todo = 0,

    /// <summary>
    /// Task is currently being worked on.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Task is under review.
    /// </summary>
    InReview = 2,

    /// <summary>
    /// Task has been completed.
    /// </summary>
    Done = 3,

    /// <summary>
    /// Task is blocked and cannot proceed.
    /// </summary>
    Blocked = 4
}
