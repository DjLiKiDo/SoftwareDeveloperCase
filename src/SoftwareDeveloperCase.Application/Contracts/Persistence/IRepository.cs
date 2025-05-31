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
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of entities that match the predicate.</returns>
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities with advanced filtering, ordering, and including options asynchronously.
    /// </summary>
    /// <param name="predicate">Optional predicate to filter entities.</param>
    /// <param name="orderBy">Optional function to order entities.</param>
    /// <param name="includeString">Optional string for including related entities.</param>
    /// <param name="disableTracking">Whether to disable entity tracking.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of entities based on the specified criteria.</returns>
    Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities with advanced filtering, ordering, and including options asynchronously.
    /// </summary>
    /// <param name="predicate">Optional predicate to filter entities.</param>
    /// <param name="orderBy">Optional function to order entities.</param>
    /// <param name="includes">Optional list of expressions for including related entities.</param>
    /// <param name="disableTracking">Whether to disable entity tracking.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of entities based on the specified criteria.</returns>
    Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<Expression<Func<T, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The inserted entity.</returns>
    Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The updated entity.</returns>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Gets a queryable collection of entities
    /// </summary>
    /// <returns>Queryable collection of entities</returns>
    IQueryable<T> GetQueryable();

    /// <summary>
    /// Counts the number of entities in the query asynchronously
    /// </summary>
    /// <param name="query">The query to count</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of entities in the query</returns>
    Task<int> CountAsync(IQueryable<T> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result from a query asynchronously
    /// </summary>
    /// <param name="query">The query to page</param>
    /// <param name="skip">Number of items to skip</param>
    /// <param name="take">Number of items to take</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged collection of entities</returns>
    Task<IReadOnlyList<T>> GetPagedAsync(IQueryable<T> query, int skip, int take, CancellationToken cancellationToken = default);
}
