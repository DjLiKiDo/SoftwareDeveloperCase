namespace SoftwareDeveloperCase.Application.Features.Projects.DTOs;

/// <summary>
/// DTO for creating a new project
/// </summary>
public class CreateProjectDto
{
    /// <summary>
    /// Project name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Project description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Team ID that will own the project
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// Project start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Estimated completion date
    /// </summary>
    public DateTime? EstimatedCompletionDate { get; set; }
}
