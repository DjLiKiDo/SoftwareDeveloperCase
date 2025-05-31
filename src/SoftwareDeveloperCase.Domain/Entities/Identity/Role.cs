using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities.Identity;

/// <summary>
/// Represents a role that can be assigned to users and contains permissions.
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the parent role for hierarchical roles.
    /// </summary>
    public Guid? ParentRoleId { get; set; }

    /// <summary>
    /// Gets or sets the parent role for hierarchical role structure.
    /// </summary>
    public Role? ParentRole { get; set; }

    /// <summary>
    /// Gets or sets the collection of user-role associations.
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Gets or sets the collection of role-permission associations.
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
