using SoftwareDeveloperCase.Application.DTOs.Common;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Features.Teams.DTOs;

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
