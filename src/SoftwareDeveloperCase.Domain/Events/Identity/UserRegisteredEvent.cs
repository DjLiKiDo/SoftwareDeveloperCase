namespace SoftwareDeveloperCase.Domain.Events.Identity;

/// <summary>
/// Domain event raised when a user is registered.
/// </summary>
public record UserRegisteredEvent(
    Guid UserId,
    string UserName,
    string Email,
    DateTime RegisteredAt);
