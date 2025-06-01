namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// DTO for burndown chart data points
/// </summary>
public class BurndownDataPointDto
{
    /// <summary>
    /// The date of the data point
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Number of tasks remaining to be completed
    /// </summary>
    public int RemainingTasks { get; set; }
    
    /// <summary>
    /// Number of tasks completed by this date
    /// </summary>
    public int CompletedTasks { get; set; }
    
    /// <summary>
    /// The ideal remaining tasks based on the perfect burndown rate
    /// </summary>
    public int IdealRemaining { get; set; }
}
