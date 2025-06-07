using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence;

/// <summary>
/// Unit of Work pattern interface for coordinating multiple repository operations
/// </summary>
public interface IUnitOfWork : IDisposable
{
    #region Identity Repositories
    /// <summary>
    /// Gets the repository for Permission entities
    /// </summary>
    IPermissionRepository PermissionRepository { get; }

    /// <summary>
    /// Gets the repository for RolePermission entities
    /// </summary>
    IRolePermissionRepository RolePermissionRepository { get; }

    /// <summary>
    /// Gets the repository for Role entities
    /// </summary>
    IRoleRepository RoleRepository { get; }

    /// <summary>
    /// Gets the repository for User entities
    /// </summary>
    IUserRepository UserRepository { get; }

    /// <summary>
    /// Gets the repository for UserRole entities
    /// </summary>
    IUserRoleRepository UserRoleRepository { get; }
    #endregion

    #region Core Repositories
    /// <summary>
    /// Gets the repository for Team entities
    /// </summary>
    ITeamRepository TeamRepository { get; }

    /// <summary>
    /// Gets the repository for TeamMember entities
    /// </summary>
    ITeamMemberRepository TeamMemberRepository { get; }

    /// <summary>
    /// Gets the repository for Project entities
    /// </summary>
    IProjectRepository ProjectRepository { get; }

    /// <summary>
    /// Gets the repository for Task entities
    /// </summary>
    ITaskRepository TaskRepository { get; }

    /// <summary>
    /// Gets the repository for TaskComment entities
    /// </summary>
    ITaskCommentRepository TaskCommentRepository { get; }
    #endregion

    /// <summary>
    /// Gets a generic repository for the specified entity type
    /// </summary>
    /// <typeparam name="TEntity">The entity type that extends BaseEntity</typeparam>
    /// <returns>The repository instance for the entity type</returns>
    IRepository<TEntity>? Repository<TEntity>() where TEntity : BaseEntity;

    /// <summary>
    /// Saves all changes made in this unit of work to the database
    /// </summary>
    /// <returns>The number of affected rows</returns>
    Task<int> SaveChanges();

    /// <summary>
    /// Saves all changes made in this unit of work to the database asynchronously with cancellation token support
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>The number of affected rows</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    #region Transaction Management
    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction and saves all changes
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>The number of affected rows</returns>
    Task<int> CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction and discards all changes
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a function within a database transaction with automatic rollback on exceptions
    /// </summary>
    /// <typeparam name="T">The return type of the function</typeparam>
    /// <param name="operation">The operation to execute within the transaction</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>The result of the operation</returns>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a database transaction with automatic rollback on exceptions
    /// </summary>
    /// <param name="operation">The operation to execute within the transaction</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a value indicating whether a transaction is currently active
    /// </summary>
    bool HasActiveTransaction { get; }

    /// <summary>
    /// Gets the current transaction identifier, or null if no transaction is active
    /// </summary>
    Guid? CurrentTransactionId { get; }
    #endregion
}
