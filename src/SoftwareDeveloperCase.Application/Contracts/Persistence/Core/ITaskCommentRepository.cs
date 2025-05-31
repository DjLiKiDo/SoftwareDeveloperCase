using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities.Task;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence.Core;

/// <summary>
/// Repository interface for TaskComment entity operations
/// </summary>
public interface ITaskCommentRepository : IRepository<TaskComment>
{
    /// <summary>
    /// Gets comments by task ID
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of comments for the task</returns>
    Task<IReadOnlyList<TaskComment>> GetCommentsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets comments by author ID
    /// </summary>
    /// <param name="authorId">The author user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of comments by the author</returns>
    Task<IReadOnlyList<TaskComment>> GetCommentsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets comments with author information
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of comments with author details</returns>
    Task<IReadOnlyList<TaskComment>> GetCommentsWithAuthorAsync(Guid taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent comments across all tasks for a project
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="take">Number of comments to take</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of recent comments</returns>
    Task<IReadOnlyList<TaskComment>> GetRecentCommentsByProjectIdAsync(Guid projectId, int take = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent comments across all tasks for a team
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="take">Number of comments to take</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of recent comments</returns>
    Task<IReadOnlyList<TaskComment>> GetRecentCommentsByTeamIdAsync(Guid teamId, int take = 10, CancellationToken cancellationToken = default);
}
