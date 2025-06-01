using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Domain.Entities.Team;

/// <summary>
/// Represents a team member relationship between a user and a team
/// </summary>
public class TeamMember : BaseEntity
{
    /// <summary>
    /// Gets or sets the team ID
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// Gets or sets the team navigation property
    /// </summary>
    public Team Team { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user navigation property
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the team role (Leader, Member)
    /// </summary>
    public TeamRole TeamRole { get; set; }

    /// <summary>
    /// Gets or sets the member status
    /// </summary>
    public MemberStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date the user joined the team
    /// </summary>
    public DateTime JoinedDate { get; set; }

    /// <summary>
    /// Gets or sets the date the user left the team (if applicable)
    /// </summary>
    public DateTime? LeftDate { get; set; }

    /// <summary>
    /// Initializes a new instance of the TeamMember class
    /// </summary>
    public TeamMember()
    {
        JoinedDate = DateTime.UtcNow;
        Status = MemberStatus.Active;
        TeamRole = TeamRole.Member;
    }

    /// <summary>
    /// Initializes a new instance of the TeamMember class with specified values
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="teamRole">The team role</param>
    /// <param name="status">The member status</param>
    public TeamMember(Guid teamId, Guid userId, TeamRole teamRole = TeamRole.Member, MemberStatus status = MemberStatus.Active) // Changed parameters to Guid
        : this()
    {
        TeamId = teamId;
        UserId = userId;
        TeamRole = teamRole;
        Status = status;
    }

    /// <summary>
    /// Marks the member as inactive and sets the left date
    /// </summary>
    public void MarkAsLeft()
    {
        Status = MemberStatus.Inactive;
        LeftDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactivates the member and clears the left date
    /// </summary>
    public void Reactivate()
    {
        Status = MemberStatus.Active;
        LeftDate = null;
    }

    /// <summary>
    /// Promotes the member to team leader
    /// </summary>
    public void PromoteToLeader()
    {
        TeamRole = TeamRole.Leader; // Ensured TeamRole is used
    }

    /// <summary>
    /// Demotes the member from team leader
    /// </summary>
    public void DemoteFromLeader()
    {
        TeamRole = TeamRole.Member; // Ensured TeamRole is used
    }
}
