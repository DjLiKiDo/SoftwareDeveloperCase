namespace SoftwareDeveloperCase.Application.Contracts.Services;

/// <summary>
/// Service interface for providing date and time operations
/// </summary>
public interface IDateTimeService
{
    /// <summary>
    /// Gets the current date and time
    /// </summary>
    DateTime Now { get; }
}
