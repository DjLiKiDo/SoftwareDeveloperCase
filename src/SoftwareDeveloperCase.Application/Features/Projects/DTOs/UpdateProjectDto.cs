using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Features.Projects.DTOs;

/// <summary>
/// DTO for updating a project
/// </summary>
public class UpdateProjectDto
{
    /// <summary>
    /// Project ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Project name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Project description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Project status
    /// </summary>
    public ProjectStatus Status { get; set; }

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

    /// <summary>
    /// Actual completion date
    /// </summary>
    public DateTime? ActualCompletionDate { get; set; }
}
