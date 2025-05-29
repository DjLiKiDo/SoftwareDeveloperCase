using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence;

/// <summary>
/// Unit of Work pattern interface for coordinating multiple repository operations
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the repository for Department entities
    /// </summary>
    IDepartmentRepository DepartmentRepository { get; }

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
}
