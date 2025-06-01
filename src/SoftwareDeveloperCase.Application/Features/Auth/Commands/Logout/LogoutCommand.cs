using MediatR;
using SoftwareDeveloperCase.Application.Attributes;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Command for user logout
/// </summary>
public class LogoutCommand : IRequest
{
    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the refresh token to revoke
    /// </summary>
    [SkipSanitization("Refresh token should not be sanitized as it could alter the token value")]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommand"/> class
    /// </summary>
    public LogoutCommand() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommand"/> class
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="refreshToken">The refresh token to revoke</param>
    public LogoutCommand(Guid userId, string refreshToken)
    {
        UserId = userId;
        RefreshToken = refreshToken;
    }
}
