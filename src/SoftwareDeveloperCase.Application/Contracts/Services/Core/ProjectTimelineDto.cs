namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for project timeline analytics
/// </summary>
public class ProjectTimelineDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? EstimatedCompletionDate { get; set; }
    public int TotalDays { get; set; }
    public int DaysElapsed { get; set; }
    public int DaysRemaining { get; set; }
    public decimal ProgressPercentage { get; set; }
    public bool IsDelayed { get; set; }
    public int DelayDays { get; set; }
    public List<ProjectMilestoneDto> Milestones { get; set; } = [];
}
