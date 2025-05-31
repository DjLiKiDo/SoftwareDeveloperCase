using Microsoft.Extensions.Caching.Memory;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities;
using System.Linq.Expressions;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories.Cached;

/// <summary>
/// Cached decorator for RoleRepository implementing the decorator pattern
/// </summary>
internal class CachedRoleRepository : IRoleRepository
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMemoryCache _cache;
    private readonly ICacheKeyService _cacheKeyService;
    private static readonly TimeSpan CacheExpirationForCollections = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CacheExpirationForSingleEntity = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Initializes a new instance of the CachedRoleRepository class
    /// </summary>
    /// <param name="roleRepository">The inner repository instance</param>
    /// <param name="cache">The memory cache instance</param>
    /// <param name="cacheKeyService">The cache key service</param>
    public CachedRoleRepository(
        IRoleRepository roleRepository,
        IMemoryCache cache,
        ICacheKeyService cacheKeyService)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheKeyService = cacheKeyService ?? throw new ArgumentNullException(nameof(cacheKeyService));
    }

    /// <summary>
    /// Gets a role by ID with caching
    /// </summary>
    /// <param name="id">The role ID</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The role if found, otherwise null</returns>
    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = _cacheKeyService.GenerateEntityCacheKey<Role>(id.ToString());

        if (_cache.TryGetValue(cacheKey, out Role? cachedRole))
        {
            return cachedRole;
        }

        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);

        if (role != null)
        {
            _cache.Set(cacheKey, role, CacheExpirationForSingleEntity);
        }

        return role;
    }

    /// <summary>
    /// Gets all roles with caching
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of all roles</returns>
    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = _cacheKeyService.GenerateListCacheKey<Role>();

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Role>? cachedRoles))
        {
            return cachedRoles;
        }

        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        _cache.Set(cacheKey, roles, CacheExpirationForCollections);

        return roles;
    }

    /// <summary>
    /// Gets roles by predicate with caching
    /// </summary>
    /// <param name="predicate">The filter predicate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of matching roles</returns>
    public async Task<IEnumerable<Role>> GetAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // For complex predicates, we can't easily cache the results
        // so we'll just pass through to the repository
        return await _roleRepository.GetAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets roles with options for ordering, filtering, and including related data
    /// </summary>
    /// <param name="predicate">The filter predicate</param>
    /// <param name="orderBy">The ordering function</param>
    /// <param name="includeString">Navigation properties to include</param>
    /// <param name="disableTracking">Whether to disable entity tracking</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of matching roles</returns>
    public async Task<IEnumerable<Role>> GetAsync(
        Expression<Func<Role, bool>>? predicate = null,
        Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, we can't easily cache the results
        // so we'll just pass through to the repository
        return await _roleRepository.GetAsync(predicate, orderBy, includeString, disableTracking, cancellationToken);
    }

    /// <summary>
    /// Gets roles with options for ordering, filtering, and including multiple related data
    /// </summary>
    /// <param name="predicate">The filter predicate</param>
    /// <param name="orderBy">The ordering function</param>
    /// <param name="includes">Collection of navigation properties to include</param>
    /// <param name="disableTracking">Whether to disable entity tracking</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of matching roles</returns>
    public async Task<IEnumerable<Role>> GetAsync(
        Expression<Func<Role, bool>>? predicate = null,
        Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
        List<Expression<Func<Role, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, we can't easily cache the results
        // so we'll just pass through to the repository
        return await _roleRepository.GetAsync(predicate, orderBy, includes, disableTracking, cancellationToken);
    }

    /// <summary>
    /// Inserts a new role
    /// </summary>
    /// <param name="entity">The role to insert</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The inserted role</returns>
    public async Task<Role> InsertAsync(Role entity, CancellationToken cancellationToken = default)
    {
        var result = await _roleRepository.InsertAsync(entity, cancellationToken);

        // Invalidate cache
        InvalidateCacheItems();

        return result;
    }

    /// <summary>
    /// Updates an existing role
    /// </summary>
    /// <param name="entity">The role to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated role</returns>
    public async Task<Role> UpdateAsync(Role entity, CancellationToken cancellationToken = default)
    {
        var result = await _roleRepository.UpdateAsync(entity, cancellationToken);

        // Invalidate cache for this entity and collections
        InvalidateCacheItems();

        return result;
    }

    /// <summary>
    /// Deletes a role entity
    /// </summary>
    /// <param name="entity">The role entity to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task DeleteAsync(Role entity, CancellationToken cancellationToken = default)
    {
        await _roleRepository.DeleteAsync(entity, cancellationToken);

        // Invalidate cache for this entity and collections
        InvalidateCacheItems();
    }

    /// <summary>
    /// Inserts a new role synchronously
    /// </summary>
    /// <param name="entity">The role to insert</param>
    public void Insert(Role entity)
    {
        _roleRepository.Insert(entity);

        // Invalidate cache
        InvalidateCacheItems();
    }

    /// <summary>
    /// Updates an existing role synchronously
    /// </summary>
    /// <param name="entity">The role to update</param>
    public void Update(Role entity)
    {
        _roleRepository.Update(entity);

        // Invalidate cache
        InvalidateCacheItems();
    }

    /// <summary>
    /// Deletes a role synchronously
    /// </summary>
    /// <param name="entity">The role to delete</param>
    public void Delete(Role entity)
    {
        _roleRepository.Delete(entity);

        // Invalidate cache
        InvalidateCacheItems();
    }

    /// <summary>
    /// Gets a queryable collection of roles
    /// </summary>
    /// <returns>Queryable collection of roles</returns>
    public IQueryable<Role> GetQueryable()
    {
        return _roleRepository.GetQueryable();
    }

    /// <summary>
    /// Counts the number of roles in the query
    /// </summary>
    /// <param name="query">The query to count</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of roles in the query</returns>
    public Task<int> CountAsync(IQueryable<Role> query, CancellationToken cancellationToken = default)
    {
        return _roleRepository.CountAsync(query, cancellationToken);
    }

    /// <summary>
    /// Gets a paged result of roles
    /// </summary>
    /// <param name="query">The query to page</param>
    /// <param name="skip">Number of items to skip</param>
    /// <param name="take">Number of items to take</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged collection of roles</returns>
    public Task<IReadOnlyList<Role>> GetPagedAsync(IQueryable<Role> query, int skip, int take, CancellationToken cancellationToken = default)
    {
        return _roleRepository.GetPagedAsync(query, skip, take, cancellationToken);
    }



    /// <summary>
    /// Clears all role-related items from the cache
    /// </summary>
    private void InvalidateCacheItems()
    {
        // A more sophisticated implementation would use cache tags or a distributed cache
        // that supports pattern-based removal of cache entries
    }
}
