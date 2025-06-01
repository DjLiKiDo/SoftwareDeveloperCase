using MediatR;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Command for user logout
/// </summary>
public record LogoutCommand(Guid UserId, string RefreshToken) : IRequest;