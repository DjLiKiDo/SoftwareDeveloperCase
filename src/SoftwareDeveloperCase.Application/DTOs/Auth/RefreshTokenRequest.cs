using SoftwareDeveloperCase.Application.Attributes;

namespace SoftwareDeveloperCase.Application.DTOs.Auth;

/// <summary>
/// Refresh token request DTO
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Gets or sets the refresh token
    /// </summary>
    [SkipSanitization("Refresh token should not be sanitized as it could alter the token value")]
    public string RefreshToken { get; set; } = string.Empty;
}