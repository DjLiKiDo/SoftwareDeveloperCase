using SoftwareDeveloperCase.Application.Attributes;

namespace SoftwareDeveloperCase.Application.DTOs.Auth;

/// <summary>
/// Login request DTO
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password
    /// </summary>
    [SkipSanitization("Password should not be sanitized as it could alter the authentication credential")]
    public string Password { get; set; } = string.Empty;
}