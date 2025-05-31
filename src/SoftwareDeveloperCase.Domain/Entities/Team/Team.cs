using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Domain.Entities.Team;

/// <summary>
/// Represents a team in the system.
/// </summary>
public class Team : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the team.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the team.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the collection of team members.
    /// </summary>
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    /// <summary>
    /// Gets or sets the collection of projects assigned to this team.
    /// </summary>
    public ICollection<Entities.Project.Project> Projects { get; set; } = new List<Entities.Project.Project>();
}
