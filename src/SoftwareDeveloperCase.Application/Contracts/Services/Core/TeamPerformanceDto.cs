namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for team performance metrics
/// </summary>
public class TeamPerformanceDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TasksCompleted { get; set; }
    public int TasksCreated { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal AverageTaskDuration { get; set; }
    public decimal TotalHoursWorked { get; set; }
    public decimal Productivity { get; set; }
}
