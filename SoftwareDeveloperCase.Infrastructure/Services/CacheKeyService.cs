namespace SoftwareDeveloperCase.Infrastructure.Services;

/// <summary>
/// Service for generating consistent cache keys across the application
/// </summary>
internal static class CacheKeyService
{
    /// <summary>
    /// Generates a cache key for all entities of a specific type
    /// </summary>
    /// <param name="entityType">The entity type name</param>
    /// <returns>The cache key for all entities</returns>
    public static string GetAllEntitiesKey(string entityType)
    {
        return $"{entityType.ToLowerInvariant()}:all";
    }

    /// <summary>
    /// Generates a cache key for a specific entity by ID
    /// </summary>
    /// <param name="entityType">The entity type name</param>
    /// <param name="id">The entity ID</param>
    /// <returns>The cache key for the specific entity</returns>
    public static string GetEntityByIdKey(string entityType, Guid id)
    {
        return $"{entityType.ToLowerInvariant()}:id:{id}";
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
