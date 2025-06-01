namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for project milestones
/// </summary>
public class ProjectMilestoneDto
{
    /// <summary>
    /// Name of the milestone
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Due date for the milestone completion
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Indicates whether the milestone has been completed
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// The date when the milestone was actually completed
    /// </summary>
    public DateTime? CompletionDate { get; set; }

    /// <summary>
    /// Description of the milestone
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
