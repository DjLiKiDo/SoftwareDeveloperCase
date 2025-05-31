using Microsoft.Extensions.Caching.Memory;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities.Identity;
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

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyService.GetEntityByIdKey(EntityTypeName, id);

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

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyService.GetAllEntitiesKey(EntityTypeName);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Role>? cachedRoles))
        {
            return cachedRoles!;
        }

        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        _cache.Set(cacheKey, roles, CacheExpirationForCollections);

        return roles;
    }

    public async Task<IEnumerable<Role>> GetAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // For filtered queries, we don't cache as the combinations are endless
        return await _roleRepository.GetAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetAsync(Expression<Func<Role, bool>>? predicate = null,
        Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, we don't cache as the combinations are endless
        return await _roleRepository.GetAsync(predicate, orderBy, includeString, disableTracking, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetAsync(Expression<Func<Role, bool>>? predicate = null,
        Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
        List<Expression<Func<Role, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, we don't cache as the combinations are endless
        return await _roleRepository.GetAsync(predicate, orderBy, includes, disableTracking, cancellationToken);
    }

    public async Task<Role> InsertAsync(Role entity, CancellationToken cancellationToken = default)
    {
        var result = await _roleRepository.InsertAsync(entity, cancellationToken);

        // Invalidate cache after insert
        InvalidateCache();

        return result;
    }

    public async Task<Role> UpdateAsync(Role entity, CancellationToken cancellationToken = default)
    {
        var result = await _roleRepository.UpdateAsync(entity, cancellationToken);

        // Invalidate cache after update
        InvalidateCache();
        InvalidateEntityCache(entity.Id);

        return result;
    }

    public async Task DeleteAsync(Role entity, CancellationToken cancellationToken = default)
    {
        await _roleRepository.DeleteAsync(entity, cancellationToken);

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

    public IQueryable<Role> GetQueryable()
    {
        return _roleRepository.GetQueryable();
    }
    
    public async Task<int> CountAsync(IQueryable<Role> query, CancellationToken cancellationToken = default)
    {
        return await _roleRepository.CountAsync(query, cancellationToken);
    }
    
    public async Task<IReadOnlyList<Role>> GetPagedAsync(IQueryable<Role> query, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _roleRepository.GetPagedAsync(query, skip, take, cancellationToken);
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
