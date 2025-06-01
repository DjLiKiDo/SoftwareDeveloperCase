using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.ValueObjects;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Domain.Entities.Task;
using SoftwareDeveloperCase.Domain.Entities.Team;

namespace SoftwareDeveloperCase.Domain.Entities;

/// <summary>
/// Represents a user in the system - focused on identity and authentication.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public Email Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the user is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the collection of user-role associations.
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Gets or sets the collection of team memberships for this user.
    /// </summary>
    public ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();

    /// <summary>
    /// Gets or sets the collection of tasks assigned to this user.
    /// </summary>
    public ICollection<Entities.Task.Task> AssignedTasks { get; set; } = new List<Entities.Task.Task>();

    /// <summary>
    /// Gets or sets the collection of task comments created by this user.
    /// </summary>
    public ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();
}
