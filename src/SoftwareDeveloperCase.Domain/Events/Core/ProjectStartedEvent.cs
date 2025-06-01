using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Domain.Events.Core;

/// <summary>
/// Domain event raised when a project is started.
/// </summary>
public record ProjectStartedEvent(
    Guid ProjectId,
    string ProjectName,
    Guid TeamId,
    ProjectStatus Status,
    DateTime StartedAt);
