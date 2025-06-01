using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities.Identity;

/// <summary>
/// Represents a permission that can be granted to roles.
/// </summary>
public class Permission : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the permission.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the permission.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the collection of role-permission associations.
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
