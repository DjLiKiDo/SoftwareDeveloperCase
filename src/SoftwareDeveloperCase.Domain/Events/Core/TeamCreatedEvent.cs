namespace SoftwareDeveloperCase.Domain.Events.Core;

/// <summary>
/// Domain event raised when a team is created.
/// </summary>
public record TeamCreatedEvent(
    Guid TeamId,
    string TeamName,
    DateTime CreatedAt);
