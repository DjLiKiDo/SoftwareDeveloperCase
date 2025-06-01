using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Events.Identity;

/// <summary>
/// Domain event raised when a role is assigned to a user.
/// </summary>
public record RoleAssignedEvent(
    Guid UserId,
    Guid RoleId,
    string RoleName,
    Guid? AssignedByUserId,
    DateTime AssignedAt) : IDomainEvent;
