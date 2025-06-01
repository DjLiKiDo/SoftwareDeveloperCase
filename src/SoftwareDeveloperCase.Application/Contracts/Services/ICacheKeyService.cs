namespace SoftwareDeveloperCase.Application.Contracts.Services;

/// <summary>
/// Interface for generating consistent cache keys for entities and operations
/// </summary>
public interface ICacheKeyService
{
    /// <summary>
    /// Creates a cache key for a specific entity with its ID
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="id">The entity identifier</param>
    /// <returns>A string cache key</returns>
    string GenerateEntityCacheKey<T>(string id) where T : class;

    /// <summary>
    /// Creates a cache key for a collection of entities
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <returns>A string cache key</returns>
    string GenerateListCacheKey<T>() where T : class;

    /// <summary>
    /// Creates a cache key for a filtered collection of entities
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="filter">A string describing the filter being applied</param>
    /// <returns>A string cache key</returns>
    string GenerateFilteredListCacheKey<T>(string filter) where T : class;
}
