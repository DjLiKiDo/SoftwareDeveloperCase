using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using TaskEntity = SoftwareDeveloperCase.Domain.Entities.Task.Task;
using DomainTaskStatus = SoftwareDeveloperCase.Domain.Enums.Core.TaskStatus;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

/// <summary>
/// Task repository implementation providing data access for Task entities
/// </summary>
internal class TaskRepository : Repository<TaskEntity>, ITaskRepository
{
    public TaskRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets a task by ID with all required related entities included
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task with Project, AssignedTo, SubTasks included or null if not found</returns>
    public new async Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await (_context.Tasks?
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.SubTasks)
            .ThenInclude(st => st.AssignedTo) ?? throw new InvalidOperationException("Tasks DbSet is null"))
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <summary>
    /// Updates a task with optimistic concurrency handling
    /// </summary>
    /// <param name="entity">Task entity to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated task entity</returns>
    /// <exception cref="DbUpdateConcurrencyException">Thrown when concurrency conflict occurs</exception>
    public new async Task<TaskEntity> UpdateAsync(TaskEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Set<TaskEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Re-throw to allow caller to handle concurrency conflicts appropriately
            throw;
        }
    }

    /// <summary>
    /// Deletes a task and cascades to all subtasks
    /// </summary>
    /// <param name="entity">Task entity to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public new async Task DeleteAsync(TaskEntity entity, CancellationToken cancellationToken = default)
    {
        // Load the task with all its subtasks for cascade deletion
        var taskWithSubtasks = await (_context.Tasks?
            .Include(t => t.SubTasks)
            .ThenInclude(st => st.SubTasks) ?? throw new InvalidOperationException("Tasks DbSet is null"))
            .FirstOrDefaultAsync(t => t.Id == entity.Id, cancellationToken);

        if (taskWithSubtasks != null)
        {
            // Recursively delete all subtasks first
            await DeleteSubtasksRecursively(taskWithSubtasks, cancellationToken);
            
            // Then delete the task itself
            _context.Set<TaskEntity>().Remove(taskWithSubtasks);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Recursively deletes all subtasks of a given task
    /// </summary>
    /// <param name="task">Parent task</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task DeleteSubtasksRecursively(TaskEntity task, CancellationToken cancellationToken)
    {
        if (task.SubTasks?.Any() == true)
        {
            foreach (var subtask in task.SubTasks.ToList())
            {
                // Load subtask with its own subtasks
                var subtaskWithChildren = await (_context.Tasks?
                    .Include(t => t.SubTasks)
                    .ThenInclude(st => st.SubTasks) ?? throw new InvalidOperationException("Tasks DbSet is null"))
                    .FirstOrDefaultAsync(t => t.Id == subtask.Id, cancellationToken);

                if (subtaskWithChildren != null)
                {
                    // Recursively delete children first
                    await DeleteSubtasksRecursively(subtaskWithChildren, cancellationToken);
                    
                    // Then remove the subtask
                    _context.Set<TaskEntity>().Remove(subtaskWithChildren);
                }
            }
        }
    }

    /// <summary>
    /// Gets tasks by project ID with assigned user information
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of project tasks</returns>
    public async Task<IReadOnlyList<TaskEntity>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var tasks = await (_context.Tasks?
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.ParentTask)
            .Where(t => t.ProjectId == projectId) ?? throw new InvalidOperationException("Tasks DbSet is null"))
            .ToListAsync(cancellationToken);

        return tasks.AsReadOnly();
    }

    /// <summary>
    /// Gets tasks assigned to a specific user
    /// </summary>
    /// <param name="assigneeId">Assignee user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of assigned tasks</returns>
    public async Task<IReadOnlyList<TaskEntity>> GetTasksByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default)
    {
        var tasks = await (_context.Tasks?
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.ParentTask)
            .Where(t => t.AssignedToId == assigneeId) ?? throw new InvalidOperationException("Tasks DbSet is null"))
            .ToListAsync(cancellationToken);

        return tasks.AsReadOnly();
    }

    /// <summary>
    /// Gets tasks by status
    /// </summary>
    /// <param name="status">Task status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of tasks with specified status</returns>
    public async Task<IReadOnlyList<TaskEntity>> GetTasksByStatusAsync(DomainTaskStatus status, CancellationToken cancellationToken = default)
    {
        var tasks = await (_context.Tasks?
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.ParentTask)
            .Where(t => t.Status == status) ?? throw new InvalidOperationException("Tasks DbSet is null"))
            .ToListAsync(cancellationToken);

        return tasks.AsReadOnly();
    }

    /// <summary>
    /// Gets tasks by priority
    /// </summary>
    /// <param name="priority">Task priority</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of tasks with specified priority</returns>
    public async Task<IReadOnlyList<TaskEntity>> GetTasksByPriorityAsync(Domain.Enums.Core.Priority priority, CancellationToken cancellationToken = default)
    {
        var tasks = await (_context.Tasks?
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.ParentTask)
            .Where(t => t.Priority == priority) ?? throw new InvalidOperationException("Tasks DbSet is null"))
            .ToListAsync(cancellationToken);

        return tasks.AsReadOnly();
    }

    /// <summary>
    /// Gets subtasks of a specific task
    /// </summary>
    /// <param name="parentTaskId">Parent task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of subtasks</returns>
    public async Task<IReadOnlyList<TaskEntity>> GetSubtasksAsync(Guid parentTaskId, CancellationToken cancellationToken = default)
    {
        var tasks = await (_context.Tasks?
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.ParentTask)
            .Include(t => t.SubTasks)
            .Where(t => t.ParentTaskId == parentTaskId) ?? throw new InvalidOperationException("Tasks DbSet is null"))
            .ToListAsync(cancellationToken);

        return tasks.AsReadOnly();
    }

    /// <summary>
    /// Gets task with comments loaded
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task with comments or null</returns>
    public async Task<TaskEntity?> GetTaskWithCommentsAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await (_context.Tasks?
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.ParentTask)
            .Include(t => t.Comments)
            .ThenInclude(c => c.Author) ?? throw new InvalidOperationException("Tasks DbSet is null"))
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
    }

    /// <summary>
    /// Gets task by ID with full details
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task with full details or null</returns>
    public async Task<TaskEntity?> GetTaskWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await (_context.Tasks?
            .Include(t => t.Project)
            .ThenInclude(p => p.Team)
            .Include(t => t.AssignedTo)
            .Include(t => t.ParentTask)
            .Include(t => t.SubTasks)
            .ThenInclude(st => st.AssignedTo)
            .Include(t => t.Comments)
            .ThenInclude(c => c.Author) ?? throw new InvalidOperationException("Tasks DbSet is null"))
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets overdue tasks
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of overdue tasks</returns>
    public async Task<IReadOnlyList<TaskEntity>> GetOverdueTasksAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var query = _context.Tasks?.Where(t => t.DueDate.HasValue &&
                                               t.DueDate.Value < now &&
                                               t.Status != DomainTaskStatus.Done);

        return query != null ? await query.ToListAsync(cancellationToken) : new List<TaskEntity>();
    }

    /// <summary>
    /// Gets tasks due within specified days
    /// </summary>
    /// <param name="daysAhead">Number of days to look ahead</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of tasks due soon</returns>
    public async Task<IReadOnlyList<TaskEntity>> GetTasksDueWithinDaysAsync(int daysAhead, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);
        var query = _context.Tasks?.Where(t => t.DueDate.HasValue &&
                                               t.DueDate.Value <= cutoffDate &&
                                               t.Status != DomainTaskStatus.Done);

        return query != null ? await query.ToListAsync(cancellationToken) : new List<TaskEntity>();
    }

    /// <summary>
    /// Gets task hierarchy (task with all parent and child relationships)
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task with complete hierarchy</returns>
    public async Task<TaskEntity?> GetTaskHierarchyAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await (_context.Tasks?
            .Include(t => t.ParentTask)
            .ThenInclude(pt => pt!.ParentTask)
            .Include(t => t.SubTasks)
            .ThenInclude(st => st.SubTasks)
            .Include(t => t.Project)
            .Include(t => t.AssignedTo) ?? throw new InvalidOperationException("Tasks DbSet is null"))
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
    }

    /// <summary>
    /// Updates time tracking information for a task
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="estimatedHours">Estimated hours</param>
    /// <param name="actualHours">Actual hours</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if updated successfully</returns>
    public async Task<bool> UpdateTaskTimeTrackingAsync(Guid taskId, decimal? estimatedHours, decimal? actualHours, CancellationToken cancellationToken = default)
    {
        var task = await (_context.Tasks ?? throw new InvalidOperationException("Tasks DbSet is null")).FindAsync(new object[] { taskId }, cancellationToken);
        if (task == null)
            return false;

        if (estimatedHours.HasValue)
            task.EstimatedHours = estimatedHours.Value;

        if (actualHours.HasValue)
            task.ActualHours = actualHours.Value;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
