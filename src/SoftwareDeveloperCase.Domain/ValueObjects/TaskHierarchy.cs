using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.ValueObjects;

/// <summary>
/// Represents task hierarchy information.
/// </summary>
public class TaskHierarchy : ValueObject
{
    /// <summary>
    /// Gets the level in the hierarchy (0 for root tasks).
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Gets the path from root to this task (e.g., "1.2.3").
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the order within the same level.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskHierarchy"/> class.
    /// </summary>
    /// <param name="level">The hierarchy level.</param>
    /// <param name="path">The hierarchy path.</param>
    /// <param name="order">The order within the level.</param>
    public TaskHierarchy(int level, string path, int order)
    {
        if (level < 0)
            throw new ArgumentException("Level cannot be negative.", nameof(level));

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        if (order < 0)
            throw new ArgumentException("Order cannot be negative.", nameof(order));

        Level = level;
        Path = path;
        Order = order;
    }

    /// <summary>
    /// Creates a root task hierarchy.
    /// </summary>
    /// <param name="order">The order of the root task.</param>
    /// <returns>A new TaskHierarchy instance for a root task.</returns>
    public static TaskHierarchy CreateRoot(int order) => new(0, order.ToString(), order);

    /// <summary>
    /// Creates a child task hierarchy.
    /// </summary>
    /// <param name="parentHierarchy">The parent task hierarchy.</param>
    /// <param name="order">The order of the child task.</param>
    /// <returns>A new TaskHierarchy instance for a child task.</returns>
    public static TaskHierarchy CreateChild(TaskHierarchy parentHierarchy, int order)
    {
        var newPath = $"{parentHierarchy.Path}.{order}";
        return new TaskHierarchy(parentHierarchy.Level + 1, newPath, order);
    }

    /// <summary>
    /// Gets the parent path if this is not a root task.
    /// </summary>
    /// <returns>The parent path or null if this is a root task.</returns>
    public string? GetParentPath()
    {
        if (Level == 0) return null;

        var lastDotIndex = Path.LastIndexOf('.');
        return lastDotIndex > 0 ? Path[..lastDotIndex] : null;
    }

    /// <summary>
    /// Checks if this task is a root task.
    /// </summary>
    public bool IsRoot => Level == 0;

    /// <summary>
    /// Gets the components used for equality comparison.
    /// </summary>
    /// <returns>The hierarchy path components for equality comparison.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Level;
        yield return Path;
        yield return Order;
    }

    /// <summary>
    /// Returns the string representation of the task hierarchy.
    /// </summary>
    /// <returns>The hierarchy path.</returns>
    public override string ToString() => Path;
}
