namespace SoftwareDeveloperCase.Domain.Enums.Identity;

/// <summary>
/// Represents the system roles for users.
/// </summary>
public enum SystemRole
{
    /// <summary>
    /// Administrator with full system access.
    /// </summary>
    Admin = 0,

    /// <summary>
    /// Manager with team and project management capabilities.
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Developer with task execution capabilities.
    /// </summary>
    Developer = 2
}
