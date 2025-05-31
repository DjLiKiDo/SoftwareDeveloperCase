using SoftwareDeveloperCase.Application.DTOs.Common;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.DTOs.Teams;

/// <summary>
/// DTO for Team entity
/// </summary>
public class TeamDto : AuditableDto
{
    /// <summary>
    /// Team unique identifier
    /// </summary>
    public new int Id { get; set; }

    /// <summary>
    /// Team name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Team description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Team leader user ID
    /// </summary>
    public int? TeamLeaderId { get; set; }

    /// <summary>
    /// Team leader name
    /// </summary>
    public string? TeamLeaderName { get; set; }

    /// <summary>
    /// Number of active members
    /// </summary>
    public int MemberCount { get; set; }

    /// <summary>
    /// Number of active projects
    /// </summary>
    public int ProjectCount { get; set; }

    /// <summary>
    /// Team members
    /// </summary>
    public List<TeamMemberDto> Members { get; set; } = [];
}

/// <summary>
/// DTO for TeamMember entity
/// </summary>
public class TeamMemberDto : AuditableDto
{
    /// <summary>
    /// TeamMember unique identifier
    /// </summary>
    public new int Id { get; set; }

    /// <summary>
    /// Team ID
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// User email
    /// </summary>
    public string UserEmail { get; set; } = string.Empty;

    /// <summary>
    /// Team role (Leader, Member)
    /// </summary>
    public TeamRole TeamRole { get; set; }

    /// <summary>
    /// Member status
    /// </summary>
    public MemberStatus Status { get; set; }

    /// <summary>
    /// Date joined the team
    /// </summary>
    public DateTime JoinedDate { get; set; }
}

/// <summary>
/// DTO for creating a new team
/// </summary>
public class CreateTeamDto
{
    /// <summary>
    /// Team name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Team description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Team leader user ID
    /// </summary>
    public int? TeamLeaderId { get; set; }

    /// <summary>
    /// Initial team members
    /// </summary>
    public List<CreateTeamMemberDto> Members { get; set; } = [];
}

/// <summary>
/// DTO for creating a team member
/// </summary>
public class CreateTeamMemberDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Team role
    /// </summary>
    public TeamRole TeamRole { get; set; }

    /// <summary>
    /// Member status
    /// </summary>
    public MemberStatus Status { get; set; } = MemberStatus.Active;
}

/// <summary>
/// DTO for updating a team
/// </summary>
public class UpdateTeamDto
{
    /// <summary>
    /// Team ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Team name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Team description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Team leader user ID
    /// </summary>
    public int? TeamLeaderId { get; set; }
}
