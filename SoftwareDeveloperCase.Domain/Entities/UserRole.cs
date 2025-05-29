using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between users and roles.
/// </summary>
public class UserRole : BaseEntity
{
    /// <summary>
    /// Gets or sets the identifier of the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user associated with this role.
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the role.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the role associated with this user.
    /// </summary>
    public virtual Role? Role { get; set; }
}
