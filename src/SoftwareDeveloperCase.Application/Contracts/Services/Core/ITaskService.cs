using SoftwareDeveloperCase.Domain.Entities.Task;
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Enums.Core;
using TaskEntity = SoftwareDeveloperCase.Domain.Entities.Task.Task;
using TaskStatusEnum = SoftwareDeveloperCase.Domain.Enums.Core.TaskStatus;

namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// Service interface for task management operations
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Validates task business rules before creation or update
    /// </summary>
    /// <param name="task">The task to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if valid, false otherwise</returns>
    Task<bool> ValidateTaskAsync(TaskEntity task, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if task can transition to new status
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="newStatus">The new status</param>
    /// <param name="userId">The user attempting the transition</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if transition is valid, false otherwise</returns>
    Task<bool> CanTransitionToStatusAsync(int taskId, TaskStatusEnum newStatus, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates task progress based on subtasks
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Progress percentage (0-100)</returns>
    Task<decimal> CalculateTaskProgressAsync(int taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task dependencies and validates circular references
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="dependencyTaskId">The proposed dependency task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if dependency is valid, false if it would create a circular reference</returns>
    Task<bool> ValidateTaskDependencyAsync(int taskId, int dependencyTaskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Estimates task completion date based on current progress and workload
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Estimated completion date</returns>
    Task<DateTime?> EstimateCompletionDateAsync(int taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task workload analytics for an assignee
    /// </summary>
    /// <param name="assigneeId">The assignee user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task workload information</returns>
    Task<TaskWorkloadDto> GetTaskWorkloadAsync(int assigneeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs time spent on a task
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="hoursSpent">Hours spent</param>
    /// <param name="description">Description of work done</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if logged successfully, false otherwise</returns>
    Task<bool> LogTimeSpentAsync(int taskId, int userId, decimal hoursSpent, string description, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task burndown chart data
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="startDate">Start date for chart</param>
    /// <param name="endDate">End date for chart</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Burndown chart data</returns>
    Task<TaskBurndownDto> GetTaskBurndownAsync(int projectId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
