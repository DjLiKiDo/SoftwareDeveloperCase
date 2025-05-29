using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the department the user belongs to.
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the department the user belongs to.
    /// </summary>
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Gets or sets the collection of user-role associations.
    /// </summary>
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
}
