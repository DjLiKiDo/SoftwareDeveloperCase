namespace SoftwareDeveloperCase.Domain.Enums.Core;

/// <summary>
/// Represents the status of a team member.
/// </summary>
public enum MemberStatus
{
    /// <summary>
    /// Member is active and available.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Member is inactive.
    /// </summary>
    Inactive = 1,

    /// <summary>
    /// Member is on leave.
    /// </summary>
    OnLeave = 2
}
