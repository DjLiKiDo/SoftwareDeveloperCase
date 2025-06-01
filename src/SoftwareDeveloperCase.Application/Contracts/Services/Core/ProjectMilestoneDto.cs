namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for project milestones
/// </summary>
public class ProjectMilestoneDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string Description { get; set; } = string.Empty;
}
