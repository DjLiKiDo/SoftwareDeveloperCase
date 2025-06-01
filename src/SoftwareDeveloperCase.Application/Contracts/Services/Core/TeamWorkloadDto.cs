namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for team workload information
/// </summary>
public class TeamWorkloadDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int TotalMembers { get; set; }
    public int ActiveProjects { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public decimal AverageTasksPerMember { get; set; }
    public decimal WorkloadPercentage { get; set; }
}
