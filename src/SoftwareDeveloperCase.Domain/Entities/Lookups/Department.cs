using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities.Core;

namespace SoftwareDeveloperCase.Domain.Entities.Lookups;

/// <summary>
/// Represents a department within the organization.
/// </summary>
public class Department : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the department.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the department.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the collection of users belonging to this department.
    /// </summary>
    public ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Gets or sets the collection of teams belonging to this department.
    /// </summary>
    public ICollection<Team> Teams { get; set; } = new List<Team>();
}
