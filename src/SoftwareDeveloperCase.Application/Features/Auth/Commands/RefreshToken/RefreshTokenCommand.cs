using MediatR;
using SoftwareDeveloperCase.Application.DTOs.Auth;
using SoftwareDeveloperCase.Application.Attributes;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Command for refreshing authentication tokens
/// </summary>
public class RefreshTokenCommand : IRequest<AuthenticationResponse>
{
    /// <summary>
    /// Gets or sets the refresh token
    /// </summary>
    [SkipSanitization("Refresh token should not be sanitized as it could alter the token value")]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommand"/> class
    /// </summary>
    public RefreshTokenCommand() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommand"/> class
    /// </summary>
    /// <param name="refreshToken">The refresh token</param>
    public RefreshTokenCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
