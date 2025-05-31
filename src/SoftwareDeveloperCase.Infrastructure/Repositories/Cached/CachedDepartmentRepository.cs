using Microsoft.Extensions.Caching.Memory;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.Entities.Lookups;
using SoftwareDeveloperCase.Infrastructure.Services;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SoftwareDeveloperCase.Infrastructure.Repositories.Cached;

/// <summary>
/// Cached decorator for DepartmentRepository implementing the decorator pattern
/// </summary>
internal class CachedDepartmentRepository : IDepartmentRepository
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheExpirationForCollections = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CacheExpirationForSingleEntity = TimeSpan.FromMinutes(10);
    private const string EntityTypeName = "Department";

    public CachedDepartmentRepository(IDepartmentRepository departmentRepository, IMemoryCache cache)
    {
        _departmentRepository = departmentRepository;
        _cache = cache;
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyService.GetEntityByIdKey(EntityTypeName, id);

        if (_cache.TryGetValue(cacheKey, out Department? cachedDepartment))
        {
            return cachedDepartment;
        }

        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);

        if (department != null)
        {
            _cache.Set(cacheKey, department, CacheExpirationForSingleEntity);
        }

        return department;
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyService.GetAllEntitiesKey(EntityTypeName);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Department>? cachedDepartments))
        {
            return cachedDepartments!;
        }

        var departments = await _departmentRepository.GetAllAsync(cancellationToken);
        _cache.Set(cacheKey, departments, CacheExpirationForCollections);

        return departments;
    }

    public async Task<IEnumerable<Department>> GetAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // For filtered queries, we don't cache as the combinations are endless
        return await _departmentRepository.GetAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<Department>> GetAsync(Expression<Func<Department, bool>>? predicate = null,
        Func<IQueryable<Department>, IOrderedQueryable<Department>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, we don't cache as the combinations are endless
        return await _departmentRepository.GetAsync(predicate, orderBy, includeString, disableTracking, cancellationToken);
    }

    public async Task<IEnumerable<Department>> GetAsync(Expression<Func<Department, bool>>? predicate = null,
        Func<IQueryable<Department>, IOrderedQueryable<Department>>? orderBy = null,
        List<Expression<Func<Department, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, we don't cache as the combinations are endless
        return await _departmentRepository.GetAsync(predicate, orderBy, includes, disableTracking, cancellationToken);
    }

    public async Task<List<User>> GetManagersAsync(Guid departmentId)
    {
        var cacheKey = $"{EntityTypeName.ToLowerInvariant()}:managers:{departmentId}";

        if (_cache.TryGetValue(cacheKey, out List<User>? cachedManagers))
        {
            return cachedManagers!;
        }

        var managers = await _departmentRepository.GetManagersAsync(departmentId);
        _cache.Set(cacheKey, managers, CacheExpirationForCollections);

        return managers;
    }

    public async Task<Department> InsertAsync(Department entity, CancellationToken cancellationToken = default)
    {
        var result = await _departmentRepository.InsertAsync(entity, cancellationToken);

        // Invalidate cache after insert
        InvalidateCache();

        return result;
    }

    public async Task<Department> UpdateAsync(Department entity, CancellationToken cancellationToken = default)
    {
        var result = await _departmentRepository.UpdateAsync(entity, cancellationToken);

        // Invalidate cache after update
        InvalidateCache();
        InvalidateEntityCache(entity.Id);
        InvalidateManagersCache(entity.Id);

        return result;
    }

    public async System.Threading.Tasks.Task DeleteAsync(Department entity, CancellationToken cancellationToken = default)
    {
        await _departmentRepository.DeleteAsync(entity, cancellationToken);

        // Invalidate cache after delete
        InvalidateCache();
        InvalidateEntityCache(entity.Id);
        InvalidateManagersCache(entity.Id);
    }

    public void Insert(Department entity)
    {
        _departmentRepository.Insert(entity);
        // Note: Cache invalidation will happen when UnitOfWork.SaveChanges is called
    }

    public void Update(Department entity)
    {
        _departmentRepository.Update(entity);
        // Note: Cache invalidation will happen when UnitOfWork.SaveChanges is called
    }

    public void Delete(Department entity)
    {
        _departmentRepository.Delete(entity);
        // Note: Cache invalidation will happen when UnitOfWork.SaveChanges is called
    }

    /// <summary>
    /// Invalidates all cached departments
    /// </summary>
    private void InvalidateCache()
    {
        _cache.Remove(CacheKeyService.GetAllEntitiesKey(EntityTypeName));
    }

    /// <summary>
    /// Invalidates cached department by ID
    /// </summary>
    /// <param name="id">The department ID to invalidate</param>
    private void InvalidateEntityCache(Guid id)
    {
        _cache.Remove(CacheKeyService.GetEntityByIdKey(EntityTypeName, id));
    }

    public IQueryable<Department> GetQueryable()
    {
        return _departmentRepository.GetQueryable();
    }
    
    public async Task<int> CountAsync(IQueryable<Department> query, CancellationToken cancellationToken = default)
    {
        return await _departmentRepository.CountAsync(query, cancellationToken);
    }
    
    public async Task<IReadOnlyList<Department>> GetPagedAsync(IQueryable<Department> query, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _departmentRepository.GetPagedAsync(query, skip, take, cancellationToken);
    }
    
    /// <summary>
    /// Invalidates cached managers for a department
    /// </summary>
    /// <param name="departmentId">The department ID to invalidate managers cache</param>
    private void InvalidateManagersCache(Guid departmentId)
    {
        _cache.Remove($"{EntityTypeName.ToLowerInvariant()}:managers:{departmentId}");
    }
}
