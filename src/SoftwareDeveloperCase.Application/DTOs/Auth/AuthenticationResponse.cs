namespace SoftwareDeveloperCase.Application.DTOs.Auth;

/// <summary>
/// Authentication response DTO containing access and refresh tokens
/// </summary>
public class AuthenticationResponse
{
    /// <summary>
    /// Gets or sets the access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiration time
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the user information
    /// </summary>
    public UserInfo User { get; set; } = null!;
}