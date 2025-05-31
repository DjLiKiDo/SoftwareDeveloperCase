using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Features.Teams.DTOs;

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
