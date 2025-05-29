using Microsoft.Extensions.Caching.Memory;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities;
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

    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        var cacheKey = CacheKeyService.GetEntityByIdKey(EntityTypeName, id);

        if (_cache.TryGetValue(cacheKey, out Permission? cachedPermission))
        {
            return cachedPermission;
        }

        var permission = await _permissionRepository.GetByIdAsync(id);

        if (permission != null)
        {
            _cache.Set(cacheKey, permission, CacheExpirationForSingleEntity);
        }

        return permission;
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        var cacheKey = CacheKeyService.GetAllEntitiesKey(EntityTypeName);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Permission>? cachedPermissions))
        {
            return cachedPermissions!;
        }

        var permissions = await _permissionRepository.GetAllAsync();
        _cache.Set(cacheKey, permissions, CacheExpirationForCollections);

        return permissions;
    }

    public async Task<IEnumerable<Permission>> GetAsync(Expression<Func<Permission, bool>> predicate)
    {
        // For filtered queries, we don't cache as the combinations are endless
        return await _permissionRepository.GetAsync(predicate);
    }

    public async Task<IEnumerable<Permission>> GetAsync(Expression<Func<Permission, bool>>? predicate = null,
        Func<IQueryable<Permission>, IOrderedQueryable<Permission>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true)
    {
        // For complex queries, we don't cache as the combinations are endless
        return await _permissionRepository.GetAsync(predicate, orderBy, includeString, disableTracking);
    }

    public async Task<IEnumerable<Permission>> GetAsync(Expression<Func<Permission, bool>>? predicate = null,
        Func<IQueryable<Permission>, IOrderedQueryable<Permission>>? orderBy = null,
        List<Expression<Func<Permission, object>>>? includes = null,
        bool disableTracking = true)
    {
        // For complex queries, we don't cache as the combinations are endless
        return await _permissionRepository.GetAsync(predicate, orderBy, includes, disableTracking);
    }

    public async Task<Permission> InsertAsync(Permission entity)
    {
        var result = await _permissionRepository.InsertAsync(entity);

        // Invalidate cache after insert
        InvalidateCache();

        return result;
    }

    public async Task<Permission> UpdateAsync(Permission entity)
    {
        var result = await _permissionRepository.UpdateAsync(entity);

        // Invalidate cache after update
        InvalidateCache();
        InvalidateEntityCache(entity.Id);

        return result;
    }

    public async Task DeleteAsync(Permission entity)
    {
        await _permissionRepository.DeleteAsync(entity);

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
