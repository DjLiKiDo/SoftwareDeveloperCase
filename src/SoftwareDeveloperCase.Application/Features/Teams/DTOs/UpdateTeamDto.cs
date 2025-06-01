namespace SoftwareDeveloperCase.Application.Features.Teams.DTOs;

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
