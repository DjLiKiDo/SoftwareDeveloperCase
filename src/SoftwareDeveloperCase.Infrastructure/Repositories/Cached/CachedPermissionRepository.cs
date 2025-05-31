using Microsoft.Extensions.Caching.Memory;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Infrastructure.Services;
using System.Linq.Expressions;

namespace SoftwareDeveloperCase.Infrastructure.Repositories.Cached;

/// <summary>
/// Cached decorator for PermissionRepository implementing the decorator pattern
/// </summary>
internal class CachedPermissionRepository : IPermissionRepository
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheExpirationForCollections = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CacheExpirationForSingleEntity = TimeSpan.FromMinutes(10);
    private const string EntityTypeName = "Permission";

    public CachedPermissionRepository(IPermissionRepository permissionRepository, IMemoryCache cache)
    {
        _permissionRepository = permissionRepository;
        _cache = cache;
    }

    public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyService.GetEntityByIdKey(EntityTypeName, id);

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

    public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyService.GetAllEntitiesKey(EntityTypeName);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Permission>? cachedPermissions))
        {
            return cachedPermissions!;
        }

        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        _cache.Set(cacheKey, permissions, CacheExpirationForCollections);

        return permissions;
    }

    public async Task<IEnumerable<Permission>> GetAsync(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // For filtered queries, we don't cache as the combinations are endless
        return await _permissionRepository.GetAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetAsync(Expression<Func<Permission, bool>>? predicate = null,
        Func<IQueryable<Permission>, IOrderedQueryable<Permission>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, we don't cache as the combinations are endless
        return await _permissionRepository.GetAsync(predicate, orderBy, includeString, disableTracking, cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetAsync(Expression<Func<Permission, bool>>? predicate = null,
        Func<IQueryable<Permission>, IOrderedQueryable<Permission>>? orderBy = null,
        List<Expression<Func<Permission, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, we don't cache as the combinations are endless
        return await _permissionRepository.GetAsync(predicate, orderBy, includes, disableTracking, cancellationToken);
    }

    public async Task<Permission> InsertAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        var result = await _permissionRepository.InsertAsync(entity, cancellationToken);

        // Invalidate cache after insert
        InvalidateCache();

        return result;
    }

    public async Task<Permission> UpdateAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        var result = await _permissionRepository.UpdateAsync(entity, cancellationToken);

        // Invalidate cache after update
        InvalidateCache();
        InvalidateEntityCache(entity.Id);

        return result;
    }

    public async Task DeleteAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        await _permissionRepository.DeleteAsync(entity, cancellationToken);

        // Invalidate cache after delete
        InvalidateCache();
        InvalidateEntityCache(entity.Id);
    }

    public void Insert(Permission entity)
    {
        _permissionRepository.Insert(entity);
        // Note: Cache invalidation will happen when UnitOfWork.SaveChanges is called
    }

    public void Update(Permission entity)
    {
        _permissionRepository.Update(entity);
        // Note: Cache invalidation will happen when UnitOfWork.SaveChanges is called
    }

    public void Delete(Permission entity)
    {
        _permissionRepository.Delete(entity);
        // Note: Cache invalidation will happen when UnitOfWork.SaveChanges is called
    }

    public IQueryable<Permission> GetQueryable()
    {
        return _permissionRepository.GetQueryable();
    }
    
    public async Task<int> CountAsync(IQueryable<Permission> query, CancellationToken cancellationToken = default)
    {
        return await _permissionRepository.CountAsync(query, cancellationToken);
    }
    
    public async Task<IReadOnlyList<Permission>> GetPagedAsync(IQueryable<Permission> query, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _permissionRepository.GetPagedAsync(query, skip, take, cancellationToken);
    }
    
    /// <summary>
    /// Invalidates all cached permissions
    /// </summary>
    private void InvalidateCache()
    {
        _cache.Remove(CacheKeyService.GetAllEntitiesKey(EntityTypeName));
    }

    /// <summary>
    /// Invalidates cached permission by ID
    /// </summary>
    /// <param name="id">The permission ID to invalidate</param>
    private void InvalidateEntityCache(Guid id)
    {
        _cache.Remove(CacheKeyService.GetEntityByIdKey(EntityTypeName, id));
    }
}
