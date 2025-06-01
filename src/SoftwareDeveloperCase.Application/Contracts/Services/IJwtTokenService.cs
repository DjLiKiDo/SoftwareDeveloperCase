using System.Security.Claims;
using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Application.Contracts.Services;

/// <summary>
/// Service for handling JWT token operations
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified user
    /// </summary>
    /// <param name="user">The user to generate the token for</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The generated JWT token</returns>
    Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <returns>The generated refresh token</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT token and returns the claims
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>The claims principal if valid, null otherwise</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Gets the user ID from a JWT token
    /// </summary>
    /// <param name="token">The JWT token</param>
    /// <returns>The user ID if valid, null otherwise</returns>
    string? GetUserIdFromToken(string token);

    /// <summary>
    /// Gets the JWT ID from a token
    /// </summary>
    /// <param name="token">The JWT token</param>
    /// <returns>The JWT ID if valid, null otherwise</returns>
    string? GetJwtIdFromToken(string token);
}
