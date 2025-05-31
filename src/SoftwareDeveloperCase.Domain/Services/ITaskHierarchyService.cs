using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Entities.Task;
using SoftwareDeveloperCase.Domain.ValueObjects;
using TaskEntity = SoftwareDeveloperCase.Domain.Entities.Task.Task;

namespace SoftwareDeveloperCase.Domain.Services;

/// <summary>
/// Domain service for task hierarchy management and operations.
/// </summary>
public interface ITaskHierarchyService
{
    /// <summary>
    /// Creates a hierarchy for a new root task.
    /// </summary>
    /// <param name="project">The project containing the task.</param>
    /// <returns>A new TaskHierarchy for the root task.</returns>
    TaskHierarchy CreateRootTaskHierarchy(Project project);

    /// <summary>
    /// Creates a hierarchy for a new subtask.
    /// </summary>
    /// <param name="parentTask">The parent task.</param>
    /// <returns>A new TaskHierarchy for the subtask.</returns>
    TaskHierarchy CreateSubTaskHierarchy(TaskEntity parentTask);

    /// <summary>
    /// Validates if a task can be moved to a new parent.
    /// </summary>
    /// <param name="task">The task to move.</param>
    /// <param name="newParent">The new parent task.</param>
    /// <returns>True if the move is valid.</returns>
    bool CanMoveTask(TaskEntity task, TaskEntity? newParent);

    /// <summary>
    /// Gets all tasks in a hierarchy tree starting from a root task.
    /// </summary>
    /// <param name="rootTask">The root task.</param>
    /// <returns>All tasks in the hierarchy tree.</returns>
    IEnumerable<TaskEntity> GetTaskHierarchy(TaskEntity rootTask);

    /// <summary>
    /// Calculates the total estimated hours for a task including all subtasks.
    /// </summary>
    /// <param name="task">The task to calculate for.</param>
    /// <returns>Total estimated hours including subtasks.</returns>
    decimal CalculateTotalEstimatedHours(TaskEntity task);

    /// <summary>
    /// Calculates the total actual hours for a task including all subtasks.
    /// </summary>
    /// <param name="task">The task to calculate for.</param>
    /// <returns>Total actual hours including subtasks.</returns>
    decimal CalculateTotalActualHours(TaskEntity task);

    /// <summary>
    /// Gets the maximum allowed hierarchy depth for tasks.
    /// </summary>
    int MaxHierarchyDepth { get; }
}
