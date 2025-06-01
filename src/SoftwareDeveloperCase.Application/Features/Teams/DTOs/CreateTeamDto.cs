using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Features.Teams.DTOs;

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
