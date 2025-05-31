using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.Enums.Core;
using TaskEntity = SoftwareDeveloperCase.Domain.Entities.Core.Task;
using TaskStatusEnum = SoftwareDeveloperCase.Domain.Enums.Core.TaskStatus;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence.Core;

/// <summary>
/// Repository interface for Task entity operations
/// </summary>
public interface ITaskRepository : IRepository<TaskEntity>
{
    /// <summary>
    /// Gets tasks by project ID
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of tasks for the project</returns>
    Task<IReadOnlyList<TaskEntity>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks assigned to a user
    /// </summary>
    /// <param name="assigneeId">The assignee user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of tasks assigned to the user</returns>
    Task<IReadOnlyList<TaskEntity>> GetTasksByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks by status
    /// </summary>
    /// <param name="status">The task status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of tasks with the specified status</returns>
    Task<IReadOnlyList<TaskEntity>> GetTasksByStatusAsync(TaskStatusEnum status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks by priority
    /// </summary>
    /// <param name="priority">The task priority</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of tasks with the specified priority</returns>
    Task<IReadOnlyList<TaskEntity>> GetTasksByPriorityAsync(Priority priority, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subtasks of a parent task
    /// </summary>
    /// <param name="parentTaskId">The parent task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of subtasks</returns>
    Task<IReadOnlyList<TaskEntity>> GetSubtasksAsync(Guid parentTaskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task with comments included
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task with comments or null if not found</returns>
    Task<TaskEntity?> GetTaskWithCommentsAsync(Guid taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task with all related data (project, assignee, comments, subtasks)
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task with all related data or null if not found</returns>
    Task<TaskEntity?> GetTaskWithDetailsAsync(Guid taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets overdue tasks
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of overdue tasks</returns>
    Task<IReadOnlyList<TaskEntity>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks due within specified days
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of tasks due within the specified timeframe</returns>
    Task<IReadOnlyList<TaskEntity>> GetTasksDueWithinDaysAsync(int days, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task hierarchy (parent task with all subtasks)
    /// </summary>
    /// <param name="rootTaskId">The root task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task hierarchy</returns>
    Task<TaskEntity?> GetTaskHierarchyAsync(Guid rootTaskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates task estimated hours and logs time
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="estimatedHours">Estimated hours</param>
    /// <param name="actualHours">Actual hours spent</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if updated successfully</returns>
    Task<bool> UpdateTaskTimeTrackingAsync(Guid taskId, decimal? estimatedHours, decimal? actualHours, CancellationToken cancellationToken = default);
}
