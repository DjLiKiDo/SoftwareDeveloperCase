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
    /// Gets or sets the number of failed login attempts.
    /// </summary>
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>
    /// Gets or sets the date and time when the account was locked out.
    /// </summary>
    public DateTime? LockedOutAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the account lockout expires.
    /// </summary>
    public DateTime? LockoutExpiresAt { get; set; }

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

    /// <summary>
    /// Gets or sets the collection of refresh tokens for this user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    /// <summary>
    /// Checks if the user account is currently locked out.
    /// </summary>
    /// <param name="currentTime">The current date and time</param>
    /// <returns>True if the account is locked out, false otherwise</returns>
    public bool IsLockedOut(DateTime currentTime)
    {
        return LockoutExpiresAt.HasValue && LockoutExpiresAt.Value > currentTime;
    }

    /// <summary>
    /// Increments the failed login attempts and potentially locks out the account.
    /// </summary>
    /// <param name="currentTime">The current date and time</param>
    /// <param name="maxFailedAttempts">Maximum allowed failed attempts before lockout (default: 5)</param>
    /// <param name="lockoutDurationMinutes">Duration of lockout in minutes (default: 15)</param>
    public void RecordFailedLogin(DateTime currentTime, int maxFailedAttempts = 5, int lockoutDurationMinutes = 15)
    {
        FailedLoginAttempts++;
        
        if (FailedLoginAttempts >= maxFailedAttempts)
        {
            LockedOutAt = currentTime;
            LockoutExpiresAt = currentTime.AddMinutes(lockoutDurationMinutes);
        }
    }

    /// <summary>
    /// Resets the failed login attempts counter after a successful login.
    /// </summary>
    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockedOutAt = null;
        LockoutExpiresAt = null;
    }
}
