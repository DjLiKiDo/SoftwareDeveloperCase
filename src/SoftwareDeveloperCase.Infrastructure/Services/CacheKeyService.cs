using SoftwareDeveloperCase.Application.Contracts.Services;

namespace SoftwareDeveloperCase.Infrastructure.Services;

/// <summary>
/// Service for generating consistent cache keys across the application
/// </summary>
public class CacheKeyService : ICacheKeyService
{
    /// <summary>
    /// Creates a cache key for a specific entity with its ID
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="id">The entity identifier</param>
    /// <returns>A string cache key</returns>
    public string GenerateEntityCacheKey<T>(string id) where T : class
    {
        return $"{typeof(T).Name.ToLowerInvariant()}:id:{id}";
    }

    /// <summary>
    /// Creates a cache key for a collection of entities
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <returns>A string cache key</returns>
    public string GenerateListCacheKey<T>() where T : class
    {
        return $"{typeof(T).Name.ToLowerInvariant()}:all";
    }

    /// <summary>
    /// Creates a cache key for a filtered collection of entities
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="filter">A string describing the filter being applied</param>
    /// <returns>A string cache key</returns>
    public string GenerateFilteredListCacheKey<T>(string filter) where T : class
    {
        return $"{typeof(T).Name.ToLowerInvariant()}:filter:{filter}";
    }

    /// <summary>
    /// Generates a cache key pattern for all keys related to an entity type
    /// </summary>
    /// <param name="entityType">The entity type name</param>
    /// <returns>The cache key pattern</returns>
    public static string GetEntityPatternKey(string entityType)
    {
        return $"{entityType.ToLowerInvariant()}:*";
    }
}
