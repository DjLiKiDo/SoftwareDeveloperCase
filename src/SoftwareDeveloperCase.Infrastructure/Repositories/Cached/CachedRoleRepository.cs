using Microsoft.Extensions.Caching.Memory;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Infrastructure.Services;
using System.Linq.Expressions;

namespace SoftwareDeveloperCase.Infrastructure.Repositories.Cached;

/// <summary>
/// Cached decorator for RoleRepository implementing the decorator pattern
/// </summary>
internal class CachedRoleRepository : IRoleRepository
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheExpirationForCollections = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CacheExpirationForSingleEntity = TimeSpan.FromMinutes(10);
    private const string EntityTypeName = "Role";

    public CachedRoleRepository(IRoleRepository roleRepository, IMemoryCache cache)
    {
        _roleRepository = roleRepository;
        _cache = cache;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        var cacheKey = CacheKeyService.GetEntityByIdKey(EntityTypeName, id);

        if (_cache.TryGetValue(cacheKey, out Role? cachedRole))
        {
            return cachedRole;
        }

        var role = await _roleRepository.GetByIdAsync(id);

        if (role != null)
        {
            _cache.Set(cacheKey, role, CacheExpirationForSingleEntity);
        }

        return role;
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        var cacheKey = CacheKeyService.GetAllEntitiesKey(EntityTypeName);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Role>? cachedRoles))
        {
            return cachedRoles!;
        }

        var roles = await _roleRepository.GetAllAsync();
        _cache.Set(cacheKey, roles, CacheExpirationForCollections);

        return roles;
    }

    public async Task<IEnumerable<Role>> GetAsync(Expression<Func<Role, bool>> predicate)
    {
        // For filtered queries, we don't cache as the combinations are endless
        return await _roleRepository.GetAsync(predicate);
    }

    public async Task<IEnumerable<Role>> GetAsync(Expression<Func<Role, bool>>? predicate = null,
        Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true)
    {
        // For complex queries, we don't cache as the combinations are endless
        return await _roleRepository.GetAsync(predicate, orderBy, includeString, disableTracking);
    }

    public async Task<IEnumerable<Role>> GetAsync(Expression<Func<Role, bool>>? predicate = null,
        Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
        List<Expression<Func<Role, object>>>? includes = null,
        bool disableTracking = true)
    {
        // For complex queries, we don't cache as the combinations are endless
        return await _roleRepository.GetAsync(predicate, orderBy, includes, disableTracking);
    }

    public async Task<Role> InsertAsync(Role entity)
    {
        var result = await _roleRepository.InsertAsync(entity);

        // Invalidate cache after insert
        InvalidateCache();

        return result;
    }

    public async Task<Role> UpdateAsync(Role entity)
    {
        var result = await _roleRepository.UpdateAsync(entity);

        // Invalidate cache after update
        InvalidateCache();
        InvalidateEntityCache(entity.Id);

        return result;
    }

    public async Task DeleteAsync(Role entity)
    {
        await _roleRepository.DeleteAsync(entity);

        // Invalidate cache after delete
        InvalidateCache();
        InvalidateEntityCache(entity.Id);
    }

    public void Insert(Role entity)
    {
        _roleRepository.Insert(entity);
        // Note: Cache invalidation will happen when UnitOfWork.SaveChanges is called
    }

    public void Update(Role entity)
    {
        _roleRepository.Update(entity);
        // Note: Cache invalidation will happen when UnitOfWork.SaveChanges is called
    }

    public void Delete(Role entity)
    {
        _roleRepository.Delete(entity);
        // Note: Cache invalidation will happen when UnitOfWork.SaveChanges is called
    }

    /// <summary>
    /// Invalidates all cached roles
    /// </summary>
    private void InvalidateCache()
    {
        _cache.Remove(CacheKeyService.GetAllEntitiesKey(EntityTypeName));
    }

    /// <summary>
    /// Invalidates cached role by ID
    /// </summary>
    /// <param name="id">The role ID to invalidate</param>
    private void InvalidateEntityCache(Guid id)
    {
        _cache.Remove(CacheKeyService.GetEntityByIdKey(EntityTypeName, id));
    }
}
