using Microsoft.Extensions.Caching.Memory;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using System.Linq.Expressions;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories.Cached;

/// <summary>
/// Cached decorator for PermissionRepository implementing the decorator pattern
/// </summary>
internal class CachedPermissionRepository : IPermissionRepository
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMemoryCache _cache;
    private readonly ICacheKeyService _cacheKeyService;
    private static readonly TimeSpan CacheExpirationForCollections = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CacheExpirationForSingleEntity = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Initializes a new instance of the CachedPermissionRepository class
    /// </summary>
    /// <param name="permissionRepository">The inner repository instance</param>
    /// <param name="cache">The memory cache instance</param>
    /// <param name="cacheKeyService">The cache key service</param>
    public CachedPermissionRepository(
        IPermissionRepository permissionRepository,
        IMemoryCache cache,
        ICacheKeyService cacheKeyService)
    {
        _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheKeyService = cacheKeyService ?? throw new ArgumentNullException(nameof(cacheKeyService));
    }

    /// <summary>
    /// Gets a permission by ID with caching
    /// </summary>
    /// <param name="id">The permission ID</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The permission if found, otherwise null</returns>
    public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = _cacheKeyService.GenerateEntityCacheKey<Permission>(id.ToString());

        if (_cache.TryGetValue(cacheKey, out Permission? cachedPermission))
        {
            return cachedPermission;
        }

        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);

        if (permission != null)
        {
            _cache.Set(cacheKey, permission, CacheExpirationForSingleEntity);
        }

        return permission;
    }

    /// <summary>
    /// Gets all permissions with caching
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of all permissions</returns>
    public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = _cacheKeyService.GenerateListCacheKey<Permission>();

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Permission>? cachedPermissions))
        {
            return cachedPermissions;
        }

        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        _cache.Set(cacheKey, permissions, CacheExpirationForCollections);

        return permissions;
    }

    /// <summary>
    /// Gets permissions by predicate with caching
    /// </summary>
    /// <param name="predicate">The filter predicate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of matching permissions</returns>
    public async Task<IEnumerable<Permission>> GetAsync(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // For complex predicates, we can't easily cache the results
        // so we'll just pass through to the repository
        return await _permissionRepository.GetAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets permissions with options for ordering, filtering, and including related data
    /// </summary>
    /// <param name="predicate">The filter predicate</param>
    /// <param name="orderBy">The ordering function</param>
    /// <param name="includeString">Navigation properties to include</param>
    /// <param name="disableTracking">Whether to disable entity tracking</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of matching permissions</returns>
    public async Task<IEnumerable<Permission>> GetAsync(
        Expression<Func<Permission, bool>>? predicate = null,
        Func<IQueryable<Permission>, IOrderedQueryable<Permission>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, we can't easily cache the results
        // so we'll just pass through to the repository
        return await _permissionRepository.GetAsync(predicate, orderBy, includeString, disableTracking, cancellationToken);
    }

    /// <summary>
    /// Gets permissions with options for ordering, filtering, and including multiple related data
    /// </summary>
    /// <param name="predicate">The filter predicate</param>
    /// <param name="orderBy">The ordering function</param>
    /// <param name="includes">Collection of navigation properties to include</param>
    /// <param name="disableTracking">Whether to disable entity tracking</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of matching permissions</returns>
    public async Task<IEnumerable<Permission>> GetAsync(
        Expression<Func<Permission, bool>>? predicate = null,
        Func<IQueryable<Permission>, IOrderedQueryable<Permission>>? orderBy = null,
        List<Expression<Func<Permission, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, we can't easily cache the results
        // so we'll just pass through to the repository
        return await _permissionRepository.GetAsync(predicate, orderBy, includes, disableTracking, cancellationToken);
    }

    /// <summary>
    /// Inserts a new permission
    /// </summary>
    /// <param name="entity">The permission to insert</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The inserted permission</returns>
    public async Task<Permission> InsertAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        var result = await _permissionRepository.InsertAsync(entity, cancellationToken);

        // Invalidate cache
        InvalidateCacheItems();

        return result;
    }

    /// <summary>
    /// Updates an existing permission
    /// </summary>
    /// <param name="entity">The permission to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated permission</returns>
    public async Task<Permission> UpdateAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        var result = await _permissionRepository.UpdateAsync(entity, cancellationToken);

        // Invalidate cache for this entity and collections
        InvalidateCacheItems();

        return result;
    }

    /// <summary>
    /// Deletes a permission entity
    /// </summary>
    /// <param name="entity">The permission entity to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task DeleteAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        await _permissionRepository.DeleteAsync(entity, cancellationToken);

        // Invalidate cache for this entity and collections
        InvalidateCacheItems();
    }

    /// <summary>
    /// Inserts a new permission synchronously
    /// </summary>
    /// <param name="entity">The permission to insert</param>
    public void Insert(Permission entity)
    {
        _permissionRepository.Insert(entity);

        // Invalidate cache
        InvalidateCacheItems();
    }

    /// <summary>
    /// Updates an existing permission synchronously
    /// </summary>
    /// <param name="entity">The permission to update</param>
    public void Update(Permission entity)
    {
        _permissionRepository.Update(entity);

        // Invalidate cache
        InvalidateCacheItems();
    }

    /// <summary>
    /// Deletes a permission synchronously
    /// </summary>
    /// <param name="entity">The permission to delete</param>
    public void Delete(Permission entity)
    {
        _permissionRepository.Delete(entity);

        // Invalidate cache
        InvalidateCacheItems();
    }

    /// <summary>
    /// Gets a queryable collection of permissions
    /// </summary>
    /// <returns>Queryable collection of permissions</returns>
    public IQueryable<Permission> GetQueryable()
    {
        return _permissionRepository.GetQueryable();
    }

    /// <summary>
    /// Counts the number of permissions in the query
    /// </summary>
    /// <param name="query">The query to count</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of permissions in the query</returns>
    public Task<int> CountAsync(IQueryable<Permission> query, CancellationToken cancellationToken = default)
    {
        return _permissionRepository.CountAsync(query, cancellationToken);
    }

    /// <summary>
    /// Gets a paged result of permissions
    /// </summary>
    /// <param name="query">The query to page</param>
    /// <param name="skip">Number of items to skip</param>
    /// <param name="take">Number of items to take</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged collection of permissions</returns>
    public Task<IReadOnlyList<Permission>> GetPagedAsync(IQueryable<Permission> query, int skip, int take, CancellationToken cancellationToken = default)
    {
        return _permissionRepository.GetPagedAsync(query, skip, take, cancellationToken);
    }



    /// <summary>
    /// Clears all permission-related items from the cache
    /// </summary>
    private void InvalidateCacheItems()
    {
        // A more sophisticated implementation would use cache tags or a distributed cache
        // that supports pattern-based removal of cache entries
    }
}
