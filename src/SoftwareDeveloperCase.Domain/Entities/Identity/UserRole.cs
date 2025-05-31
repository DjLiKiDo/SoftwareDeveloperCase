using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities.Core;

namespace SoftwareDeveloperCase.Domain.Entities.Identity;

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
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the identifier of the role.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the role associated with this user.
    /// </summary>
    public Role Role { get; set; } = null!;
}
