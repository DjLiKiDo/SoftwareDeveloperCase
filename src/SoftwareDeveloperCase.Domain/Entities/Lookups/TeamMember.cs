using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Domain.Entities.Lookups;

/// <summary>
/// Represents a member of a team.
/// </summary>
public class TeamMember : BaseEntity
{
    /// <summary>
    /// Gets or sets the team identifier.
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// Gets or sets the team this member belongs to.
    /// </summary>
    public Entities.Team.Team Team { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user who is a member of the team.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the role of the user in the team.
    /// </summary>
    public TeamRole TeamRole { get; set; } = TeamRole.Member;

    /// <summary>
    /// Gets or sets the status of the member in the team.
    /// </summary>
    public MemberStatus Status { get; set; } = MemberStatus.Active;

    /// <summary>
    /// Gets or sets the date when the user joined the team.
    /// </summary>
    public DateTime JoinedDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the user left the team (if applicable).
    /// </summary>
    public DateTime? LeftDate { get; set; }
}
