namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for burndown chart data points
/// </summary>
public class BurndownDataPointDto
{
    public DateTime Date { get; set; }
    public int RemainingTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int IdealRemaining { get; set; }
}
