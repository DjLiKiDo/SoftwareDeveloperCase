using MediatR;
using SoftwareDeveloperCase.Application.DTOs.Auth;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Command for refreshing authentication tokens
/// </summary>
public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthenticationResponse>;