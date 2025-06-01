namespace SoftwareDeveloperCase.Domain.Events.Core;

/// <summary>
/// Domain event raised when a task is completed.
/// </summary>
public record TaskCompletedEvent(
    Guid TaskId,
    string TaskTitle,
    Guid ProjectId,
    Guid? CompletedByUserId,
    Enums.Core.TaskStatus PreviousStatus,
    decimal ActualHours,
    DateTime CompletedAt);
