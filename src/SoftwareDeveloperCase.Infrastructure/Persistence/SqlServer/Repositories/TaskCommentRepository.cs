using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Domain.Entities.Task;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

/// <summary>
/// Task comment repository implementation providing data access for TaskComment entities
/// </summary>
internal class TaskCommentRepository : Repository<TaskComment>, ITaskCommentRepository
{
    public TaskCommentRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets comments by task ID
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of comments for the task</returns>
    public async Task<IReadOnlyList<TaskComment>> GetCommentsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var comments = await _context.TaskComments!
            .Include(tc => tc.Author)
            .Include(tc => tc.Task)
            .Where(tc => tc.TaskId == taskId)
            .OrderBy(tc => tc.CreatedAt)
            .ToListAsync(cancellationToken);
        
        return comments.AsReadOnly();
    }

    /// <summary>
    /// Gets comments by author ID
    /// </summary>
    /// <param name="authorId">The author user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of comments by the author</returns>
    public async Task<IReadOnlyList<TaskComment>> GetCommentsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var comments = await _context.TaskComments!
            .Include(tc => tc.Author) // Changed from tc.User to tc.Author
            .Include(tc => tc.Task)
            .ThenInclude(t => t!.Project) // Added null-forgiving operator
            .Where(tc => tc.AuthorId == authorId) // Changed from tc.UserId to tc.AuthorId
            .OrderByDescending(tc => tc.CreatedAt)
            .ToListAsync(cancellationToken);
        
        return comments.AsReadOnly();
    }

    /// <summary>
    /// Gets comments with author information
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of comments with author details</returns>
    public async Task<IReadOnlyList<TaskComment>> GetCommentsWithAuthorAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var comments = await _context.TaskComments!
            .Include(tc => tc.Author) // Changed from tc.User to tc.Author
            .Include(tc => tc.Task)
            .Where(tc => tc.TaskId == taskId)
            .OrderBy(tc => tc.CreatedAt)
            .ToListAsync(cancellationToken);
        
        return comments.AsReadOnly();
    }

    /// <summary>
    /// Gets recent comments across all tasks for a project
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="take">Number of comments to take</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of recent comments</returns>
    public async Task<IReadOnlyList<TaskComment>> GetRecentCommentsByProjectIdAsync(Guid projectId, int take = 10, CancellationToken cancellationToken = default)
    {
        var comments = await _context.TaskComments!
            .Include(tc => tc.Author) // Changed from tc.User to tc.Author
            .Include(tc => tc.Task)
            .Where(tc => tc.Task!.ProjectId == projectId) // Added null-forgiving operator
            .OrderByDescending(tc => tc.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
        
        return comments.AsReadOnly();
    }

    /// <summary>
    /// Gets recent comments across all tasks for a team
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="take">Number of comments to take</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of recent comments</returns>
    public async Task<IReadOnlyList<TaskComment>> GetRecentCommentsByTeamIdAsync(Guid teamId, int take = 10, CancellationToken cancellationToken = default)
    {
        var comments = await _context.TaskComments!
            .Include(tc => tc.Author) // Changed from tc.User to tc.Author
            .Include(tc => tc.Task)
            .ThenInclude(t => t!.Project) // Added null-forgiving operator
            .Where(tc => tc.Task!.Project!.TeamId == teamId) // Added null-forgiving operators
            .OrderByDescending(tc => tc.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
        
        return comments.AsReadOnly();
    }

    public async Task<IEnumerable<TaskComment>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var query = _context.TaskComments?.Where(tc => tc.TaskId == taskId)
                                          .Include(tc => tc.Author) // Changed from tc.User to tc.Author
                                          .OrderBy(tc => tc.CreatedAt);
        
        return query != null ? await query.ToListAsync(cancellationToken) : new List<TaskComment>();
    }

    public async Task<IEnumerable<TaskComment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = _context.TaskComments?.Where(tc => tc.AuthorId == userId) // Changed from tc.UserId to tc.AuthorId
                                          .Include(tc => tc.Author) // Changed from tc.User to tc.Author
                                          .Include(tc => tc.Task);
        
        return query != null ? await query.ToListAsync(cancellationToken) : new List<TaskComment>();
    }

    public async Task<IEnumerable<TaskComment>> GetRecentCommentsAsync(Guid taskId, int count, CancellationToken cancellationToken = default)
    {
        var query = _context.TaskComments?.Where(tc => tc.TaskId == taskId)
                                          .Include(tc => tc.Author) // Changed from tc.User to tc.Author
                                          .OrderByDescending(tc => tc.CreatedAt)
                                          .Take(count);
        
        return query != null ? await query.ToListAsync(cancellationToken) : new List<TaskComment>();
    }

    public async Task<IEnumerable<TaskComment>> GetCommentsByDateRangeAsync(Guid taskId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var query = _context.TaskComments?.Where(tc => tc.TaskId == taskId && 
                                                       tc.CreatedAt >= startDate && 
                                                       tc.CreatedAt <= endDate)
                                          .Include(tc => tc.Author) // Changed from tc.User to tc.Author
                                          .OrderBy(tc => tc.CreatedAt);
        
        return query != null ? await query.ToListAsync(cancellationToken) : new List<TaskComment>();
    }
}
