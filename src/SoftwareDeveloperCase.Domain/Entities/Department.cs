using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities;

/// <summary>
/// Represents a department within the organization.
/// </summary>
public class Department : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the department.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the collection of users belonging to this department.
    /// </summary>
    public ICollection<User> Users { get; set; } = new List<User>();
}
