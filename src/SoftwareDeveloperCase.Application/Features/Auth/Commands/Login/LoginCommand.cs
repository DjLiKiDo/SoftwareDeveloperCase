using MediatR;
using SoftwareDeveloperCase.Application.DTOs.Auth;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command for user login
/// </summary>
public record LoginCommand(string Email, string Password) : IRequest<AuthenticationResponse>;