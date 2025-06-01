using SoftwareDeveloperCase.Application.DTOs.Common;

namespace SoftwareDeveloperCase.Application.Features.Teams.DTOs;

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
