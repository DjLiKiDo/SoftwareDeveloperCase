namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for task burndown chart data
/// </summary>
public class TaskBurndownDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public List<BurndownDataPointDto> DataPoints { get; set; } = [];
    public decimal Velocity { get; set; }
    public DateTime? ProjectedCompletionDate { get; set; }
}
