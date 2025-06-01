namespace SoftwareDeveloperCase.Domain.Events.Core;

/// <summary>
/// Domain event raised when a task is assigned to a user.
/// </summary>
public record TaskAssignedEvent(
    Guid TaskId,
    string TaskTitle,
    Guid ProjectId,
    Guid AssignedToUserId,
    Guid? AssignedByUserId,
    DateTime AssignedAt);
