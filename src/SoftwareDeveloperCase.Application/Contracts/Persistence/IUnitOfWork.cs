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
}
