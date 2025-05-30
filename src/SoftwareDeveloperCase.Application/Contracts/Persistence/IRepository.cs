using SoftwareDeveloperCase.Domain.Common;
using System.Linq.Expressions;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence;

/// <summary>
/// Generic repository interface for basic CRUD operations on entities.
/// </summary>
/// <typeparam name="T">The entity type that inherits from BaseEntity.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Gets an entity by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all entities asynchronously.
    /// </summary>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Gets entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <returns>A collection of entities that match the predicate.</returns>
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets entities with advanced filtering, ordering, and including options asynchronously.
    /// </summary>
    /// <param name="predicate">Optional predicate to filter entities.</param>
    /// <param name="orderBy">Optional function to order entities.</param>
    /// <param name="includeString">Optional string for including related entities.</param>
    /// <param name="disableTracking">Whether to disable entity tracking.</param>
    /// <returns>A collection of entities based on the specified criteria.</returns>
    Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true);

    /// <summary>
    /// Gets entities with advanced filtering, ordering, and including options asynchronously.
    /// </summary>
    /// <param name="predicate">Optional predicate to filter entities.</param>
    /// <param name="orderBy">Optional function to order entities.</param>
    /// <param name="includes">Optional list of expressions for including related entities.</param>
    /// <param name="disableTracking">Whether to disable entity tracking.</param>
    /// <returns>A collection of entities based on the specified criteria.</returns>
    Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<Expression<Func<T, object>>>? includes = null,
        bool disableTracking = true);

    /// <summary>
    /// Inserts a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <returns>The inserted entity.</returns>
    Task<T> InsertAsync(T entity);

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The updated entity.</returns>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Inserts a new entity synchronously.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    void Insert(T entity);

    /// <summary>
    /// Updates an existing entity synchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Deletes an entity synchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(T entity);
}
